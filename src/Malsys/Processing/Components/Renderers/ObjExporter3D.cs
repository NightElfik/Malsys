// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Media3D;
using Malsys.IO;
using Malsys.Media;
using Malsys.Media.Triangulation;
using Malsys.Resources;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;
using System.IO.Packaging;

namespace Malsys.Processing.Components.Renderers {
	/// <summary>
	/// Provides commands for rendering 3D scene.
	/// Result is JavaScript script defining 3D scene in JavaScript 3D engine Three.js.
	/// </summary>
	/// <name>3D Three.js renderer</name>
	/// <group>Renderers</group>
	public class ObjExporter3D : BaseRenderer3D {

		const string objFileName = "model{0}.obj";
		const string mtlFileName = "model{0}.mtl";

		private Stream objStream, mtlStream;
		private TextWriter objWriter, mtlWriter;
		private Polygon3DTrianguler polygonTrianguler = new Polygon3DTrianguler();
		private Polygon3DTriangulerParameters polygonTriangulerParameters;


		private QuaternionRotation3D quatRotation;
		private RotateTransform3D rotTranformer;
		private TranslateTransform3D transTransformer;


		private HashSet<string> createdMetarials = new HashSet<string>();
		private string currMaterial;


		private int triangulationStrategy;


		protected long absoluteVertexNumber;



		#region User gettable & settable properties


		/// <summary>
		/// Polygon triangulation strategy.
		/// </summary>
		/// <expected>
		/// 0 for "fan from first point",
		/// 1 triangles with minimal angle are prioritized,
		/// 2 triangles with maximal angle are prioritized,
		/// 3 triangles with maximal distance from all other points are prioritized,
		/// 4 triangles with maximal distance from not-yet-triangulated points are prioritized</expected>
		/// <default>2</default>
		[AccessName("polygonTriangulationStrategy")]
		[UserSettable]
		public Constant PolygonTriangulationStrategy {
			set {
				triangulationStrategy = value.RoundedIntValue.Clamp(0, 4);
			}
		}

		[AccessName("addSuffixToFileNames")]
		[UserSettable]
		public Constant AddSuffixToFileNames {
			set {
				addFileNameSuffix = value.IsTrue;
			}
		}
		private uint fileSuffixNum = 0;
		private bool addFileNameSuffix;

		/// <summary>
		/// Set to false to turn off crappy detection of planar polygons and potentionally speedup processing.
		/// </summary>
		/// <expected>Number.</expected>
		/// <default>true</default>
		/// <typicalValue>false</typicalValue>
		[AccessName("detectPlanarPloygons")]
		[UserSettable]
		public Constant DetectPlanarPloygons { get; set; }

		#endregion


		public ObjExporter3D() {
			quatRotation = new QuaternionRotation3D();
			rotTranformer = new RotateTransform3D(quatRotation, 0, 0, 0);
			transTransformer = new TranslateTransform3D();
		}


		public override void Reset() {
			base.Reset();
			PolygonTriangulationStrategy = Constant.Two;
			addFileNameSuffix = false;
			DetectPlanarPloygons = Constant.True;
		}

		public override void Initialize(ProcessContext ctxt) {
			base.Initialize(ctxt);

			var triangleEvalFun = getTriangleEvaluationFunction();

			polygonTriangulerParameters = new Polygon3DTriangulerParameters() {
				TriangleEvalDelegate = triangleEvalFun.EvalDelegate,
				Ordering = triangleEvalFun.Ordering,
				RecountMode = triangleEvalFun.RecountMode,
				AttachedMultiplier = 1,
				DetectPlanarPolygons = DetectPlanarPloygons.IsTrue,
				MaxVarCoefOfDist = 0.2
			};

		}



		public override bool RequiresMeasure { get { return false; } }

		public override void BeginProcessing(bool measuring) {

			base.BeginProcessing(measuring);

			if (measuring) {
				objWriter = null;
				return;
			}

			currMaterial = null;
			createdMetarials.Clear();


			objStream = context.OutputProvider.GetOutputStream<ObjExporter3D>(
				"OBJ temp", MimeType.Application.OctetStream, true);
			objWriter = new StreamWriter(objStream);

			mtlStream = context.OutputProvider.GetOutputStream<ObjExporter3D>(
				"MTL temp", MimeType.Application.OctetStream, true);
			mtlWriter = new StreamWriter(mtlStream);

			outputStream = context.OutputProvider.GetOutputStream<ObjExporter3D>(
				"OBJ result from `{0}`".Fmt(context.Lsystem.Name),
				MimeType.Application.Zip, false, globalMetadata);

			startFiles();
		}

		public override void EndProcessing() {

			base.EndProcessing();

			if (measuring) {
				return;
			}

			createdMetarials.Clear();

			objWriter.Flush();
			mtlWriter.Flush();

			using (Package package = Package.Open(outputStream, FileMode.Create)) {
				Uri objFileUri = PackUriHelper.CreatePartUri(new Uri(getObjFileName(), UriKind.Relative));
				PackagePart objPackagePart = package.CreatePart(objFileUri, MimeType.Application.OctetStream, CompressionOption.Normal);
				objStream.Seek(0, SeekOrigin.Begin);
				objStream.CopyTo(objPackagePart.GetStream());

				Uri mtlFileUri = PackUriHelper.CreatePartUri(new Uri(getMtlFileName(), UriKind.Relative));
				PackagePart mtlackagePart = package.CreatePart(mtlFileUri, MimeType.Application.OctetStream, CompressionOption.Normal);
				mtlStream.Seek(0, SeekOrigin.Begin);
				mtlStream.CopyTo(mtlackagePart.GetStream());
			}

			objWriter.Close();
			objWriter = null;

			mtlWriter.Close();
			mtlWriter = null;

			outputStream.Flush();
			outputStream = null;

			if (addFileNameSuffix) {
				fileSuffixNum++;
			}
		}

		public override void MoveTo(Point3D endPoint, Quaternion rotation, double width, ColorF color) {
			saveLastState(endPoint, rotation, width, color);
		}

		public override void DrawTo(Point3D endPoint, Quaternion rotation, double width, ColorF color, double quality) {
			if (measuring) {
				return;
			}

			switchToMaterial(color);

			var middle = Math3D.CountMiddlePoint(endPoint, lastPoint);
			var scale = new Vector3D(Math3D.Distance(endPoint, lastPoint), width, width);

			writeBox(middle, scale, rotation);
			objWriter.WriteLine();

			saveLastState(endPoint, rotation, width, color);
		}

		public override void DrawPolygon(Polygon3D polygon) {
			if (measuring) {
				return;
			}

			if (polygon.Ponits.Count < 3) {
				Logger.LogMessage(Message.InvalidPolygon, polygon.Ponits.Count);
				return;
			}

			// draw stroke if its width is greater than zero
			if (polygon.StrokeWidth.EpsilonCompareTo(0) > 0) {
				if (polygon.Ponits.Count != polygon.Rotations.Count) {
					Logger.LogMessage(Message.InvalidPolygonPtRotCount, polygon.Ponits.Count);
				}

				MoveTo(polygon.Ponits[0], polygon.Rotations[0], polygon.StrokeWidth, polygon.StrokeColor);
				for (int i = 1; i < polygon.Ponits.Count; i++) {
					DrawTo(polygon.Ponits[i], polygon.Rotations[i], polygon.StrokeWidth, polygon.StrokeColor, 1);
				}
			}

			switchToMaterial(polygon.Color);

			foreach (var pt in polygon.Ponits) {
				writeVertex(pt);
			}

			int polVertCount = polygon.Ponits.Count;
			var indices = polygonTrianguler.Triangularize(polygon.Ponits, polygonTriangulerParameters);
			for (int i = 0; i < indices.Count; i += 3) {
				writeFace(indices[i] - polVertCount, indices[i + 1] - polVertCount, indices[i + 2] - polVertCount);
			}

			objWriter.WriteLine();

		}

		public override void DrawSphere(double radius, ColorF color, double quality) {
			if (measuring) {
				return;
			}

		}


		protected void writeBox(Point3D position, Vector3D scale, Quaternion rotation) {

			const double bs = 0.5;

			Point3D[] vertices = new Point3D[] {
				new Point3D(bs * scale.X, bs * scale.Y, bs * scale.Z),  // -8
				new Point3D(-bs * scale.X, bs * scale.Y, bs * scale.Z),  // -7
				new Point3D(-bs * scale.X, bs * scale.Y, bs * -scale.Z),  // -6
				new Point3D(bs * scale.X, bs * scale.Y, bs * -scale.Z),  // -5

				new Point3D(bs * scale.X, -bs * scale.Y, bs * scale.Z),  // -4
				new Point3D(-bs * scale.X, -bs * scale.Y, bs * scale.Z),  // -3
				new Point3D(-bs * scale.X, -bs * scale.Y, -bs * scale.Z),  // -2
				new Point3D(bs * scale.X, -bs * scale.Y, -bs * scale.Z)  // -1
			};

			quatRotation.Quaternion = rotation;
			rotTranformer.Transform(vertices);

			transTransformer.OffsetX = position.X;
			transTransformer.OffsetY = position.Y;
			transTransformer.OffsetZ = position.Z;

			transTransformer.Transform(vertices);

			foreach (var v in vertices) {
				writeVertex(v);
			}

			/*writer.WriteLine("f -8 -7 -6");  // y+
			writer.WriteLine("f -8 -5 -6");
			writer.WriteLine("f -4 -3 -2");  // y-
			writer.WriteLine("f -4 -1 -2");

			writer.WriteLine("f -8 -5 -1");  // x+
			writer.WriteLine("f -8 -4 -1");
			writer.WriteLine("f -7 -6 -2");  // x-
			writer.WriteLine("f -7 -3 -2");

			writer.WriteLine("f -8 -7 -3");  // z+
			writer.WriteLine("f -8 -4 -3");
			writer.WriteLine("f -6 -5 -1");  // z-
			writer.WriteLine("f -6 -2 -1");*/

			objWriter.WriteLine("f -8 -7 -6 -5");  // y+
			objWriter.WriteLine("f -4 -3 -2 -1");  // y-

			objWriter.WriteLine("f -8 -5 -1 -4");  // x+
			objWriter.WriteLine("f -7 -6 -2 -3");  // x-

			objWriter.WriteLine("f -8 -7 -3 -4");  // z+
			objWriter.WriteLine("f -6 -5 -1 -2");  // z-

		}

		protected long writeVertex(Point3D pt) {

			const int roundingPrec = 6;

			string str = "v {0} {1} {2}".FmtInvariant(Math.Round(pt.X, roundingPrec), Math.Round(pt.Y, roundingPrec), Math.Round(pt.Z, roundingPrec));
			objWriter.WriteLine(str);

			return absoluteVertexNumber++;
		}

		protected void writeFace(params long[] vertices) {

			objWriter.WriteLine("f " + vertices.JoinToString(" "));

		}


		protected void switchToMaterial(ColorF color) {

			string colorHexa = color.ToRgbHexString();
			string name = "material" + colorHexa;

			if (!createdMetarials.Contains(name)) {
				mtlWriter.WriteLine("newmtl " + name);
				string r = Math.Round(color.R, 3).ToString();
				string g = Math.Round(color.G, 3).ToString();
				string b = Math.Round(color.B, 3).ToString();
				mtlWriter.WriteLine("Ka 0 0 0");
				mtlWriter.WriteLine("Kd {0} {1} {2}".Fmt(r, g, b));
				mtlWriter.WriteLine("Ks 0 0 0");
				mtlWriter.WriteLine();
				createdMetarials.Add(name);
			}

			if (currMaterial != name) {
				objWriter.WriteLine("usemtl " + name);
				currMaterial = name;
			}

		}


		private string getObjFileName() {
			return objFileName.Fmt(addFileNameSuffix ? fileSuffixNum.ToString("D5") : "");
		}

		private string getMtlFileName() {
			return mtlFileName.Fmt(addFileNameSuffix ? fileSuffixNum.ToString("D5") : "");
		}


		private void startFiles() {

			absoluteVertexNumber = 0;

			objWriter.WriteLine("# Created by Malsys – http://malsys.cz");
			objWriter.WriteLine("# generated by L-system " + context.Lsystem.Name);
			objWriter.WriteLine();
			objWriter.WriteLine("mtllib " + getMtlFileName());
			objWriter.WriteLine();

			mtlWriter.WriteLine("# Created by Malsys – http://malsys.cz");
			mtlWriter.WriteLine("# generated by L-system " + context.Lsystem.Name);
			mtlWriter.WriteLine();

		}


		private TriangleEvaluationFunction getTriangleEvaluationFunction() {
			switch (triangulationStrategy) {
				case 1: return TriangleEvaluationFunctions.MinAngle;
				case 2: return TriangleEvaluationFunctions.MaxAngle;
				case 3: return TriangleEvaluationFunctions.MaxDistanceAll;
				case 4: return TriangleEvaluationFunctions.MaxDistanceRemaining;
				default: return TriangleEvaluationFunctions.AsInInputAsc;
			}
		}


		public enum Message {
			[Message(MessageType.Warning, "Invalid polygon with {0} points ignored.")]
			InvalidPolygon,
			[Message(MessageType.Warning, "Invalid polygon with different number of points and rotations, ignored.")]
			InvalidPolygonPtRotCount,
		}

	}
}
