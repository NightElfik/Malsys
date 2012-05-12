/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Malsys.Media.Triangulation {

	public delegate float TriangleEvaluateDelegate(int index, CyclicIndexedLinkedList<Point3D> polygon);

	/// <summary>
	/// Specifies how to order candidates for new triangle by score.
	/// </summary>
	/// <remarks>
	/// This allows to switch meaning of score from scoring function (whether more is better or worst).
	/// </remarks>
	public enum Trianguler3DScoreOrdering {
		Ascending,
		Descending,
	}

	/// <summary>
	/// Specifies which triangles candidate's score will be recount after creating new triangle.
	/// </summary>
	/// <remarks>
	/// Result of scoring functions can change only if neighbor points is changed or it is not affected by any changes.
	/// </remarks>
	public enum Trianguler3DRecountMode {
		Never,
		Neighbours,
		All,
	}

	/// <summary>
	/// 3D polygon trianguler.
	/// Triangulates polygon specified by border poly-line by cutting-ear algorithm.
	/// Triangulation strategy can be specified by scoring function.
	/// </summary>
	/// <remarks>
	/// Ear is triangle made of 3 consecutive points.
	/// Every ear is scored by given function and best ear is made into triangle and is cut.
	/// After n steps polygon is triangulated.
	///
	/// The trianguler has detection for polygons which are nearly planar (border points are in plane).
	/// It detects the plane projects 3D point to 2D plane and triangulates using robust Delaunay triangulation
	/// algorithm.
	/// </remarks>
	public class Polygon3DTrianguler {

		/// <summary>
		/// Triangulates given polygon specified by points on border with given parameters and
		/// returns list of indexes of triangles (each 3 indexes are one triangle).
		/// </summary>
		public List<int> Triangularize(IList<Point3D> points, Polygon3DTriangulerParameters prms) {

			Contract.Requires<ArgumentNullException>(points != null);
			Contract.Requires<ArgumentNullException>(prms != null);
			Contract.Requires<ArgumentException>(points.Count >= 3);
			Contract.Requires<ArgumentException>(prms.TriangleEvalDelegate != null);
			Contract.Ensures(Contract.Result<List<int>>() != null);
			Contract.Ensures(Contract.Result<List<int>>().Count % 3 == 0);

			if (prms.DetectPlanarPolygons) {
				List<int> result;
				if (tryTriangulizeAsPlanar(points, prms, out result)) {
					// triangulated as planar
					return result;
				}
			}

			var earEvalDel = prms.TriangleEvalDelegate;
			bool orderAsc = prms.Ordering == Trianguler3DScoreOrdering.Ascending;

			var polygon = new CyclicIndexedLinkedList<Point3D>(points);
			float[] earsScore = new float[points.Count];

			// evaluate ears score
			for (int i = 0; i < points.Count; i++) {
				earsScore[i] = earEvalDel(i, polygon);
			}

			List<int> triangleIndices = new List<int>(points.Count);

			//for (int i = 2; i < points.Length; i++) {
			while (true) {
				// find highest score
				int winnerIndex = polygon.EntryNode.Index;
				float winnerScore = earsScore[winnerIndex];

				var node = polygon.EntryNode.Next;

				for (int i = 1; i < polygon.Count; i++, node = node.Next) {
					int nodeIndex = node.Index;
					float nodeScore = earsScore[nodeIndex];

					if (float.IsNaN(nodeScore)) {
						continue;
					}

					if (orderAsc ? nodeScore < winnerScore : nodeScore > winnerScore) {
						winnerIndex = nodeIndex;
						winnerScore = nodeScore;
					}
				}

				// add triangle
				int previousIndex = polygon[winnerIndex].Previous.Index;
				int nextIndex = polygon[winnerIndex].Next.Index;

				triangleIndices.Add(winnerIndex);
				triangleIndices.Add(previousIndex);
				triangleIndices.Add(nextIndex);

				// remove point from polygon
				polygon[winnerIndex].Remove();

				if (polygon.Count < 3) {
					break;
				}

				// recount score
				switch (prms.RecountMode) {
					case Trianguler3DRecountMode.Never:
						break;
					case Trianguler3DRecountMode.Neighbours:
						earsScore[previousIndex] = earEvalDel(previousIndex, polygon);
						earsScore[nextIndex] = earEvalDel(nextIndex, polygon);
						break;
					case Trianguler3DRecountMode.All:
						var currNode = polygon.EntryNode;
						for (int i = 0; i < polygon.Count; i++, currNode = currNode.Next) {
							earsScore[currNode.Index] = earEvalDel(currNode.Index, polygon);
						}
						break;
				}

				earsScore[previousIndex] *= (float)prms.AttachedMultiplier;
				earsScore[nextIndex] *= (float)prms.AttachedMultiplier;
			}

			return triangleIndices;
		}


		private bool tryTriangulizeAsPlanar(IList<Point3D> points, Polygon3DTriangulerParameters prms, out List<int> tringleIndices) {

			Point3D p1, p2, p3;
			if (!tryGetPlaneFromPoints(points, out p1, out p2, out p3)) {
				tringleIndices = null;
				return false;
			}

			Vector3D normal = Vector3D.CrossProduct(p2 - p1, p3 - p1);

			var distances = points.Select(p => Vector3D.DotProduct(normal, p - p1));
			var mean = distances.Average();
			var stdDeviation = distances.Sum(d => (d - mean) * (d - mean)) / (points.Count - 1);
			var coefOfVariation = Math.Abs(stdDeviation / mean);

			if (coefOfVariation > prms.MaxVarCoefOfDist) {
				tringleIndices = null;
				return false;
			}


			tringleIndices = new List<int>();

			var planePoints = projectToPlane(p1, p2, p3, points);

			var polygon = new CyclicIndexedLinkedList<Point>(planePoints);

			// find extreme point
			int extremeIndex = 0;
			Point extremePoint = planePoints[extremeIndex];

			for (int i = 1; i < planePoints.Length; i++) {
				if (planePoints[i].X > extremePoint.X ||
					planePoints[i].Y > extremePoint.Y && Math.Abs(planePoints[i].X - extremePoint.X) < 0.0001) {
					extremeIndex = i;
					extremePoint = planePoints[extremeIndex];
				}
			}

			double refConvex = Vector.CrossProduct(
				polygon[extremeIndex].Item - polygon[extremeIndex].Previous.Item,
				polygon[extremeIndex].Next.Item - polygon[extremeIndex].Item);

			if (refConvex == 0) {
				tringleIndices = null;
				return false;
			}

			polygon.EntryNode = polygon[extremeIndex];

			for (int i = 2; i < planePoints.Length; i++) {
				// find suitable ear
				var node = polygon.EntryNode;
				int j = 0;

				for (; j < polygon.Count; j++, node = node.Next) {
					bool isConvex = Vector.CrossProduct(node.Item - node.Previous.Item, node.Next.Item - node.Item) * refConvex > 0;
					if (isConvex && isEarEmpty(node, polygon)) {
						break;
					}
				}

				if (j >= polygon.Count) {
					tringleIndices = null;
					return false;
				}

				tringleIndices.Add(node.Previous.Index);
				tringleIndices.Add(node.Index);
				tringleIndices.Add(node.Next.Index);

				node.Remove();
			}

			return true;
		}

		private bool isEarEmpty(CyclicIndexedLinkedList<Point>.Node pointNode, CyclicIndexedLinkedList<Point> polygon) {

			var p1 = pointNode.Item;
			var p2 = pointNode.Previous.Item;
			var p3 = pointNode.Next.Item;

			var node = polygon.EntryNode;

			for (int i = 0; i < polygon.Count; i++, node = node.Next) {
				if (node == pointNode || node == pointNode.Previous || node == pointNode.Next) {
					continue;
				}

				if (Math3D.IsPointInTriangle(node.Item, p1, p2, p3)) {
					return false;
				}
			}

			return true;
		}

		private bool tryGetPlaneFromPoints(IList<Point3D> points, out Point3D p1, out Point3D p2, out Point3D p3) {
			Contract.Requires<ArgumentException>(points.Count >= 3);

			if (points.Count == 3) {
				p1 = points[0];
				p2 = points[1];
				p3 = points[2];
				return true;
			}

			int maxPointI = -1, minPointI = -1;

			{
				// extremes
				double maxSum = double.NegativeInfinity;
				double minSum = double.PositiveInfinity;

				for (int i = 0; i < points.Count; i++) {

					double sum = points[i].X + points[i].Y + points[i].Z;

					if (sum > maxSum) {
						maxPointI = i;
						maxSum = sum;
					}

					if (sum < minSum) {
						minPointI = i;
						minSum = sum;
					}

				}
			}

			p1 = Math3D.ZeroPoint;
			p2 = points[maxPointI];
			p3 = points[minPointI];

			double minVectorLengthSquared = (points[maxPointI] - points[minPointI]).LengthSquared / 16;

			int randomPointI;

			{
				int maxIters = 4 + (int)Math.Log(points.Count, 2);
				// count deterministic random seed to achieve stability (same plane for same polygon)
				int seed = Math.Abs(maxIters + points.Count + (int)points[0].X + (int)points[0].Y + (int)points[0].Z);
				Random randomGenerator = new Random(maxIters);

				const double minAngle = 8;
				int i = 0;

				for (; i < maxIters; i++) {
					// get random unique index
					do { randomPointI = randomGenerator.Next(points.Count); }
					while (randomPointI == minPointI || randomPointI == maxPointI);

					p1 = points[randomPointI];

					Vector3D v1 = p2 - p1;
					Vector3D v2 = p3 - p1;

					// ensure lengths
					if (v1.LengthSquared < minVectorLengthSquared || v2.LengthSquared < minVectorLengthSquared) {
						continue;
					}

					double angle = Vector3D.AngleBetween(v1, v2);
					// ensure angle
					if (Math.Abs(angle) < minAngle) {
						continue;
					}

					break;
				}

				if (i == maxIters) {
					p1 = Math3D.ZeroPoint; p2 = Math3D.ZeroPoint; p3 = Math3D.ZeroPoint;
					return false;
				}
			}

			return true;
		}

		private Point[] projectToPlane(Point3D p1, Point3D p2, Point3D p3, IList<Point3D> points) {

			// line – parametrized by p in direction of normal vector n from point b
			// x = bx + p nx
			// y = by + p ny
			// z = bz + p nz
			//
			// plane – given by vectors u and v through point r (parametrized by t and s)
			// x = rx + t ux + s vx,
			// x = ry + t uy + s vy,
			// x = rz + t uz + s vz
			//
			// system of equations
			// bx + p nx == rx + t ux + s vx
			// by + p ny == ry + t uy + s vy
			// bz + p nz == rz + t uz + s vz
			//
			// sollution
			// p = -(-bz uy vx + rz uy vx + by uz vx - ry uz vx + bz ux vy -
			//     rz ux vy - bx uz vy + rx uz vy - by ux vz + ry ux vz +
			//     bx uy vz - rx uy vz)/(-nz uy vx + ny uz vx + nz ux vy -
			//     nx uz vy - ny ux vz + nx uy vz)
			// t = -(bz ny vx - by nz vx + nz ry vx - ny rz vx - bz nx vy +
			//     bx nz vy - nz rx vy + nx rz vy + by nx vz - bx ny vz +
			//     ny rx vz - nx ry vz)/(nz uy vx - ny uz vx - nz ux vy +
			//     nx uz vy + ny ux vz - nx uy vz)
			// s = -(bz ny ux - by nz ux + nz ry ux - ny rz ux - bz nx uy +
			//     bx nz uy - nz rx uy + nx rz uy + by nx uz - bx ny uz +
			//     ny rx uz - nx ry uz)/(-nz uy vx + ny uz vx + nz ux vy -
			//     nx uz vy - ny ux vz + nx uy vz)

			Vector3D v1 = p2 - p1;
			Vector3D v2 = p3 - p1;
			Vector3D normal = Vector3D.CrossProduct(v1, v2);

			int count = points.Count;
			Point[] result = new Point[count];

			double rx = p1.X,
				ry = p1.Y,
				rz = p1.Z;

			double nx = normal.X,
				ny = normal.Y,
				nz = normal.Z;

			double ux = v1.X,
				uy = v1.Y,
				uz = v1.Z;

			double vx = v2.X,
				vy = v2.Y,
				vz = v2.Z;


			for (int i = 0; i < count; i++) {
				double bx = points[i].X,
					by = points[i].Y,
					bz = points[i].Z;

				result[i].X = -(bz * ny * vx - by * nz * vx + nz * ry * vx - ny * rz * vx - bz * nx * vy +
					bx * nz * vy - nz * rx * vy + nx * rz * vy + by * nx * vz - bx * ny * vz +
					ny * rx * vz - nx * ry * vz) / (nz * uy * vx - ny * uz * vx - nz * ux * vy +
					nx * uz * vy + ny * ux * vz - nx * uy * vz);

				result[i].Y = -(bz * ny * ux - by * nz * ux + nz * ry * ux - ny * rz * ux - bz * nx * uy +
					bx * nz * uy - nz * rx * uy + nx * rz * uy + by * nx * uz - bx * ny * uz +
					ny * rx * uz - nx * ry * uz) / (-nz * uy * vx + ny * uz * vx + nz * ux * vy -
					nx * uz * vy - ny * ux * vz + nx * uy * vz);
			}


			return result;
		}


	}

}
