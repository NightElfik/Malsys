// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using Malsys.Parsing;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Text.Lexing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Expressions {
	/// <summary>
	/// It is hard to test parsing, compiling and evaluating of expressions separately, but it is very easy to test together.
	/// For this general tests is this class.
	/// </summary>
	[TestClass]
	public class GeneralExpressionsTests {

		[TestMethod]
		public void ConstantsTest() {
			const string tn = "Constants test";

			evalAndCompareD(tn, "007", 7.0);
			evalAndCompareD(tn, "1", 1.0);
			evalAndCompareD(tn, "3.14159", 3.14159);
			evalAndCompareD(tn, "0.314159e1", 3.14159);
			evalAndCompareD(tn, "314159e-5", 3.14159);

			evalAndCompareD(tn, "0b01101", 13.0);
			evalAndCompareD(tn, "0B1111111111", 1023.0);
			evalAndCompareD(tn, "0o64", 52.0);
			evalAndCompareD(tn, "0O1234567", 342391.0);
			evalAndCompareD(tn, "0x6F", 111.0);
			evalAndCompareD(tn, "0XAbCdEf", 11259375.0);

			evalAndCompare(tn, "{}", ValuesArray.Empty);
			evalAndCompare(tn, "{{},{{}}}", new ValuesArray(new IValue[] { ValuesArray.Empty, new ValuesArray(new IValue[] { ValuesArray.Empty }) }));

			evalAndCompareD(tn, "nan", double.NaN);
			evalAndCompareD(tn, "true", 1.0);
			evalAndCompareD(tn, "false", 0.0);

		}

		[TestMethod]
		public void UnaryOperatorsTest() {
			const string tn = "Unary operators test";

			evalAndCompareD(tn, "+1", 1.0);
			evalAndCompareD(tn, "-1", -1.0);
			evalAndCompareD(tn, "!1", 0.0);
			evalAndCompareD(tn, "!0", 1.0);
			evalAndCompareD(tn, "!50", 0.0);
		}

		[TestMethod]
		public void BinaryOperatorsTest() {
			const string tn = "Binary operators test";

			evalAndCompareD(tn, "2 ^ 10", 1024.0);
			evalAndCompareD(tn, "3 * 7", 21);
			evalAndCompareD(tn, "5 / 2", 2.5);
			evalAndCompareD(tn, "11 \\ 10", 1.0);
			evalAndCompareD(tn, "5.5 \\ 1.5", 3.0);
			evalAndCompareD(tn, "55 % 21", 13.0);

			evalAndCompareD(tn, "1 + 1", 2.0);
			evalAndCompareD(tn, "0 - 1", -1.0);

			evalAndCompareD(tn, "7 < 7.001", 1.0);
			evalAndCompareD(tn, "7 < 7", 0.0);
			evalAndCompareD(tn, "7.001 < 7", 0.0);
			evalAndCompareD(tn, "999 < {}", 1.0);
			evalAndCompareD(tn, "{1,2,3} < {1,2,4}", 1.0);

			evalAndCompareD(tn, "7 > 7.001", 0.0);
			evalAndCompareD(tn, "7 > 7", 0.0);
			evalAndCompareD(tn, "7.001 > 7", 1.0);
			evalAndCompareD(tn, "{{}} > {}", 1.0);
			evalAndCompareD(tn, "{1,2} > {0, 0, 0}", 0.0);

			evalAndCompareD(tn, "7 <= 7.001", 1.0);
			evalAndCompareD(tn, "7 <= 7", 1.0);
			evalAndCompareD(tn, "7.001 <= 7", 0.0);
			evalAndCompareD(tn, "{{}} <= {{}}", 1.0);
			evalAndCompareD(tn, "{1,2,{}} <= {1,2,3}", 0.0);

			evalAndCompareD(tn, "7 >= 7.001", 0.0);
			evalAndCompareD(tn, "7 >= 7", 1.0);
			evalAndCompareD(tn, "7.001 >= 7", 1.0);
			evalAndCompareD(tn, "6 >= {5}", 0.0);
			evalAndCompareD(tn, "{{{8}}} >= {{{7}}}", 1.0);

			evalAndCompareD(tn, "7 == 7", 1.0);
			evalAndCompareD(tn, "7.001 == 7", 0.0);
			evalAndCompareD(tn, "{} == {}", 1.0);
			evalAndCompareD(tn, "{} == {{}}", 0.0);
			evalAndCompareD(tn, "nan == nan", double.NaN);
			evalAndCompareD(tn, "infinity == infinity", 1.0);

			evalAndCompareD(tn, "0 && 0", 0.0);
			evalAndCompareD(tn, "1 && 0", 0.0);
			evalAndCompareD(tn, "0 && 1", 0.0);
			evalAndCompareD(tn, "1 && 1", 1.0);

			evalAndCompareD(tn, "0 ^^ 0", 0.0);
			evalAndCompareD(tn, "1 ^^ 0", 1.0);
			evalAndCompareD(tn, "0 ^^ 1", 1.0);
			evalAndCompareD(tn, "1 ^^ 1", 0.0);

			evalAndCompareD(tn, "0 || 0", 0.0);
			evalAndCompareD(tn, "1 || 0", 1.0);
			evalAndCompareD(tn, "0 || 1", 1.0);
			evalAndCompareD(tn, "1 || 1", 1.0);
		}

		[TestMethod]
		public void FunctionsTest() {
			const string tn = "Functions test";

			evalAndCompareD(tn, "abs(3)", 3.0);
			evalAndCompareD(tn, "abs(-3)", 3.0);

			evalAndCompareD(tn, "sign(-0.1)", -1.0);
			evalAndCompareD(tn, "sign(0)", 0.0);
			evalAndCompareD(tn, "sign(0.1)", 1.0);

			evalAndCompareD(tn, "round(4.49)", 4.0);
			evalAndCompareD(tn, "round(4.50)", 4.0);  // rounding to nearest even :)
			evalAndCompareD(tn, "round(5.50)", 6.0);

			evalAndCompareD(tn, "floor(4.99)", 4.0);
			evalAndCompareD(tn, "floor(5.00)", 5.0);

			evalAndCompareD(tn, "ceiling(4.00)", 4.0);
			evalAndCompareD(tn, "ceiling(4.01)", 5.0);

			evalAndCompareD(tn, "min({1,2,3,4,5,6,7,8,9})", 1.0);
			evalAndCompareD(tn, "min(1,2,3,4,5,6,7,8,9)", 1.0);

			evalAndCompareD(tn, "max({1,2,3,4,5,6,7,8,9})", 9.0);
			evalAndCompareD(tn, "max(1,2,3,4,5,6,7,8,9)", 9.0);

			evalAndCompareD(tn, "sqrt(81)", 9.0);
			evalAndCompareD(tn, "sqrt(1)", 1.0);

			evalAndCompareD(tn, "factorial(0)", 1.0);
			evalAndCompareD(tn, "factorial(4)", 24.0);

			evalAndCompareD(tn, "log(1)", 0.0);
			evalAndCompareD(tn, "log(2.7182818284590452354)", 1.0);
			evalAndCompareD(tn, "log(2.7182818284590452354^6)", 6.0);
			evalAndCompareD(tn, "log(64, 8)", 2.0);

			evalAndCompareD(tn, "log10(1)", 0.0);
			evalAndCompareD(tn, "log10(1000)", 3.0);

			evalAndCompareD(tn, "sum()", 0.0);
			evalAndCompareD(tn, "sum(0,1,2,3,4,5,6,7,8,9)", 45);

			evalAndCompareD(tn, "product()", 1.0);
			evalAndCompareD(tn, "product(1,2,3,4)", 24);

			evalAndCompareD(tn, "average({7.2})", 7.2);
			evalAndCompareD(tn, "average({0,1,2,3,4,5,6,7,8,9})", 4.5);
			evalAndCompareD(tn, "average(0,1,2,3,4,5,6,7,8,9)", 4.5);

			evalAndCompareD(tn, "deg2rad(0)", 0.0);
			evalAndCompareD(tn, "deg2rad(90)", Math.PI / 2);
			evalAndCompareD(tn, "deg2rad(180)", Math.PI);

			evalAndCompareD(tn, "rad2deg(0)", 0.0);
			evalAndCompareD(tn, "rad2deg(3.14159265358979323846/2)", 90.0);
			evalAndCompareD(tn, "rad2deg(3.14159265358979323846)", 180.0);

			evalAndCompareD(tn, "sin(0)", 0.0);
			evalAndCompareD(tn, "sin(3.14159265358979323846/2)", 1.0);
			evalAndCompareD(tn, "sin(3.14159265358979323846/6)", 0.5);

			evalAndCompareD(tn, "cos(0)", 1.0);
			evalAndCompareD(tn, "cos(3.14159265358979323846/2)", 0.0);
			evalAndCompareD(tn, "cos(3.14159265358979323846/3)", 0.5);

			evalAndCompareD(tn, "tan(0)", 0.0);
			evalAndCompareD(tn, "tan(3.14159265358979323846 / 4)", 1.0);
			evalAndCompareD(tn, "tan(3 * 3.14159265358979323846 / 4)", -1.0);

			evalAndCompareD(tn, "asin(0)", 0.0);
			evalAndCompareD(tn, "asin(1)", Math.PI / 2);
			evalAndCompareD(tn, "asin(0.5)", Math.PI / 6);

			evalAndCompareD(tn, "acos(1)", 0.0);
			evalAndCompareD(tn, "acos(0)", Math.PI / 2);
			evalAndCompareD(tn, "acos(0.5)", Math.PI / 3);

			evalAndCompareD(tn, "atan(0)", 0.0);
			evalAndCompareD(tn, "atan(1)", Math.PI / 4);
			evalAndCompareD(tn, "atan(-1)", -Math.PI / 4);

			evalAndCompareD(tn, "if(1, 2, 3)", 2.0);
			evalAndCompareD(tn, "if(true, 2, 3)", 2.0);
			evalAndCompareD(tn, "if(0, 2, 3)", 3.0);
			evalAndCompareD(tn, "if(false, 2, 3)", 3.0);

			evalAndCompareD(tn, "length({})", 0.0);
			evalAndCompareD(tn, "length({3,3,3})", 3.0);
			evalAndCompareD(tn, "length({{},{}})", 2.0);

			evalAndCompareD(tn, "isNan(0)", 0.0);
			evalAndCompareD(tn, "isNan(nan)", 1.0);
			evalAndCompareD(tn, "isNan(0/0)", 1.0);

			evalAndCompareD(tn, "isInfinity(7)", 0.0);
			evalAndCompareD(tn, "isInfinity(infinity)", 1.0);
			evalAndCompareD(tn, "isInfinity(2/0)", 1.0);

		}

		[TestMethod]
		public void OperatorPrecedenceTest() {
			const string tn = "Operator precedence test";

			evalAndCompareD(tn, "5 - 1 - 1 - 1", 2.0);
			evalAndCompareD(tn, "5 - 1 * 3", 2.0);
			evalAndCompareD(tn, "5 + -3", 2.0);

			evalAndCompareD(tn, "-1 ^ 2", -1);
			evalAndCompareD(tn, "-2 ^ -2", -0.25);
			evalAndCompareD(tn, "-2 ^ -2 ^ -2 ^ -2", -0.558296564952432106);
			evalAndCompareD(tn, "3^2^3", 6561.0);

			evalAndCompareD(tn, "2+3 > 2*2", 1.0);
			evalAndCompareD(tn, "8 < 8 || 8 > 8", 0.0);

			evalAndCompareD(tn, "1 && 1 || 1 && 0", 1.0);
			evalAndCompareD(tn, "1 && 1 ^^ 1 && 0", 1.0);
			evalAndCompareD(tn, "1 && 1 ^^ 1 && 0 || 1 && 0 ^^ 0 && 1", 1.0);
		}

		[TestMethod]
		public void ArrayTest() {
			const string tn = "Array tests";

			evalAndCompareD(tn, "{1}[0]", 1.0);
			evalAndCompareD(tn, "{1, 2, 3, 4}[3]", 4.0);
		}


		private void evalAndCompareD(string testName, string exprStr, double exceptedValue) {
			evalAndCompare(testName, exprStr, exceptedValue.ToConst());
		}

		private void evalAndCompare(string testName, string exprStr, IValue exceptedValue) {

			var actual = TestUtils.EvaluateExpression(exprStr);

			Assert.IsTrue((actual.CompareTo(exceptedValue) == 0) || (actual.IsNaN && exceptedValue.IsNaN),
				"Expression `{0}` evaluated to `{1}`, but excepted value was `{2}`.",
				exprStr, actual, exceptedValue);
		}

		private string getTokensFromStr(string str) {

			var lexbuff = LexBuffer<char>.FromString(str);
			StringBuilder sb = new StringBuilder();
			var comments = new List<Ast.Comment>();
			var msgs = new MessageLogger();


			Parser.token tok;
			do {
				tok = Lexer.tokenize(msgs, comments, lexbuff);
				sb.Append(Parser.token_to_string(tok));
				sb.Append(" ");
			} while (tok != Parser.token.EOF);

			return sb.ToString();
		}
	}
}
