using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Malsys.Rewriters;

namespace Malsys.Tests.Rewriters {
	[TestClass]
	public class ArrayRewriterTests {

		[TestMethod]
		public void GenericTests() {
			GenericRewriterTests.RunAllTests(new ArrayRewriter());
		}
	}
}
