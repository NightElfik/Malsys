using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows;

namespace Malsys.Media {
	/// <remarks>
	/// I really don't know why guys from M$soft do not implement + or - on Point3D.
	/// I know that I should adding vectors instead of points, but sometimes
	/// it is needed. Point3D and Vector3D is nice example for type synonyms
	/// and that's a pity c# does not allow them!.
	/// The actual type carries only semantic meaning, but operations should
	/// be the same on both (to avoid casting on to another).
	/// </remarks>
	public static class Math3D {

		public static readonly Vector3D ZeroVector = new Vector3D(0, 0, 0);
		public static readonly Vector3D UnitX = new Vector3D(1, 0, 0);
		public static readonly Vector3D UnitY = new Vector3D(0, 1, 0);
		public static readonly Vector3D UnitZ = new Vector3D(0, 0, 1);


		/// <summary>
		/// Adds two Point3D.
		/// </summary>
		public static Point3D AddPoints(Point3D pt1, Point3D pt2) {

			pt1.X += pt2.X;
			pt1.Y += pt2.Y;
			pt1.Z += pt2.Z;

			return pt1;
		}

		/// <summary>
		/// Subtracts two Point3D.
		/// </summary>
		public static Point3D SubtractPoints(Point3D pt1, Point3D pt2) {

			pt1.X -= pt2.X;
			pt1.Y -= pt2.Y;
			pt1.Z -= pt2.Z;

			return pt1;
		}

		public static Point3D CountMiddlePoint(Point3D pt1, Point3D pt2) {

			pt1.X += pt2.X;
			pt1.Y += pt2.Y;
			pt1.Z += pt2.Z;

			pt1.X /= 2;
			pt1.Y /= 2;
			pt1.Z /= 2;

			return pt1;
		}

		public static double Distance(Point3D pt1, Point3D pt2) {

			double dx = pt2.X - pt1.X;
			double dy = pt2.Y - pt1.Y;
			double dz = pt2.Z - pt1.Z;

			return Math.Sqrt(dx * dx + dy * dy + dz * dz);
		}

		public static bool IsEpsilonEqualTo(this Vector3D vector, Vector3D another) {
			return vector.X.EpsilonCompareTo(another.X) == 0
				&& vector.Y.EpsilonCompareTo(another.Y) == 0
				&& vector.Z.EpsilonCompareTo(another.Z) == 0;
		}

		public static bool IsEpsilonEqualTo(this Point3D vector, Point3D another) {
			return vector.X.EpsilonCompareTo(another.X) == 0
				&& vector.Y.EpsilonCompareTo(another.Y) == 0
				&& vector.Z.EpsilonCompareTo(another.Z) == 0;
		}

		/// <summary>
		/// Counts vector of Euler rotation from given quaternion.
		/// Works even if given quaternion is not normalized.
		/// </summary>
		/// <remarks>
		/// The cutoff point for singularities is set to 0.4995 which corresponds to 87.4 degrees.
		/// http://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles
		/// http://www.euclideanspace.com/maths/geometry/rotations/conversions/quaternionToEuler/
		/// </remarks>
		public static Vector3D ToEuclidRotation(this Quaternion q) {

			if (!q.IsNormalized) {
				q.Normalize();
			}

			double test = q.X * q.Y + q.Z * q.W;
			// Math.Asin(2 * 0.4995) = 87.4 degrees
			const double gimbalLockTreshold = 0.4995;
			// singularity at north pole
			if (test > gimbalLockTreshold) {
				return new Vector3D(0, 2 * Math.Atan2(q.X, q.W), MathHelper.PiHalf);
			}
			// singularity at south pole
			if (test < -gimbalLockTreshold) {
				return new Vector3D(0, -2 * Math.Atan2(q.X, q.W), -MathHelper.PiHalf);
			}

			double sqx = q.X * q.X;
			double sqy = q.Y * q.Y;
			double sqz = q.Z * q.Z;

			double roll = Math.Atan2(2 * q.X * q.W - 2 * q.Y * q.Z, 1 - 2 * sqx - 2 * sqz);
			double yaw = Math.Atan2(2 * q.Y * q.W - 2 * q.X * q.Z, 1 - 2 * sqy - 2 * sqz);
			double pitch = Math.Asin(2 * test);
			return new Vector3D(roll, yaw, pitch);
		}


	}
}
