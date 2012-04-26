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

namespace Malsys.Processing.Components.Renderers {
	/// <summary>
	/// Provides commands for rendering 3D scene.
	/// Result is JavaScript script defining 3D scene in JavaScript 3D engine Three.js.
	/// </summary>
	/// <name>3D Three.js renderer</name>
	/// <group>Renderers</group>
	public class ThreeJsSceneRenderer3D : BaseRenderer3D {

		private IndentTextWriter writer;
		private Polygon3DTrianguler polygonTrianguler = new Polygon3DTrianguler();
		Polygon3DTriangulerParameters polygonTriangulerParameters;

		private bool smoothShading;

		private int triangulationStrategy;


		bool lineGeometryCreated;
		private HashSet<string> createdMetarials = new HashSet<string>();
		private HashSet<string> createdSphereGeomteries = new HashSet<string>();


		#region User gettable & settable properties

		/// <summary>
		/// If set to true, triangles will be shaded smoothly.
		/// This can improve quality of spheres or cylinders but it has no effect on cubes.
		/// Also it significantly reduces performance of rendering.
		/// </summary>
		/// <expected>true or false</expected>
		/// <default>false</default>
		[AccessName("smoothShading")]
		[UserSettable]
		public Constant SmoothShading {
			set {
				smoothShading = value.IsTrue;
			}
		}

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

		#endregion


		public override void Initialize(ProcessContext ctxt) {

			var triangleEvalFun = getTriangleEvaluationFunction();

			polygonTriangulerParameters = new Polygon3DTriangulerParameters() {
				TriangleEvalDelegate = triangleEvalFun.EvalDelegate,
				Ordering = triangleEvalFun.Ordering,
				RecountMode = triangleEvalFun.RecountMode,
				AttachedMultiplier = 1,
				DetectPlanarPolygons = true,
				MaxVarCoefOfDist = 0.1
			};

			base.Initialize(ctxt);
		}

		public override void Cleanup() {
			SmoothShading = Constant.False;
			PolygonTriangulationStrategy = new Constant(2d);
			cameraPosition = null;
			cameraUpVector = null;
			cameraTarget = null;
			base.Cleanup();
		}


		public override bool RequiresMeasure { get { return false; } }

		public override void BeginProcessing(bool measuring) {

			base.BeginProcessing(measuring);

			if (measuring) {
				writer = null;
			}
			else {
				createdMetarials.Clear();
				createdSphereGeomteries.Clear();
				lineGeometryCreated = false;

				outputStream = context.OutputProvider.GetOutputStream<ThreeJsSceneRenderer3D>(
					"3D result from `{0}`".Fmt(context.Lsystem.Name),
					MimeType.Application.Javascript, false, globalAdditionalData);

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

				createdMetarials.Clear();
				createdSphereGeomteries.Clear();
			}
		}


		public override void DrawTo(Point3D endPoint, Quaternion rotation, double width, ColorF color, double quality) {
			if (!measuring) {
				string geometryName = createLineGeometry(quality);
				string materialName = createMaterial(color);
				writer.WriteLine("mesh = new THREE.Mesh({0}, {1});".Fmt(geometryName, materialName));

				var middle = Math3D.CountMiddlePoint(endPoint, lastPoint);
				writer.WriteLine("mesh.position.x = {0:0.###};".Fmt(middle.X));
				writer.WriteLine("mesh.position.y = {0:0.###};".Fmt(middle.Y));
				writer.WriteLine("mesh.position.z = {0:0.###};".Fmt(middle.Z));

				var euclidRotation = rotation.ToEuclidRotation();
				writer.WriteLine("mesh.eulerOrder = 'YZX';");
				writer.WriteLine("mesh.rotation.x = {0:0.###};".Fmt(euclidRotation.X));
				writer.WriteLine("mesh.rotation.y = {0:0.###};".Fmt(euclidRotation.Y));
				writer.WriteLine("mesh.rotation.z = {0:0.###};".Fmt(euclidRotation.Z));

				writer.WriteLine("mesh.scale.x = {0:0.###};".Fmt(Math3D.Distance(endPoint, lastPoint)));
				writer.WriteLine("mesh.scale.y = {0:0.###};".Fmt(width));
				writer.WriteLine("mesh.scale.z = {0:0.###};".Fmt(width));

				writer.WriteLine("mesh.updateMatrix();");
				writer.WriteLine("mesh.matrixAutoUpdate = false;");
				writer.WriteLine("mesh.rotationAutoUpdate = false;");
				writer.WriteLine("objectHolder.add(mesh);");

				base.DrawTo(endPoint, rotation, width, color, quality);

				measure(endPoint);
			}
		}

		public override void DrawPolygon(Polygon3D polygon) {
			if (!measuring) {

				if (polygon.Ponits.Count < 3) {
					Logger.LogMessage(Message.InvalidPolygon, polygon.Ponits.Count);
					return;
				}

				writer.WriteLine("geom = new THREE.Geometry();");

				foreach (var pt in polygon.Ponits) {
					writer.WriteLine("geom.vertices.push(new THREE.Vertex(new THREE.Vector3({0:0.###},{1:0.###},{2:0.###})));"
						.Fmt(pt.X, pt.Y, pt.Z));
					measure(pt);
				}

				var indices = polygonTrianguler.Triangularize(polygon.Ponits, polygonTriangulerParameters);
				for (int i = 0; i < indices.Count; i += 3) {
					writer.WriteLine("geom.faces.push(new THREE.Face3({0},{1},{2}));".Fmt(indices[i], indices[i + 1], indices[i + 2]));

				}

				writer.WriteLine("geom.computeFaceNormals();");
				string materialName = createMaterial(polygon.Color);

				writer.WriteLine("mesh = new THREE.Mesh(geom, {0});".Fmt(materialName));
				writer.WriteLine("mesh.matrixAutoUpdate = false;");
				writer.WriteLine("mesh.doubleSided = true;");
				writer.WriteLine("objectHolder.add(mesh);");
			}
		}

		public override void DrawSphere(Point3D center, Quaternion rotation, double radius, ColorF color, double quality) {
			if (!measuring) {
				string geometryName = createSphereGeometry(quality);
				string materialName = createMaterial(color);
				writer.WriteLine("mesh = new THREE.Mesh({0}, {1});".Fmt(geometryName, materialName));

				writer.WriteLine("mesh.position.x = {0:0.###};".Fmt(center.X));
				writer.WriteLine("mesh.position.y = {0:0.###};".Fmt(center.Y));
				writer.WriteLine("mesh.position.z = {0:0.###};".Fmt(center.Z));

				var euclidRotation = rotation.ToEuclidRotation();
				writer.WriteLine("mesh.eulerOrder = 'YZX';");
				writer.WriteLine("mesh.rotation.x = {0:0.###};".Fmt(euclidRotation.X));
				writer.WriteLine("mesh.rotation.y = {0:0.###};".Fmt(euclidRotation.Y));
				writer.WriteLine("mesh.rotation.z = {0:0.###};".Fmt(euclidRotation.Z));

				if (radius.EpsilonCompareTo(1.0) != 0) {
					writer.WriteLine("mesh.scale.x = {0:0.###};".Fmt(radius));
					writer.WriteLine("mesh.scale.y = {0:0.###};".Fmt(radius));
					writer.WriteLine("mesh.scale.z = {0:0.###};".Fmt(radius));
				}

				writer.WriteLine("mesh.updateMatrix();");
				writer.WriteLine("mesh.matrixAutoUpdate = false;");
				writer.WriteLine("objectHolder.add(mesh);");

				var radPoint = new Point3D(radius, radius, radius);
				measure(Math3D.AddPoints(center, radPoint));
				measure(Math3D.SubtractPoints(center, radPoint));

			}
		}

		/// <summary>
		/// Here is space for improvements. For example render cylinder with variable number of edges depending on quality.
		/// </summary>
		private string createLineGeometry(double quality) {

			string name = "lineGeometry";

			if (!lineGeometryCreated) {
				writer.WriteLine("var {0} = new THREE.CubeGeometry(1, 1, 1);".Fmt(name));
				lineGeometryCreated = true;
			}

			return name;
		}

		private string createSphereGeometry(double quality) {

			int segmentsWidth = Math.Max(7, (int)Math.Round(quality * 3));
			int segmentsHeight = Math.Max(5, (int)Math.Round(quality * 2));
			string name = "spgereGeometry" + segmentsWidth.ToString() + segmentsHeight.ToString();

			if (!createdSphereGeomteries.Contains(name)) {
				writer.WriteLine("var {0} = new THREE.SphereGeometry(1, {1}, {2});".Fmt(name, segmentsWidth, segmentsHeight));
				createdSphereGeomteries.Add(name);
			}

			return name;
		}

		private string createMaterial(ColorF color) {

			string colorHexa = color.ToRgbHexString();
			string name = "material" + colorHexa;

			if (!createdMetarials.Contains(name)) {
				writer.WriteLine("var {0} = new THREE.MeshLambertMaterial({{ color: 0x{1}, shading: THREE.{2} }});"
					.Fmt(name, colorHexa, smoothShading ? "SmoothShading" : "FlatShading"));
				createdMetarials.Add(name);
			}

			return name;
		}


		private void startFile() {

			writer.WriteLine("var Scene = function() {");  // create scope to not fill global scope with mess
			writer.WriteLine("var scene = new THREE.Scene();");

			// objects
			writer.WriteLine("var objectHolder = new THREE.Object3D();");
			writer.WriteLine("scene.add(objectHolder);");

			writer.WriteLine("var mesh; var material; var geom;");  // mesh for creating objects

		}

		private void endFile() {

			Point3D camTarget = Math3D.ZeroPoint;
			if (cameraTarget != null) {
				camTarget.X = ((Constant)cameraTarget[0]).Value;
				camTarget.Y = ((Constant)cameraTarget[1]).Value;
				camTarget.Z = ((Constant)cameraTarget[2]).Value;
			}
			else {
				camTarget = Math3D.CountMiddlePoint(measuredMin, measuredMax);
			}

			Point3D camPosition = Math3D.ZeroPoint;
			if (cameraPosition != null) {
				camPosition.X = ((Constant)cameraPosition[0]).Value;
				camPosition.Y = ((Constant)cameraPosition[1]).Value;
				camPosition.Z = ((Constant)cameraPosition[2]).Value;
			}
			else {
				camPosition = Math3D.AddPoints(camTarget, measuredMax);
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


			var light1 = new Vector3D(-0.9, -1.2, -1.1);
			var light2 = new Vector3D(0, 1.2, 0.9);
			var light3 = new Vector3D(1, 1.1, 0);

			light1.Normalize();
			light2.Normalize();
			light3.Normalize();

			writer.WriteLine("var directionalLight = new THREE.DirectionalLight(0xF0F0F0);");
			writer.WriteLine("directionalLight.position.x = {0:0.###};".Fmt(light1.X));
			writer.WriteLine("directionalLight.position.y = {0:0.###};".Fmt(light1.Y));
			writer.WriteLine("directionalLight.position.z = {0:0.###};".Fmt(light1.Z));
			writer.WriteLine("scene.add(directionalLight);");

			writer.WriteLine("directionalLight = new THREE.DirectionalLight(0x999999);");
			writer.WriteLine("directionalLight.position.x = {0:0.###};".Fmt(light2.X));
			writer.WriteLine("directionalLight.position.y = {0:0.###};".Fmt(light2.Y));
			writer.WriteLine("directionalLight.position.z = {0:0.###};".Fmt(light2.Z));
			writer.WriteLine("scene.add(directionalLight);");

			writer.WriteLine("directionalLight = new THREE.DirectionalLight(0x999999);");
			writer.WriteLine("directionalLight.position.x = {0:0.###};".Fmt(light3.X));
			writer.WriteLine("directionalLight.position.y = {0:0.###};".Fmt(light3.Y));
			writer.WriteLine("directionalLight.position.z = {0:0.###};".Fmt(light3.Z));
			writer.WriteLine("scene.add(directionalLight);");


			writer.WriteLine("return {");
			writer.WriteLine("getScene: function() { return scene; },");
			writer.WriteLine("getCameraPosition: function() {{ return new THREE.Vector3( {0:0.###}, {1:0.###}, {2:0.###} ); }},"
				.Fmt(camPosition.X, camPosition.Y, camPosition.Z));
			writer.WriteLine("getCameraUpVector: function() {{ return new THREE.Vector3( {0:0.###}, {1:0.###}, {2:0.###} ); }},"
				.Fmt(camUp.X, camUp.Y, camUp.Z));
			writer.WriteLine("getCameraTarget: function() {{ return new THREE.Vector3( {0:0.###}, {1:0.###}, {2:0.###} ); }}"
				.Fmt(camTarget.X, camTarget.Y, camTarget.Z));
			writer.WriteLine("};");

			writer.WriteLine("};");  // end of scope

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
			InvalidPolygon
		}

	}
}
