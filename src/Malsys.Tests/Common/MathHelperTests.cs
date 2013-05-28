using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Common {
	[TestClass]
	public class MathHelperTests {

		[TestMethod]
		public void RoundToShortestStringZeroTest() {
			for (int scientific = 0; scientific < 2; ++scientific) {
				bool sn = scientific == 0 ? false : true;

				for (int sd = 1; sd < 4; ++sd) {
					roundToShortestStringTestHelper(0.0, "0", sd, sn);
					roundToShortestStringTestHelper(1e-10, "0", sd, sn);
					roundToShortestStringTestHelper(-1e-10, "0", sd, sn);
				}
			}
		}

		[TestMethod]
		public void RoundToShortestStringBasicTest() {
			for (int scientific = 0; scientific < 2; ++scientific) {
				bool sn = scientific == 0 ? false : true;

				roundToShortestStringTestHelper(0.44444, "0.4", 1, sn);
				roundToShortestStringTestHelper(0.44444, "0.44", 2, sn);
				roundToShortestStringTestHelper(0.44444, "0.444", 3, sn);
				roundToShortestStringTestHelper(0.44444, "0.4444", 4, sn);

				roundToShortestStringTestHelper(0.66666, "0.7", 1, sn);
				roundToShortestStringTestHelper(0.66666, "0.67", 2, sn);
				roundToShortestStringTestHelper(0.66666, "0.667", 3, sn);
				roundToShortestStringTestHelper(0.66666, "0.6667", 4, sn);

				roundToShortestStringTestHelper(-0.44444, "-0.4", 1, sn);
				roundToShortestStringTestHelper(-0.44444, "-0.44", 2, sn);
				roundToShortestStringTestHelper(-0.44444, "-0.444", 3, sn);
				roundToShortestStringTestHelper(-0.44444, "-0.4444", 4, sn);

				roundToShortestStringTestHelper(-0.66666, "-0.7", 1, sn);
				roundToShortestStringTestHelper(-0.66666, "-0.67", 2, sn);
				roundToShortestStringTestHelper(-0.66666, "-0.667", 3, sn);
				roundToShortestStringTestHelper(-0.66666, "-0.6667", 4, sn);
			}
		}

		[TestMethod]
		public void RoundToShortestStringPrecisionPreservingTest() {
			for (int scientific = 0; scientific < 2; ++scientific) {
				bool sn = scientific == 0 ? false : true;

				roundToShortestStringTestHelper(444.44, "444", 1, sn);
				roundToShortestStringTestHelper(444.44, "444", 2, sn);
				roundToShortestStringTestHelper(444.44, "444", 3, sn);
				roundToShortestStringTestHelper(444.44, "444.4", 4, sn);

				roundToShortestStringTestHelper(666.66, "667", 1, sn);
				roundToShortestStringTestHelper(666.66, "667", 2, sn);
				roundToShortestStringTestHelper(666.66, "667", 3, sn);
				roundToShortestStringTestHelper(666.66, "666.7", 4, sn);

				roundToShortestStringTestHelper(-444.44, "-444", 1, sn);
				roundToShortestStringTestHelper(-444.44, "-444", 2, sn);
				roundToShortestStringTestHelper(-444.44, "-444", 3, sn);
				roundToShortestStringTestHelper(-444.44, "-444.4", 4, sn);

				roundToShortestStringTestHelper(-666.66, "-667", 1, sn);
				roundToShortestStringTestHelper(-666.66, "-667", 2, sn);
				roundToShortestStringTestHelper(-666.66, "-667", 3, sn);
				roundToShortestStringTestHelper(-666.66, "-666.7", 4, sn);
			}
		}

		[TestMethod]
		public void RoundToShortestStringSciNotationTest() {
			roundToShortestStringTestHelper(0.0004444, "0.0004", 1, false);
			roundToShortestStringTestHelper(0.0004444, "4E-4", 1, true);
			roundToShortestStringTestHelper(0.0004444, "0.00044", 2, false);
			roundToShortestStringTestHelper(0.0004444, "44E-5", 2, true);
			roundToShortestStringTestHelper(0.0004444, "0.000444", 3, false);
			roundToShortestStringTestHelper(0.0004444, "444E-6", 3, true);

			roundToShortestStringTestHelper(0.0006666, "0.0007", 1, false);
			roundToShortestStringTestHelper(0.0006666, "7E-4", 1, true);
			roundToShortestStringTestHelper(0.0006666, "0.00067", 2, false);
			roundToShortestStringTestHelper(0.0006666, "67E-5", 2, true);
			roundToShortestStringTestHelper(0.0006666, "0.000667", 3, false);
			roundToShortestStringTestHelper(0.0006666, "667E-6", 3, true);

			roundToShortestStringTestHelper(444444444, "444444444", 1, false);
			roundToShortestStringTestHelper(444444444, "4E8", 1, true);
			roundToShortestStringTestHelper(444444444, "444444444", 2, false);
			roundToShortestStringTestHelper(444444444, "44E7", 2, true);
			roundToShortestStringTestHelper(444444444, "444444444", 3, false);
			roundToShortestStringTestHelper(444444444, "444E6", 3, true);

			roundToShortestStringTestHelper(666666666, "666666666", 1, false);
			roundToShortestStringTestHelper(666666666, "7E8", 1, true);
			roundToShortestStringTestHelper(666666666, "666666666", 2, false);
			roundToShortestStringTestHelper(666666666, "67E7", 2, true);
			roundToShortestStringTestHelper(666666666, "666666666", 3, false);
			roundToShortestStringTestHelper(666666666, "667E6", 3, true);
		}

		[TestMethod]
		public void RoundToShortestStringSciNotationNegTest() {
			roundToShortestStringTestHelper(-0.0004444, "-0.0004", 1, false);
			roundToShortestStringTestHelper(-0.0004444, "-4E-4", 1, true);
			roundToShortestStringTestHelper(-0.0004444, "-0.00044", 2, false);
			roundToShortestStringTestHelper(-0.0004444, "-44E-5", 2, true);
			roundToShortestStringTestHelper(-0.0004444, "-0.000444", 3, false);
			roundToShortestStringTestHelper(-0.0004444, "-444E-6", 3, true);

			roundToShortestStringTestHelper(-0.0006666, "-0.0007", 1, false);
			roundToShortestStringTestHelper(-0.0006666, "-7E-4", 1, true);
			roundToShortestStringTestHelper(-0.0006666, "-0.00067", 2, false);
			roundToShortestStringTestHelper(-0.0006666, "-67E-5", 2, true);
			roundToShortestStringTestHelper(-0.0006666, "-0.000667", 3, false);
			roundToShortestStringTestHelper(-0.0006666, "-667E-6", 3, true);

			roundToShortestStringTestHelper(-444444444, "-444444444", 1, false);
			roundToShortestStringTestHelper(-444444444, "-4E8", 1, true);
			roundToShortestStringTestHelper(-444444444, "-444444444", 2, false);
			roundToShortestStringTestHelper(-444444444, "-44E7", 2, true);
			roundToShortestStringTestHelper(-444444444, "-444444444", 3, false);
			roundToShortestStringTestHelper(-444444444, "-444E6", 3, true);

			roundToShortestStringTestHelper(-666666666, "-666666666", 1, false);
			roundToShortestStringTestHelper(-666666666, "-7E8", 1, true);
			roundToShortestStringTestHelper(-666666666, "-666666666", 2, false);
			roundToShortestStringTestHelper(-666666666, "-67E7", 2, true);
			roundToShortestStringTestHelper(-666666666, "-666666666", 3, false);
			roundToShortestStringTestHelper(-666666666, "-667E6", 3, true);
		}

		[TestMethod]
		public void RoundToShortestStringSciNotationCornerCaseTest() {
			roundToShortestStringTestHelper(0.4444, "0.4", 1, false);
			roundToShortestStringTestHelper(0.4444, "0.4", 1, true);

			roundToShortestStringTestHelper(0.04444, "0.04", 1, false);
			roundToShortestStringTestHelper(0.04444, "0.04", 1, true);

			roundToShortestStringTestHelper(0.004444, "0.004", 1, false);
			roundToShortestStringTestHelper(0.004444, "4E-3", 1, true);

			roundToShortestStringTestHelper(444, "444", 1, false);
			roundToShortestStringTestHelper(444, "444", 1, true);

			roundToShortestStringTestHelper(4444, "4444", 1, false);
			roundToShortestStringTestHelper(4444, "4E3", 1, true);
		}

		[TestMethod]
		public void RoundToShortestStringSciNotationCornerCaseNegTest() {
			roundToShortestStringTestHelper(-0.4444, "-0.4", 1, false);
			roundToShortestStringTestHelper(-0.4444, "-0.4", 1, true);

			roundToShortestStringTestHelper(-0.04444, "-0.04", 1, false);
			roundToShortestStringTestHelper(-0.04444, "-0.04", 1, true);

			roundToShortestStringTestHelper(-0.004444, "-0.004", 1, false);
			roundToShortestStringTestHelper(-0.004444, "-4E-3", 1, true);

			roundToShortestStringTestHelper(-444, "-444", 1, false);
			roundToShortestStringTestHelper(-444, "-444", 1, true);

			roundToShortestStringTestHelper(-4444, "-4444", 1, false);
			roundToShortestStringTestHelper(-4444, "-4E3", 1, true);
		}

		[TestMethod]
		public void RoundToShortestStringSciNotationZeroEatingTest() {
			roundToShortestStringTestHelper(20001, "20001", 1, false);
			roundToShortestStringTestHelper(20001, "2E4", 1, true);

			roundToShortestStringTestHelper(20001, "20001", 2, false);
			roundToShortestStringTestHelper(20001, "2E4", 2, true);

			roundToShortestStringTestHelper(20001, "20001", 3, false);
			roundToShortestStringTestHelper(20001, "2E4", 3, true);

			roundToShortestStringTestHelper(20001, "20001", 4, false);
			roundToShortestStringTestHelper(20001, "2E4", 4, true);
		}


		private void roundToShortestStringTestHelper(double number, string expectedResult, int significantDigits, bool allowScientific) {

			string actual = number.RoundToShortestString(significantDigits, allowScientific);
			Assert.AreEqual(expectedResult, actual);

		}
	}
}
