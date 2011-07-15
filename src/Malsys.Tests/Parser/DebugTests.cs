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
	< lctx(t) < A(_,n,a,m,e) > r(c)t(x,t) > ?{let a = 10; a*a^b-c} :{let b = x; n} -> r(a*b,c*d+4)e(f)PLAc;
}";
			var lsys = ParserUtils.ParseFromString(testInput);

		}
	}
}
