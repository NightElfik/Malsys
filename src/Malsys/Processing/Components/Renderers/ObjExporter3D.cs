// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Windows.Media.Media3D;
using Malsys.IO;
using Malsys.Media;
using Malsys.Media.Triangulation;
using Malsys.Resources;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components.Renderers {
	/// <summary>
	/// Provides commands for rendering 3D scene.
	/// Returns a 3D model in OBJ format together with MTL material description for colors.
	/// </summary>
	/// <name>OBJ exporter</name>
	/// <group>Renderers</group>
	public class ObjExporter3D : BaseRenderer3D {

		private Stream objStream, mtlStream, metaStream;
		private TextWriter objWriter, mtlWriter, metaWriter;
		private Polygon3DTrianguler polygonTrianguler = new Polygon3DTrianguler();
		private Polygon3DTriangulerParameters polygonTriangulerParameters;


		private QuaternionRotation3D quatRotation;
		private RotateTransform3D rotTranformer;
		private TranslateTransform3D transTransformer;


		private HashSet<string> createdMetarials = new HashSet<string>();
		private string currMaterial;


		private int triangulationStrategy;


		protected uint absoluteVertexNumber;



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
		/// Set to false to turn off (crappy) detection of planar polygons and potentially speedup processing.
		/// </summary>
		/// <expected>Number.</expected>
		/// <default>true</default>
		/// <typicalValue>false</typicalValue>
		[AccessName("detectPlanarPloygons")]
		[UserSettable]
		public Constant DetectPlanarPloygons { get; set; }

		/// <summary>
		/// Camera position. If not set it is counted automatically.
		/// </summary>
		/// <expected>Array 3 numbers representing x, y and z coordinate of camera position.</expected>
		/// <default>counted dynamically</default>
		[AccessName("cameraPosition")]
		[UserSettable]
		public ValuesArray CameraPosition {
			get { return cameraPosition; }
			set {
				if (!value.IsConstArrayOfLength(3)) {
					throw new InvalidUserValueException("Camera position must be array of 3 numbers representing x, y and z coordinate.");
				}
				cameraPosition = value;
			}
		}
		private ValuesArray cameraPosition;

		/// <summary>
		/// Camera up vector.
		/// </summary>
		/// <expected>Array 3 numbers representing x, y and z up vector of camera.</expected>
		/// <default>{0, 1, 0}</default>
		[AccessName("cameraUpVector")]
		[UserSettable]
		public ValuesArray CameraUpVector {
			get { return cameraUpVector; }
			set {
				if (!value.IsConstArrayOfLength(3)) {
					throw new InvalidUserValueException("Camera up vector must be array of 3 numbers representing x, y and z coordinate.");
				}
				cameraUpVector = value;
			}
		}
		private ValuesArray cameraUpVector;

		/// <summary>
		/// Camera target. If not set it is counted automatically.
		/// </summary>
		/// <expected>Array 3 numbers representing x, y and z coordinate of camera target.</expected>
		/// <default>counted dynamically</default>
		[AccessName("cameraTarget")]
		[UserSettable]
		public ValuesArray CameraTarget {
			get { return cameraTarget; }
			set {
				if (!value.IsConstArrayOfLength(3)) {
					throw new InvalidUserValueException("Camera target must be array of 3 numbers representing x, y and z coordinate.");
				}
				cameraTarget = value;
			}
		}
		private ValuesArray cameraTarget;

		/// <summary>
		/// Background color of rendered image.
		/// Some output formats may not support transparent backgrounds.
		/// </summary>
		/// <expected>Number representing ARGB color (in range from 0 to 2^32 - 1) or array of numbers (in range from 0.0 to 1.0) of length of 3 (RGB) or 4 (ARGB).</expected>
		/// <default>#FFFFFF (white)</default>
		[AccessName("bgColor")]
		[UserSettable]
		public IValue BackgroundColor { get; set; }
		protected ColorF bgColor;

		#endregion User gettable & settable properties


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
			BackgroundColor = 0xFFFFFF.ToConst();
			cameraPosition = null;
			cameraUpVector = null;
			cameraTarget = null;
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

			var colorParser = new ColorParser(Logger);
			colorParser.TryParseColor(BackgroundColor, out bgColor, Logger);

			currMaterial = null;
			createdMetarials.Clear();


			objStream = context.OutputProvider.GetOutputStream<ObjExporter3D>(
				"OBJ temp", MimeType.Application.OctetStream, true);
			objWriter = new StreamWriter(objStream);

			mtlStream = context.OutputProvider.GetOutputStream<ObjExporter3D>(
				"MTL temp", MimeType.Application.OctetStream, true);
			mtlWriter = new StreamWriter(mtlStream);

			metaStream = context.OutputProvider.GetOutputStream<ObjExporter3D>(
				"Metadata temp", MimeType.Application.Json, true);
			metaWriter = new StreamWriter(metaStream);

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

			generateMetadata();

			objWriter.Flush();
			mtlWriter.Flush();
			metaWriter.Flush();


			AddCurrentOutputData(OutputMetadataKeyHelper.ObjMetadata,
				getObjFileName() + " " + getMtlFileName() + " " + getMetadataFileName());

			using (Package package = Package.Open(outputStream, FileMode.Create)) {
				Uri objFileUri = PackUriHelper.CreatePartUri(new Uri(getObjFileName(), UriKind.Relative));
				PackagePart objPackagePart = package.CreatePart(objFileUri, MimeType.Application.OctetStream, CompressionOption.Normal);
				objStream.Seek(0, SeekOrigin.Begin);
				objStream.CopyTo(objPackagePart.GetStream());

				Uri mtlFileUri = PackUriHelper.CreatePartUri(new Uri(getMtlFileName(), UriKind.Relative));
				PackagePart mtlPackagePart = package.CreatePart(mtlFileUri, MimeType.Application.OctetStream, CompressionOption.Normal);
				mtlStream.Seek(0, SeekOrigin.Begin);
				mtlStream.CopyTo(mtlPackagePart.GetStream());

				Uri metaFileUri = PackUriHelper.CreatePartUri(new Uri(getMetadataFileName(), UriKind.Relative));
				PackagePart metaPackagePart = package.CreatePart(metaFileUri, MimeType.Application.Json, CompressionOption.Normal);
				metaStream.Seek(0, SeekOrigin.Begin);
				metaStream.CopyTo(metaPackagePart.GetStream());
			}

			objWriter.Close();
			objWriter = null;

			mtlWriter.Close();
			mtlWriter = null;

			metaWriter.Close();
			metaWriter = null;

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
			measure(endPoint, width);
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

			uint baseI = absoluteVertexNumber + 1;  // OBJ indices are one-based.
			foreach (var pt in polygon.Ponits) {
				writeVertex(pt);
				measure(pt);
			}

			int polVertCount = polygon.Ponits.Count;
			var indices = polygonTrianguler.Triangularize(polygon.Ponits, polygonTriangulerParameters);

			for (int i = 0; i < indices.Count; i += 3) {
				writeFace(baseI + indices[i], baseI + indices[i + 1], baseI + indices[i + 2]);
			}

			objWriter.WriteLine();

		}

		public override void DrawSphere(double radius, ColorF color, double quality) {
			if (measuring) {
				return;
			}
			// TODO.
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

			uint baseI = absoluteVertexNumber + 1;  // OBJ vertex indices are one-based.

			objWriter.WriteLine("f {0} {1} {2} {3}", baseI - 5, baseI - 6, baseI - 7, baseI - 8);  // y+
			objWriter.WriteLine("f {0} {1} {2} {3}", baseI - 4, baseI - 3, baseI - 2, baseI - 1);  // y-

			objWriter.WriteLine("f {0} {1} {2} {3}", baseI - 4, baseI - 1, baseI - 5, baseI - 8);  // x+
			objWriter.WriteLine("f {0} {1} {2} {3}", baseI - 7, baseI - 6, baseI - 2, baseI - 3);  // x-

			objWriter.WriteLine("f {0} {1} {2} {3}", baseI - 8, baseI - 7, baseI - 3, baseI - 4);  // z+
			objWriter.WriteLine("f {0} {1} {2} {3}", baseI - 6, baseI - 5, baseI - 1, baseI - 2);  // z-


			//objWriter.WriteLine("f -8 -7 -6 -5");  // y+
			//objWriter.WriteLine("f -4 -3 -2 -1");  // y-

			//objWriter.WriteLine("f -8 -5 -1 -4");  // x+
			//objWriter.WriteLine("f -7 -6 -2 -3");  // x-

			//objWriter.WriteLine("f -8 -7 -3 -4");  // z+
			//objWriter.WriteLine("f -6 -5 -1 -2");  // z-

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
			string name = "m" + colorHexa;

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
			return "model{0}.obj".Fmt(addFileNameSuffix ? fileSuffixNum.ToString("D5") : "");
		}

		private string getMtlFileName() {
			return "model{0}.mtl".Fmt(addFileNameSuffix ? fileSuffixNum.ToString("D5") : "");
		}

		private string getMetadataFileName() {
			return "model{0}.json".Fmt(addFileNameSuffix ? fileSuffixNum.ToString("D5") : "");
		}


		private void startFiles() {

			absoluteVertexNumber = 0;

			objWriter.WriteLine("# Created by Malsys: http://malsys.cz");
			objWriter.WriteLine("# L-system name: " + context.Lsystem.Name);
			objWriter.WriteLine();
			objWriter.WriteLine("mtllib " + getMtlFileName());
			objWriter.WriteLine();

			mtlWriter.WriteLine("# Created by Malsys: http://malsys.cz");
			mtlWriter.WriteLine("# L-system name: " + context.Lsystem.Name);
			mtlWriter.WriteLine();

		}

		private void generateMetadata() {
			Point3D camTarget = Math3D.ZeroPoint;
			if (cameraTarget != null) {
				camTarget.X = ((Constant)cameraTarget[0]).Value;
				camTarget.Y = ((Constant)cameraTarget[1]).Value;
				camTarget.Z = ((Constant)cameraTarget[2]).Value;
			}
			else {
				camTarget = Math3D.CountMiddlePoint(currentMeasuredMin, currentMeasuredMax);
			}

			Point3D camPosition = Math3D.ZeroPoint;
			if (cameraPosition != null) {
				camPosition.X = ((Constant)cameraPosition[0]).Value;
				camPosition.Y = ((Constant)cameraPosition[1]).Value;
				camPosition.Z = ((Constant)cameraPosition[2]).Value;
			}
			else {
				camPosition = Math3D.AddPoints(camTarget, currentMeasuredMax);
			}

			Point3D camUp = Math3D.ZeroPoint;
			if (cameraUpVector != null) {
				camUp.X = ((Constant)cameraUpVector[0]).Value;
				camUp.Y = ((Constant)cameraUpVector[1]).Value;
				camUp.Z = ((Constant)cameraUpVector[2]).Value;
			}
			else {
				camUp = new Point3D(0, 1, 0);
			}

			camUp.Normalize();

			metaWriter.WriteLine("{");
			metaWriter.WriteLine("  \"source\": \"http://malsys.cz\",");
			metaWriter.WriteLine("  \"timestamp\": \"{0}\",", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			metaWriter.WriteLine("  \"name\": \"{0}\",", context.Lsystem.Name);
			metaWriter.WriteLine("  \"bgColor\": \"{0}\",", bgColor.ToRgbHexString());
			metaWriter.WriteLine("  \"cameraPosition\": [{0}, {1}, {2}],", camPosition.X, camPosition.Y, camPosition.Z);
			metaWriter.WriteLine("  \"cameraUpVector\": [{0}, {1}, {2}],", camUp.X, camUp.Y, camUp.Z);
			metaWriter.WriteLine("  \"cameraTarget\": [{0}, {1}, {2}]", camTarget.X, camTarget.Y, camTarget.Z);
			metaWriter.WriteLine("}");
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
