using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Malsys.Media {
	/// <remarks>
	/// I really don't know why guys from M$soft do not implement + or - on Point3D.
	/// I know that I should adding vectors instead of points, but sometimes it is needed.
	/// Point3D and Vector3D is nice example for type synonyms and that's a pity c# does not allow them!.
	/// The actual type carries only semantic meaning, but operations should be the same on both
	/// to avoid casting on to another.
	/// </remarks>
	public static class Math3D {

		public static readonly Vector3D ZeroVector = new Vector3D(0, 0, 0);
		public static readonly Point3D ZeroPoint = new Point3D(0, 0, 0);

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
			return new Point3D((pt1.X + pt2.X) / 2.0, (pt1.Y + pt2.Y) / 2.0, (pt1.Z + pt2.Z) / 2.0);
		}

		public static double Distance(Point3D pt1, Point3D pt2) {

			double dx = pt2.X - pt1.X;
			double dy = pt2.Y - pt1.Y;
			double dz = pt2.Z - pt1.Z;

			return Math.Sqrt(dx * dx + dy * dy + dz * dz);
		}


		public static Point3D Normalize(this Point3D pt) {
			// WTF M$oft! Normalization of points is not cool huh?
			Vector3D v = (Vector3D)pt;
			v.Normalize();
			return (Point3D)pt;
		}

		public static bool IsAlmostEqualTo(this Vector3D vector, Vector3D another) {
			return vector.X.IsAlmostEqualTo(another.X)
				&& vector.Y.IsAlmostEqualTo(another.Y)
				&& vector.Z.IsAlmostEqualTo(another.Z);
		}

		public static bool IsAlmostEqualTo(this Point3D pt, Point3D another) {
			return pt.X.IsAlmostEqualTo(another.X)
				&& pt.Y.IsAlmostEqualTo(another.Y)
				&& pt.Z.IsAlmostEqualTo(another.Z);
		}

		public static bool IsAlmostEqualTo(this Point pt, Point another) {
			return pt.X.IsAlmostEqualTo(another.X)
				&& pt.Y.IsAlmostEqualTo(another.Y);
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

		public static Vector3D GetTriangleNormal(Point3D p1, Point3D p2, Point3D p3) {

			var v12 = p2 - p1;
			var v13 = p3 - p1;

			const double minLengthSq = 0.001;

			if (v12.LengthSquared < minLengthSq || v13.LengthSquared < minLengthSq) {
				return ZeroVector;
			}

			var normal = Vector3D.CrossProduct(v12, v13);
			normal.Normalize();
			return normal;
		}

		public static Tuple<double, double, double> GetTriangleAngles(Point3D p1, Point3D p2, Point3D p3) {

			var v12 = p2 - p1;
			var v23 = p3 - p2;
			var v13 = p3 - p1;

			const double minLengthSq = 0.001;

			if (v12.LengthSquared < minLengthSq || v23.LengthSquared < minLengthSq || v13.LengthSquared < minLengthSq) {
				return new Tuple<double, double, double>(double.NaN, double.NaN, double.NaN);
			}

			v12.Normalize();
			v23.Normalize();
			v13.Normalize();

			double angle1 = Math.Abs(Math.Acos(Vector3D.DotProduct(v12, v13).Clamp(-1, 1)));
			double angle2 = Math.PI - Math.Abs(Math.Acos(Vector3D.DotProduct(v12, v23).Clamp(-1, 1)));
			double angle3 = Math.Abs(Math.Acos(Vector3D.DotProduct(v23, v13).Clamp(-1, 1)));

			Debug.Assert(Math.Abs(angle1 + angle2 + angle3 - Math.PI) < 0.001, "Sum of angles of triangle is not 180°.");

			return new Tuple<double, double, double>(angle1, angle2, angle3);

		}

		public static bool IsAtSameSideOfLine(Point p1, Point p2, Point a, Point b) {
			double cp1 = Vector.CrossProduct(b - a, p1 - a);
			double cp2 = Vector.CrossProduct(b - a, p2 - a);
			return cp1 * cp2 >= 0;
		}

		public static bool IsPointInTriangle(Point p, Point a, Point b, Point c) {
			return IsAtSameSideOfLine(p, a, b, c)
				&& IsAtSameSideOfLine(p, b, a, c)
				&& IsAtSameSideOfLine(p, c, a, b);
		}


	}
}
