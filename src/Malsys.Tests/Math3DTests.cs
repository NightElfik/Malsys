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

	}
}
