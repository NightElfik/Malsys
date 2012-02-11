using System;
using System.Windows.Media.Media3D;
using Malsys.Media;
using Microsoft.FSharp.Collections;
using System.IO;
using Malsys.IO;

namespace Malsys.Processing.Components.Renderers {
	[Component("3D WebGl renderer", ComponentGroupNames.Renderers)]
	public class ThreeJsSceneRenderer3D : BaseRenderer3D {

		protected bool measuring;
		private IndentTextWriter writer;

		private int objCounetr;


		private const string lineGeometryName = "line";
		private const string lineMaterialName = "basic";



		public override bool RequiresMeasure { get { return false; } }

		public override void BeginProcessing(bool measuring) {

			this.measuring = measuring;
			objCounetr = 0;

			if (measuring) {
				writer = null;
			}
			else {

				outputStream = context.OutputProvider.GetOutputStream<ThreeJsSceneRenderer3D>(
					"3D result from `{0}`".Fmt(context.Lsystem.Name),
					MimeType.Application_Json, false, globalAdditionalData);

				writer = new IndentTextWriter(new StreamWriter(outputStream));

				startFile();
			}
		}

		public override void EndProcessing() {

			if (!measuring) {
				endFile();
				writer.Close();
				writer = null;
			}
		}


		public override void LineTo(Point3D endPoint, double forwardAxisRotation, double width, ColorF color) {

			if (!measuring) {
				Vector3D rotation = Math3D.CountRotation(Point3D.Subtract(lastPoint, endPoint), forwardAxisRotation);
				rotation.Z -= MathHelper.PiHalf;

				startJsonObject("obj" + objCounetr++);
				{
					writeJsonValue("geometry", lineGeometryName);
					writeJson("\"materials\": [\"" + lineMaterialName + "\"]");
					writeJsonValue("position", Math3D.CountMiddlePoint(endPoint, lastPoint));
					writeJsonValue("rotation", rotation);
					writeJsonValue("scale", new Vector3D(width, Math3D.Distance(endPoint, lastPoint), width));
					writeJsonValue("visible", true, false);
				}
				endJsonObject();

				base.LineTo(endPoint, forwardAxisRotation, width, color);
			}
		}

		public override void DrawPolygon(Polygon3D polygon) {
			throw new NotImplementedException();
		}



		private void startJsonObject(string name) {
			writer.WriteLine("\"" + name + "\": {");
			writer.Indent();
		}

		private void writeJson(string value, bool comma = true) {
			writer.WriteLine(value + (comma ? "," : ""));
		}

		private void writeJsonValue(string name, bool value, bool comma = true) {
			writer.WriteLine("\"{0}\": \"{1}\"{2}".Fmt(name, value ? "true" : "false", comma ? "," : ""));
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
					writeJsonValue("type", "cylinder");
					writeJsonValue("topRad", 0.5);
					writeJsonValue("botRad", 0.5);
					writeJsonValue("height", 1);
					writeJsonValue("radSegs", 7);
					writeJsonValue("heightSegs", 1, false);
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

			startJsonObject("obj" + objCounetr++);
			{
				writeJsonValue("geometry", lineGeometryName);
				writeJson("\"materials\": [\"" + lineMaterialName + "\"]");
				writeJsonValue("position", new Point3D(0, 0, 0));
				writeJsonValue("rotation", new Vector3D(0, 0, 0));
				writeJsonValue("scale", new Vector3D(1, 10, 1));
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
					writeJsonValue("far", 1000);
					writeJsonValue("position", new Point3D(50, 0, 100));
					writeJsonValue("target", new Point3D(50, 0, 0), false);
				}
				endJsonObject(false);
			}
			endJsonObject(false);


			writer.WriteLine("}");
		}




	}
}
