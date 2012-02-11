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
		/// <summary>
		/// Adds two Point3D to Point3D.
		/// </summary>
		public static Point3D AddPoints(Point3D pt1, Point3D pt2) {

			pt1.X += pt2.X;
			pt1.Y += pt2.Y;
			pt1.Z += pt2.Z;

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

		public static Vector3D CountRotation(Vector3D forward, double forwardAxisRotation) {

			Vector3D result = new Vector3D();

			result.X = forwardAxisRotation * MathHelper.PiOver180;
			// dot product of (0,1) and (fwd.x, fwd.z)
			double dotY = forward.X * forward.X + forward.Z * forward.Z;
			if (dotY != 0) {
				result.Y = Math.Acos(forward.Z / Math.Sqrt(forward.X * forward.X + forward.Z * forward.Z));
			}
			else {
				result.Y = 0;
			}
			// dot product of (1,0) and (fwd.x, fwd.y)
			double dotZ = forward.X * forward.X + forward.Y * forward.Y;
			if (dotZ != 0) {
				result.Z = Math.Acos(forward.X / Math.Sqrt(forward.X * forward.X + forward.Y * forward.Y));
			}
			else {
				result.Z = 0;
			}

			return result;
		}


	}
}
