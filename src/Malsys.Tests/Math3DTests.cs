using System.Windows;
using System.Windows.Media.Media3D;
using Malsys.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests {
	[TestClass]
	public class Math3DTests {

		[TestMethod]
		public void QuaternionToEuclidRotationTest() {

			doQuatToEuclidTest(new Vector3D(0, 0, 0), Quaternion.Identity);
			doQuatToEuclidTest(new Vector3D(0, 0, 0), new Quaternion(new Vector3D(1, 2, 3), 0));

			doQuatToEuclidTest(new Vector3D(90, 0, 0), new Quaternion(new Vector3D(1, 0, 0), 90));
			doQuatToEuclidTest(new Vector3D(0, 90 ,0), new Quaternion(new Vector3D(0, 1, 0), 90));
			doQuatToEuclidTest(new Vector3D(0, 0, 90), new Quaternion(new Vector3D(0, 0, 1), 90));

			// TODO
		}


		private void doQuatToEuclidTest(Vector3D euclidInDegrees, Quaternion quaternion) {

			Vector3D excpected = euclidInDegrees * MathHelper.PiOver180;
			Vector3D actual = quaternion.ToEuclidRotation();
			Assert.IsTrue(Math3D.IsEpsilonEqualTo(excpected, actual));

		}


		[TestMethod]
		public void IsPointInTriangleTest() {
			{
				Point p1 = new Point(-1, 0);
				Point p2 = new Point(1, 1);
				Point p3 = new Point(1, -1);
				isPointInTriangle(true, new Point(0, 0), p1, p2, p3);
				isPointInTriangle(false, new Point(-1, 1), p1, p2, p3);
				isPointInTriangle(false, new Point(-1, -1), p1, p2, p3);
				isPointInTriangle(false, new Point(0, 2), p1, p2, p3);
			}

			{
				Point p1 = new Point(100, 100);
				Point p2 = new Point(110, 100);
				Point p3 = new Point(110, 110);
				isPointInTriangle(true, new Point(105, 103), p1, p2, p3);
				isPointInTriangle(false, new Point(105, 107), p1, p2, p3);
				isPointInTriangle(false, new Point(105, 99), p1, p2, p3);
				isPointInTriangle(false, new Point(111, 101), p1, p2, p3);
			}
		}

		private void isPointInTriangle(bool expected, Point p, Point a, Point b, Point c) {
			bool actual = Math3D.IsPointInTriangle(p, a, b, c);
			Assert.AreEqual(expected, actual);
		}

	}
}
