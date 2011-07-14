using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Malsys.Parser;

namespace Malsys.Tests.Parser {
	/// <summary>
	/// Thiese tests do not test anything concrete, they are just for debuging of parser.
	/// </summary>
	[TestClass]
	public class DebugTests {
		[TestMethod]
		public void DebugTest() {
			string testInput = @"
lsystem LsysName {
	let x = 3 + a * 5;
	let a = a;
}";
			var lsys = ParserUtils.ParseFromString(testInput);

		}
	}
}
