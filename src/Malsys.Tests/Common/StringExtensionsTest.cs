/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Common {
	[TestClass]
	public class StringExtensionsTest {

		[TestMethod]
		public void SplitToLinesTest() {
			splitToLinesTestHelper("");
			splitToLinesTestHelper("", "");
			splitToLinesTestHelper("", "", "");
			splitToLinesTestHelper("", "", "", "");

			splitToLinesTestHelper("abc");
			splitToLinesTestHelper("abc", "def");

			splitToLinesTestHelper("", "abc", "def");
			splitToLinesTestHelper("", "", "abc", "def");

			splitToLinesTestHelper("abc", "def", "");
			splitToLinesTestHelper("abc", "def", "", "");

			splitToLinesTestHelper("abc", "", "def");
			splitToLinesTestHelper("abc", "", "", "def");

			splitToLinesTestHelper("abc", "", "def", "");
			splitToLinesTestHelper("", "abc", "", "def");

		}


		private void splitToLinesTestHelper(params string[] lines) {
			splitToLinesTestHelper("\n", lines);  // UNIX
			splitToLinesTestHelper("\r\n", lines);  // Windows
			splitToLinesTestHelper("\r", lines);  // MAC
		}

		private void splitToLinesTestHelper(string newLineSeparator, string[] lines) {
			var actual = string.Join(newLineSeparator, lines).SplitToLines().ToArray();

			if (!lines.SequenceEqual(actual)) {
			    Console.WriteLine(string.Join(" | ", lines) + " != " + string.Join(" | ", actual));
			    Assert.Fail();
			}
		}
	}
}
