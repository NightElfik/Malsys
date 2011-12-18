using Malsys.Processing.Components.RewriterIterators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Rewriters {
	[TestClass]
	public class MemoryBufferedIteratorTests {

		[TestMethod]
		public void EmptyInputTests() {
			GenericRewriterIteratorTests.EmptyInputTests(new MemoryBufferedIterator());
		}

		[TestMethod]
		public void ManyItersTests() {
			GenericRewriterIteratorTests.ManyItersTests(new MemoryBufferedIterator());
		}

		[TestMethod]
		public void FibTests() {
			GenericRewriterIteratorTests.FibTests(new MemoryBufferedIterator());
		}

	}
}
