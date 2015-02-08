using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Windows.Media.Media3D;

namespace Malsys.Media {
	public class SubdividableSphere {

		// Length of vector (1, 1, 1) = 1 / sqrt(3).
		private const double oneOverSqrt3 = 0.57735026918962576450914878050196;

		public static readonly Point3D[] TetrahedronVertices = new Point3D[] {
			new Point3D(oneOverSqrt3, oneOverSqrt3, oneOverSqrt3),
			new Point3D(-oneOverSqrt3, -oneOverSqrt3, oneOverSqrt3),
			new Point3D(-oneOverSqrt3, oneOverSqrt3, -oneOverSqrt3),
			new Point3D(oneOverSqrt3, -oneOverSqrt3, -oneOverSqrt3),
		};

		public static readonly Point3Di[] TetrahedronIndices = new Point3Di[] {
			new Point3Di(0, 1, 2),
			new Point3Di(0, 3, 1),
			new Point3Di(0, 2, 3),
			new Point3Di(0, 2, 3),
		};


		// Golden ratio = (1.0 + sqrt(5.0)) / 2.0 = 1.6180339887498948482045868343656.
		// Length of vector (0, gr, 1) = (1.0 / sqrt(gr * gr + 1.0)).
		private const double oneNorm = 0.52573111211913360602566908484789;
		// Normalized golden ration = gr * norm.
		private const double grNorm = 0.85065080835203993218154049706301;

		public static readonly Point3D[] IcosahedronVertices = new Point3D[] {
			// x = 0, yz plane
			new Point3D(0, grNorm, oneNorm),
			new Point3D(0, grNorm,-oneNorm),
			new Point3D(0, -grNorm, oneNorm),
			new Point3D(0, -grNorm,-oneNorm),

			// y = 0, xz plane
			new Point3D(oneNorm, 0, grNorm),
			new Point3D(-oneNorm, 0, grNorm),
			new Point3D(oneNorm, 0, -grNorm),
			new Point3D(-oneNorm, 0, -grNorm),

			// z = 0, xy plane
			new Point3D(grNorm, oneNorm, 0),
			new Point3D(grNorm,-oneNorm, 0),
			new Point3D(-grNorm, oneNorm, 0),
			new Point3D(-grNorm,-oneNorm, 0),
		};

		public static readonly Point3Di[] IcosahedronIndices = new Point3Di[] {
			new Point3Di(0, 1, 8),
			new Point3Di(0, 10, 1),
			new Point3Di(2, 9, 3),
			new Point3Di(2, 3, 11),

			new Point3Di(4, 5, 0),
			new Point3Di(4, 2, 5),
			new Point3Di(6, 1, 7),
			new Point3Di(6, 7, 3),

			new Point3Di(8, 9, 4),
			new Point3Di(8, 6, 9),
			new Point3Di(10, 5, 11),
			new Point3Di(10, 11, 7),

			new Point3Di(1, 6, 8),
			new Point3Di(0, 8, 4),

			new Point3Di(1, 10, 7),
			new Point3Di(0, 5, 10),

			new Point3Di(3, 9, 6),
			new Point3Di(2, 4, 9),

			new Point3Di(3, 7, 11),
			new Point3Di(2, 11, 5),

		};


		public List<Point3D> Vertices { get; set; }
		public List<Point3Di> Indices { get; set; }


		/// <summary>
		/// Initializes new subdividable sphere as either tetrahedron or icosahedron based on given parameter.
		/// </summary>
		/// <param name="icosahedron"></param>
		public SubdividableSphere(bool icosahedron) {
			Vertices = new List<Point3D>(icosahedron ? IcosahedronVertices : TetrahedronVertices);
			Indices = new List<Point3Di>(icosahedron ? IcosahedronIndices : TetrahedronIndices);
		}


		/// <summary>
		/// Subdivides this sphere.
		/// </summary>
		/// <remarks>
		/// Note that the indices array gets replaced.
		/// </remarks>
		public void Subdivide() {
			Contract.Requires(Vertices != null && Vertices.Count > 0);
			Contract.Requires(Indices != null && Indices.Count > 0);

			List<Point3Di> newIndices = new List<Point3Di>();
			Dictionary<Tuple<int, int>, int> newVertIndicesMap = new Dictionary<Tuple<int, int>, int>();

			foreach (var triIndices in Indices) {
				int i1 = triIndices.X;
				int i2 = triIndices.Y;
				int i3 = triIndices.Z;

				int i12 = getMidpointIndex(i1, i2, newVertIndicesMap);
				int i23 = getMidpointIndex(i2, i3, newVertIndicesMap);
				int i13 = getMidpointIndex(i1, i3, newVertIndicesMap);

				newIndices.Add(new Point3Di(i1, i12, i13));
				newIndices.Add(new Point3Di(i2, i23, i12));
				newIndices.Add(new Point3Di(i3, i13, i23));
				newIndices.Add(new Point3Di(i12, i23, i13));
			}

			Indices = newIndices;
		}


		private int getMidpointIndex(int i1, int i2, Dictionary<Tuple<int, int>, int> indexPairs) {

			if (i1 > i2) {
				int oldI1 = i1;
				i1 = i2;
				i2 = oldI1;
			}

			Debug.Assert(i1 < i2);
			var key = new Tuple<int, int>(i1, i2);

			int existingIndex;
			if (indexPairs.TryGetValue(key, out existingIndex)) {
				return existingIndex;
			}

			Point3D v1 = Vertices[i1];
			Point3D v2 = Vertices[i2];

			int index = Vertices.Count;
			// WTF M$oft ... again ... can't do just v1 + v2 because "summing" of points is not cool... gj...
			Vector3D mid = (Vector3D)v1 + (Vector3D)v2;
			mid.Normalize();

			Vertices.Add((Point3D)mid);
			indexPairs.Add(key, index);
			return index;
		}

	}
}
