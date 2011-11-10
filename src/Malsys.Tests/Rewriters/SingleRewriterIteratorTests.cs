using Malsys.Processing.Components.RewriterIterators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Rewriters {
	[TestClass]
	public class SingleRewriterIteratorTests {

		[TestMethod]
		public void EmptyInputTests() {
			GenericRewriterIteratorTests.EmptyInputTests(new SingleRewriterIterator());
		}

		[TestMethod]
		public void ManyItersTests() {
			GenericRewriterIteratorTests.ManyItersTests(new SingleRewriterIterator());
		}

		[TestMethod]
		public void FibTests() {
			GenericRewriterIteratorTests.FibTests(new SingleRewriterIterator());
		}

	}
}
