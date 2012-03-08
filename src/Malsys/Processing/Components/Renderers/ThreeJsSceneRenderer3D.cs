using System;
using System.IO;
using System.Windows.Media.Media3D;
using Malsys.IO;
using Malsys.Media;

namespace Malsys.Processing.Components.Renderers {
	[Component("3D WebGl renderer", ComponentGroupNames.Renderers)]
	public class ThreeJsSceneRenderer3D : BaseRenderer3D {

		private IndentTextWriter writer;

		private int objCounetr;


		private const string lineGeometryName = "line";
		private const string lineMaterialName = "basic";

		public override void Initialize(ProcessContext ctxt) {
			base.Initialize(ctxt);

			ctxt.Logger.LogMessage("beta", MessageType.Warning, Position.Unknown,
				"3D renderer is not working properly.");
		}


		public override bool RequiresMeasure { get { return false; } }

		public override void BeginProcessing(bool measuring) {

			base.BeginProcessing(measuring);

			objCounetr = 0;

			if (measuring) {
				writer = null;
			}
			else {

				outputStream = context.OutputProvider.GetOutputStream<ThreeJsSceneRenderer3D>(
					"3D result from `{0}`".Fmt(context.Lsystem.Name),
					MimeType.Application.Json, false, globalAdditionalData);

				writer = new IndentTextWriter(new StreamWriter(outputStream));

				startFile();
			}
		}

		public override void EndProcessing() {

			base.EndProcessing();

			if (!measuring) {
				// we don't require measuring, so save even if there was no measure (while measuring, base method do this)
				measuredMin = currentMeasuredMin;
				measuredMax = currentMeasuredMax;

				endFile();
				writer.Close();
				writer = null;

			}
		}


		public override void DrawTo(Point3D endPoint, Quaternion rotation, double width, ColorF color) {

			if (!measuring) {
				var euclidRotation = rotation.ToEuclidRotation();
				//euclidRotation.Z -= MathHelper.PiHalf;

				startJsonObject("obj" + objCounetr++);
				{
					writeJsonValue("geometry", lineGeometryName);
					writeJson("\"materials\": [\"" + lineMaterialName + "\"]");
					writeJsonValue("position", Math3D.CountMiddlePoint(endPoint, lastPoint));
					writeJsonValue("rotation", euclidRotation);
					writeJsonValue("quaternion", rotation);
					writeJsonValue("useQuaternion", true);
					writeJsonValue("scale", new Vector3D(Math3D.Distance(endPoint, lastPoint), width , width));
					writeJsonValue("doubleSided", false);
					writeJsonValue("visible", true, false);
				}
				endJsonObject();

				base.DrawTo(endPoint, rotation, width, color);

				measure(endPoint);
			}
		}

		public override void DrawPolygon(Polygon3D polygon) {
			throw new NotImplementedException();
		}


		#region Json output methods

		private void startJsonObject(string name) {
			writer.WriteLine("\"" + name + "\": {");
			writer.Indent();
		}

		private void writeJson(string value, bool comma = true) {
			writer.WriteLine(value + (comma ? "," : ""));
		}

		private void writeJsonValue(string name, bool value, bool comma = true) {
			writer.WriteLine("\"{0}\": {1}{2}".Fmt(name, value ? "true" : "false", comma ? "," : ""));
		}

		private void writeJsonValue(string name, string value, bool comma = true) {
			writer.WriteLine("\"{0}\": \"{1}\"{2}".Fmt(name, value, comma ? "," : ""));
		}

		private void writeJsonValue(string name, double value, bool comma = true) {
			writer.WriteLine("\"{0}\": {1}{2}".FmtInvariant(name, value, comma ? "," : ""));
		}

		private void writeJsonValue(string name, Point3D value, bool comma = true) {
			writer.WriteLine("\"{0}\": [{1},{2},{3}]{4}".FmtInvariant(name, value.X, value.Y, value.Z, comma ? "," : ""));
		}

		private void writeJsonValue(string name, Vector3D value, bool comma = true) {
			writer.WriteLine("\"{0}\": [{1},{2},{3}]{4}".FmtInvariant(name, value.X, value.Y, value.Z, comma ? "," : ""));
		}

		private void writeJsonValue(string name, Quaternion value, bool comma = true) {
			writer.WriteLine("\"{0}\": [{1},{2},{3},{4}]{5}".FmtInvariant(name, value.W, value.X, value.Y, value.Z, comma ? "," : ""));
		}

		private void endJsonObject(bool comma = true) {
			writer.Unindent();
			writer.WriteLine("}" + (comma ? "," : ""));
		}

		private void startFile() {

			writer.WriteLine("{");

			startJsonObject("metadata");
			{
				writeJsonValue("formatVersion", 3);
				writeJsonValue("type", "scene", false);
			}
			endJsonObject();


			startJsonObject("geometries");
			{
				startJsonObject(lineGeometryName);
				{
					//writeJsonValue("type", "cylinder");
					//writeJsonValue("topRad", 0.5);
					//writeJsonValue("botRad", 0.5);
					//writeJsonValue("height", 1);
					//writeJsonValue("radSegs", 7);
					//writeJsonValue("heightSegs", 1, false);
					writeJsonValue("type", "cube");
					writeJsonValue("width", 1);
					writeJsonValue("height", 1);
					writeJsonValue("depth", 1);
					writeJsonValue("segmentsWidth", 1);
					writeJsonValue("segmentsHeight", 1);
					writeJsonValue("segmentsDepth", 1);
					writeJsonValue("flipped", false, false);
				}
				endJsonObject(false);
			}
			endJsonObject();


			startJsonObject("materials");
			{
				startJsonObject(lineMaterialName);
				{
					writeJsonValue("type", "MeshLambertMaterial");
					startJsonObject("parameters");
					{
						writeJsonValue("color", 16777215);
						writeJsonValue("shading", "flat", false);
					}
					endJsonObject(false);
				}
				endJsonObject();

				startJsonObject("red");
				{
					writeJsonValue("type", "MeshLambertMaterial");
					startJsonObject("parameters");
					{
						writeJsonValue("color", 16711680);
						writeJsonValue("shading", "flat", false);
					}
					endJsonObject(false);
				}
				endJsonObject();

				startJsonObject("green");
				{
					writeJsonValue("type", "MeshLambertMaterial");
					startJsonObject("parameters");
					{
						writeJsonValue("color", 65280);
						writeJsonValue("shading", "flat", false);
					}
					endJsonObject(false);
				}
				endJsonObject();

				startJsonObject("blue");
				{
					writeJsonValue("type", "MeshLambertMaterial");
					startJsonObject("parameters");
					{
						writeJsonValue("color", 255);
						writeJsonValue("shading", "flat", false);
					}
					endJsonObject(false);
				}
				endJsonObject(false);
			}
			endJsonObject();


			startJsonObject("lights");
			{
				startJsonObject("directionalLight");
				{
					writeJsonValue("type", "directional");
					writeJsonValue("direction", new Point3D(0, 1, 1));
					writeJsonValue("color", 16777215);
					writeJsonValue("intensity", 0.8, false);
				}
				endJsonObject(false);
			}
			endJsonObject();


			startJsonObject("defaults");
			{
				writeJsonValue("bgcolor", new Point3D(1, 1, 1));
				writeJsonValue("bgalpha", 1);
				writeJsonValue("camera", "camera", false);
			}
			endJsonObject();


			startJsonObject("objects");
		}

		private void endFile() {

			startJsonObject("x");
			{
				writeJsonValue("geometry", lineGeometryName);
				writeJson("\"materials\": [\"red\"]");
				writeJsonValue("position", new Point3D(10, 0, 0));
				writeJsonValue("rotation", new Vector3D(0, 0, MathHelper.PiHalf));
				writeJsonValue("scale", new Vector3D(1, 20, 1));
				writeJsonValue("visible", true, false);
			}
			endJsonObject();

			startJsonObject("y");
			{
				writeJsonValue("geometry", lineGeometryName);
				writeJson("\"materials\": [\"green\"]");
				writeJsonValue("position", new Point3D(0, 10, 0));
				writeJsonValue("rotation", new Vector3D(0, 0, 0));
				writeJsonValue("scale", new Vector3D(1, 20, 1));
				writeJsonValue("visible", true, false);
			}
			endJsonObject();

			startJsonObject("z");
			{
				writeJsonValue("geometry", lineGeometryName);
				writeJson("\"materials\": [\"blue\"]");
				writeJsonValue("position", new Point3D(0, 0, 10));
				writeJsonValue("rotation", new Vector3D(MathHelper.PiHalf, 0, 0));
				writeJsonValue("scale", new Vector3D(1, 20, 1));
				writeJsonValue("visible", true, false);
			}
			endJsonObject(false);

			endJsonObject();  // end "objects" object


			startJsonObject("cameras");
			{
				startJsonObject("camera");
				{
					writeJsonValue("type", "perspective");
					writeJsonValue("fov", 60);
					writeJsonValue("aspect", 1.33333);
					writeJsonValue("near", 1);
					writeJsonValue("far", 1e7);
					writeJsonValue("position", measuredMax);
					writeJsonValue("target", Math3D.CountMiddlePoint(measuredMin, measuredMax), false);
				}
				endJsonObject(false);
			}
			endJsonObject(false);


			writer.WriteLine("}");
		}

		#endregion


	}
}
