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

			/******************************************************************/

			//double e =q.W*q.W;
			//double d =q.X*q.X;
			//double c =q.Y*q.Y;
			//double b =q.Z*q.Z;

			//double Z = (Math.Atan2(2 * (q.X * q.Y + q.Z * q.W), (d - c - b + e)));
			//double X = (Math.Atan2(2 * (q.Y * q.Z + q.X * q.W), (-d - c + b + e)));
			//double Y = Math.Asin(MathHelper.Clamp(-2 * (q.X * q.Z - q.Y * q.W), -1, 1));

			//return new Vector3D(X, Y, Z);

			/******************************************************************/

			//double test = q.W * q.Y - q.X * q.Z;

			////// test singularities (gimbal lock) http://en.wikipedia.org/wiki/Gimbal_lock
			//// singularity at north pole
			////if (test > gimbalLockTreshold) {
			////    return new Vector3D(MathHelper.PiHalf, MathHelper.PiHalf, MathHelper.PiHalf);
			////}

			////// singularity at south pole
			////if (test < -gimbalLockTreshold) {
			////    return new Vector3D(-2 * Math.Atan2(q.X, q.W), -MathHelper.PiHalf, 0);
			////}

			//return new Vector3D(
			//    Math.Atan2(2 * (q.W * q.X + q.Y * q.Z), 1 - 2 * (q.X * q.X + q.Y * q.Y)),
			//    Math.Asin(2 * test),
			//    Math.Atan2(2 * (q.W * q.Z + q.X * q.Y), 1 - 2 * (q.Y * q.Y + q.Z * q.Z))
			//);

			/******************************************************************/

			//Vector3D rotationaxes = new Vector3D();

			//QuaternionRotation3D quatRotation = new QuaternionRotation3D(q);
			//RotateTransform3D tranform = new RotateTransform3D(quatRotation);

			//Vector3D forward = tranform.Transform(new Vector3D(1, 0, 0));
			//Vector3D up = tranform.Transform(new Vector3D(0, 1, 0));
			//rotationaxes = AngleTo(new Vector3D(), forward);

			//if (rotationaxes.X.EpsilonCompareTo(MathHelper.PiHalf) == 0) {
			//    rotationaxes.Y = Math.Atan2(up.Z, up.X);
			//    rotationaxes.Z = 0;
			//}
			//else if (rotationaxes.X.EpsilonCompareTo(-MathHelper.PiHalf) == 0) {
			//    rotationaxes.Y = Math.Atan2(-up.Z, -up.X);
			//    rotationaxes.Z = 0;
			//}
			//else {
			//    AxisAngleRotation3D r = new AxisAngleRotation3D();
			//    r.Axis = new Vector3D(0, 1, 0);
			//    r.Angle = -rotationaxes.Y;
			//    tranform.Rotation = r;
			//    tranform.Transform(up);

			//    r.Axis = new Vector3D(1, 0, 0);
			//    r.Angle = -rotationaxes.X;
			//    tranform.Transform(up);

			//    rotationaxes.Z = Math.Atan2(up.Y, -up.X);
			//}

			//return new Vector3D(rotationaxes.Z, rotationaxes.Y, rotationaxes.X);
		}

		//public static Vector3D AngleTo(Vector3D from, Vector3D location) {
		//    Vector3D angle = new Vector3D();
		//    Vector3D v3 = location - from;
		//    v3.Normalize();
		//    angle.X = Math.Asin(v3.Y);
		//    angle.Y = Math.Atan2(-v3.Z, -v3.X);
		//    return angle;
		//}

	}
}
