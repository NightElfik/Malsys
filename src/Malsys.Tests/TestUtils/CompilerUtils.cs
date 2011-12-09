using System;
using Malsys.Compilers;
using Malsys.IO;
using Malsys.Parsing;
using Malsys.SemanticModel.Compiled;
using Malsys.SourceCode.Printers;
using Microsoft.FSharp.Text.Lexing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Malsys.Evaluators;
using ConstsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;
using FunsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Compiled.FunctionEvaledParams>;

namespace Malsys.Tests {
	internal static class CompilerUtils {

		public static Ast.Expression ParseExpression(string input) {

			var lexBuff = LexBuffer<char>.FromString(input);
			var logger = new MessageLogger();
			var result = ParserUtils.ParseExpression(lexBuff, logger, "testInput");
			if (logger.ErrorOcured) {
				Console.WriteLine(logger.ToString());
				Assert.Fail("Failed to parse expression: " + input);
			}

			return result;
		}

		public static IExpression CompileExpression(string input) {

			var parsedExpr = ParseExpression(input);
			return CompileExpression(parsedExpr);
		}

		public static Ast.InputBlock ParseLsystem(string input) {

			var lexBuff = LexBuffer<char>.FromString(input);
			var logger = new MessageLogger();
			var result = ParserUtils.ParseInputNoComents(lexBuff, logger, "testInput");
			if (logger.ErrorOcured) {
				Console.WriteLine(logger.ToString());
				Assert.Fail("Failed to parse L-system: " + input);
			}

			return result;
		}

		public static IExpression CompileExpression(Ast.Expression input) {

			var logger = new MessageLogger();
			var compiledExpr = new CompilersContainer().ResolveExpressionCompiler().Compile(input, logger);

			if (logger.ErrorOcured) {
				Console.WriteLine(logger.ToString());
				Assert.Fail("Failed to compile expression");
			}

			return compiledExpr;
		}

		public static InputBlock CompileLsystem(string input) {
			return CompileLsystem(ParseLsystem(input));
		}

		public static InputBlock CompileLsystem(Ast.InputBlock input) {

			var logger = new MessageLogger();
			var compiled = new CompilersContainer().ResolveInputCompiler().Compile(input, logger);

			if (logger.ErrorOcured) {
				Console.WriteLine(logger.ToString());
				Assert.Fail("Failed to compile L-system");
			}

			return compiled;
		}

		public static Malsys.SemanticModel.Evaluated.IValue EvaluateExpression(IExpression input, ConstsMap consts, FunsMap funs) {

			return new ExpressionEvaluator().Evaluate(input, consts, funs);
		}

		public static Malsys.SemanticModel.Evaluated.IValue EvaluateExpression(string input) {

			return new ExpressionEvaluator().Evaluate(CompileExpression(input));
		}

		public static Malsys.SemanticModel.Evaluated.IValue EvaluateExpression(string input, ConstsMap consts, FunsMap funs) {

			return new ExpressionEvaluator().Evaluate(CompileExpression(input), consts, funs);
		}

		public static SemanticModel.Evaluated.InputBlock EvaluateLsystem(InputBlock input) {

			return new EvaluatorsContainer().EvaluateInput(input);
		}

		public static SemanticModel.Evaluated.InputBlock EvaluateLsystem(string input) {

			return new EvaluatorsContainer().EvaluateInput(CompileLsystem(input));
		}


		public static string Print(Malsys.SemanticModel.Evaluated.IValue val) {

			var writer = new IndentStringWriter();
			new CanonicPrinter(writer).Print(val);
			return writer.GetResult();
		}

		public static string Print(IExpression expr) {

			var writer = new IndentStringWriter();
			new CanonicPrinter(writer).Print(expr);
			return writer.GetResult();
		}

		public static string Print(Ast.InputBlock val) {

			var writer = new IndentStringWriter();
			new CanonicAstPrinter(writer).Print(val);
			return writer.GetResult();
		}

		public static string Print(Ast.Expression expr) {

			var writer = new IndentStringWriter();
			new CanonicAstPrinter(writer).Print(expr);
			return writer.GetResult();
		}

		public static string Print(Malsys.SemanticModel.Evaluated.InputBlock input) {

			var writer = new IndentStringWriter();
			new CanonicPrinter(writer).Print(input);
			return writer.GetResult();
		}

	}
}
