/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using System.Windows.Media.Media3D;
using Malsys.Media;
using Malsys.Media.Triangulation;

namespace Malsys.Resources {
	public static class TriangleEvaluationFunctions {

		/// <summary>
		/// Returns minimal angle from triangle as score.
		/// </summary>
		public static TriangleEvaluationFunction MinAngle = new TriangleEvaluationFunction(Trianguler3DScoreOrdering.Descending, Trianguler3DRecountMode.Neighbours,
			(index, polygon) => {
				var angles = Math3D.GetTriangleAngles(polygon[index].Item, polygon[index].Previous.Item, polygon[index].Next.Item);
				return (float)Math.Min(angles.Item1, Math.Min(angles.Item2, angles.Item3));
			});

		/// <summary>
		/// Returns maximal angle from triangle as score.
		/// </summary>
		public static TriangleEvaluationFunction MaxAngle = new TriangleEvaluationFunction(Trianguler3DScoreOrdering.Ascending, Trianguler3DRecountMode.Neighbours,
			(index, polygon) => {
				var angles = Math3D.GetTriangleAngles(polygon[index].Item, polygon[index].Previous.Item, polygon[index].Next.Item);
				return (float)Math.Max(angles.Item1, Math.Max(angles.Item2, angles.Item3));
			});

		/// <summary>
		/// Returns sum of distances from center of triangle to all other points as score.
		/// </summary>
		public static TriangleEvaluationFunction MaxDistanceAll = new TriangleEvaluationFunction(Trianguler3DScoreOrdering.Descending, Trianguler3DRecountMode.Never,
			(index, polygon) => {
				int i1 = index;
				int i2 = polygon[index].Previous.Index;
				int i3 = polygon[index].Next.Index;

				Vector3D center = ((Vector3D)polygon[i1].Item + (Vector3D)polygon[i2].Item + (Vector3D)polygon[i3].Item) / 3;

				double distance = 0;

				for (int i = 0; i < polygon.AllNodesCount; i++) {
					if (i == i1 || i == i2 || i == i3) {
						continue;
					}

					distance += ((Vector3D)polygon.GetData(i) - center).LengthSquared;
				}

				return (float)Math.Log(distance);
			});

		/// <summary>
		/// Returns sum of distances from center of triangle to all non-triangulated points as score.
		/// </summary>
		public static TriangleEvaluationFunction MaxDistanceRemaining = new TriangleEvaluationFunction(Trianguler3DScoreOrdering.Descending, Trianguler3DRecountMode.All,
			(index, polygon) => {
				int i1 = index;
				int i2 = polygon[index].Previous.Index;
				int i3 = polygon[index].Next.Index;

				Vector3D center = ((Vector3D)polygon[i1].Item + (Vector3D)polygon[i2].Item + (Vector3D)polygon[i3].Item) / 3;

				double distance = 0;
				var node = polygon.EntryNode;

				for (int i = 0; i < polygon.Count; i++, node = node.Next) {
					int nodeI = node.Index;
					if (nodeI == i1 || nodeI == i2 || nodeI == i3) {
						continue;
					}

					distance += ((Vector3D)polygon.GetData(nodeI) - center).LengthSquared;
				}

				return (float)Math.Log(distance);
			});

		/// <summary>
		/// Returns index as score.
		/// </summary>
		public static TriangleEvaluationFunction AsInInputAsc = new TriangleEvaluationFunction(Trianguler3DScoreOrdering.Descending, Trianguler3DRecountMode.Never,
			(index, polygon) => {
				var angles = Math3D.GetTriangleAngles(polygon[index].Item, polygon[index].Previous.Item, polygon[index].Next.Item);
				return (float)Math.Max(angles.Item1, Math.Max(angles.Item2, angles.Item3));
			});


	}
}
