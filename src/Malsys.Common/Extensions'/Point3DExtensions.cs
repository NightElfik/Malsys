using System.Windows;
using System.Windows.Media.Media3D;

namespace Malsys {
	public static class Point3DExtensions {


		public static Point ToPoint2D(this Point3D point) {
			return new Point(point.X, point.Y);
		}


	}
}
