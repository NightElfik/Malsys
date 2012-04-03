using System;
using System.IO;
using System.Windows.Media.Media3D;
using Malsys.IO;
using Malsys.Media;

namespace Malsys.Processing.Components.Renderers {
	[Component("3D WebGl renderer", ComponentGroupNames.Renderers)]
	public class ThreeJsSceneRenderer3D : BaseRenderer3D {

		private IndentTextWriter writer;


		private const string lineGeometryName = "line";
		private const string lineMaterialName = "basic";

		public override void Initialize(ProcessContext ctxt) {
			base.Initialize(ctxt);
		}


		public override bool RequiresMeasure { get { return false; } }

		public override void BeginProcessing(bool measuring) {

			base.BeginProcessing(measuring);

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

				// materials
				writer.WriteLine("material = new THREE.MeshLambertMaterial({{ color: 0x{0}, shading: THREE.FlatShading }});".Fmt(color.ToRgbHexString()));
				writer.WriteLine("mesh = new THREE.Mesh(lineGeometry, material);");

				var middle = Math3D.CountMiddlePoint(endPoint, lastPoint);
				writer.WriteLine("mesh.position.x = {0:0.000};".Fmt(middle.X));
				writer.WriteLine("mesh.position.y = {0:0.000};".Fmt(middle.Y));
				writer.WriteLine("mesh.position.z = {0:0.000};".Fmt(middle.Z));

				var euclidRotation = rotation.ToEuclidRotation();
				writer.WriteLine("mesh.eulerOrder = 'YZX';");
				writer.WriteLine("mesh.rotation.x = {0:0.000};".Fmt(euclidRotation.X));
				writer.WriteLine("mesh.rotation.y = {0:0.000};".Fmt(euclidRotation.Y));
				writer.WriteLine("mesh.rotation.z = {0:0.000};".Fmt(euclidRotation.Z));

				writer.WriteLine("mesh.scale.x = {0:0.000};".Fmt(Math3D.Distance(endPoint, lastPoint)));
				writer.WriteLine("mesh.scale.y = {0:0.000};".Fmt(width));
				writer.WriteLine("mesh.scale.z = {0:0.000};".Fmt(width));

				writer.WriteLine("mesh.updateMatrix();");
				writer.WriteLine("mesh.matrixAutoUpdate = false;");
				writer.WriteLine("objectHolder.add(mesh);");

				base.DrawTo(endPoint, rotation, width, color);

				measure(endPoint);
			}
		}

		public override void DrawPolygon(Polygon3D polygon) {
			throw new NotImplementedException();
		}

		public override void DrawSphere(Point3D center, double radius, ColorF color) {
			if (!measuring) {
				// materials
				writer.WriteLine("material = new THREE.MeshLambertMaterial({{ color: 0x{0}, shading: THREE.FlatShading }});".Fmt(color.ToRgbHexString()));
				writer.WriteLine("mesh = new THREE.Mesh(spgereGeometry, material);");

				writer.WriteLine("mesh.position.x = {0:0.000};".Fmt(center.X));
				writer.WriteLine("mesh.position.y = {0:0.000};".Fmt(center.Y));
				writer.WriteLine("mesh.position.z = {0:0.000};".Fmt(center.Z));

				if (radius.EpsilonCompareTo(1.0) != 0) {
					writer.WriteLine("mesh.scale.x = {0:0.000};".Fmt(radius));
					writer.WriteLine("mesh.scale.y = {0:0.000};".Fmt(radius));
					writer.WriteLine("mesh.scale.z = {0:0.000};".Fmt(radius));
				}

				writer.WriteLine("mesh.updateMatrix();");
				writer.WriteLine("mesh.matrixAutoUpdate = false;");
				writer.WriteLine("objectHolder.add(mesh);");

				var radPoint = new Point3D(radius, radius, radius);
				measure(Math3D.AddPoints(center, radPoint));
				measure(Math3D.SubtractPoints(center, radPoint));

			}
		}



		private void startFile() {

			writer.WriteLine("var Scene = function() {");  // create scope to not fill global scope with mess
			writer.WriteLine("var scene = new THREE.Scene();");

			// geometries
			writer.WriteLine("var lineGeometry = new THREE.CubeGeometry(1, 1, 1);");
			writer.WriteLine("var spgereGeometry = new THREE.SphereGeometry(1, 7, 5);");

			// objects
			writer.WriteLine("var objectHolder = new THREE.Object3D();");
			writer.WriteLine("scene.add(objectHolder);");

			writer.WriteLine("var mesh; var material;");  // mesh for creating objects

		}

		private void endFile() {

			var cameraTarget = Math3D.CountMiddlePoint(measuredMin, measuredMax);
			var cameraPos = Math3D.AddPoints(cameraTarget, measuredMax);
			var cameraPosNorm = ((Vector3D)cameraPos);
			cameraPosNorm.Normalize();

			// lights
			//writer.WriteLine("var ambientLight = new THREE.AmbientLight(0x111111);");
			//writer.WriteLine("scene.add(ambientLight);");

			// main light from camera
			writer.WriteLine("var directionalLight = new THREE.DirectionalLight(0xFFFFFF);");
			writer.WriteLine("directionalLight.position.x = {0:0.000};".Fmt(cameraPosNorm.X));
			writer.WriteLine("directionalLight.position.y = {0:0.000};".Fmt(cameraPosNorm.Y));
			writer.WriteLine("directionalLight.position.z = {0:0.000};".Fmt(cameraPosNorm.Z));
			writer.WriteLine("scene.add(directionalLight);");

			// secondary light to avoid completely black sides (not directly against primaty light)
			writer.WriteLine("directionalLight = new THREE.DirectionalLight(0x777777);");
			writer.WriteLine("directionalLight.position.x = {0:0.000};".Fmt(-cameraPosNorm.Y));
			writer.WriteLine("directionalLight.position.y = {0:0.000};".Fmt(-cameraPosNorm.X));
			writer.WriteLine("directionalLight.position.z = {0:0.000};".Fmt(-cameraPosNorm.Z));
			writer.WriteLine("scene.add(directionalLight);");

			writer.WriteLine("return {");
			writer.WriteLine("getScene: function() { return scene; },");
			writer.WriteLine("getCameraPosition: function() {{ return new THREE.Vector3( {0:0.000}, {1:0.000}, {2:0.000} ); }},"
				.Fmt(cameraPos.X, cameraPos.Y, cameraPos.Z));
			writer.WriteLine("getCameraTarget: function() {{ return new THREE.Vector3( {0:0.000}, {1:0.000}, {2:0.000} ); }}"
				.Fmt(cameraTarget.X, cameraTarget.Y, cameraTarget.Z));
			writer.WriteLine("};");

			writer.WriteLine("};");  // end of scope

		}

	}
}
