using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests {
	[TestClass]
	public class DataSizeHelperTest {

		[TestMethod]
		public void ToOptimalUnitStringTest() {

			doTest(0, "0.00 B");
			doTest(9, "9.0 B");
			doTest(10, "10 B");
			doTest(99, "99 B");
			doTest(100, "100 B");
			doTest(999, "999 B");
			doTest(1000, "0.98 kB");

			doTest(1024, "1.0 kB");
			doTest(9 * 1024, "9.0 kB");
			doTest(10 * 1024, "10 kB");
			doTest(99 * 1024, "99 kB");
			doTest(100 * 1024, "100 kB");
			doTest(999 * 1024, "999 kB");
			doTest(1000 * 1024, "0.98 MB");

			doTest(1024 * 1024, "1.0 MB");
			doTest(9 * 1024 * 1024, "9.0 MB");
			doTest(10 * 1024 * 1024, "10 MB");
			doTest(99 * 1024 * 1024, "99 MB");
			doTest(100 * 1024 * 1024, "100 MB");
			doTest(999 * 1024 * 1024, "999 MB");
			doTest(1000 * 1024 * 1024, "0.98 GB");

		}


		private void doTest(long size, string expected) {
			string actual = DataSizeHelper.ToOptimalUnitString(size);
			Assert.AreEqual(expected, actual);
		}

	}
}
