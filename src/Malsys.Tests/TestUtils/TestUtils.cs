using System;
using Malsys.Compilers;
using Malsys.Evaluators;
using Malsys.IO;
using Malsys.Parsing;
using Malsys.SemanticModel.Compiled;
using Malsys.SourceCode.Printers;
using Microsoft.FSharp.Text.Lexing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Malsys.Reflection;
using Malsys.Resources;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Tests {
	internal static class TestUtils {

		public static readonly IExpressionEvaluatorContext ExpressionEvaluatorContext;

		static TestUtils() {

			ExpressionEvaluatorContext = new FunctionDumper().RegiterAllFunctions(typeof(StdFunctions), new ExpressionEvaluatorContext());

		}


		public static ImmutableList<Ast.LsystemSymbol> ParseSymbols(string input) {

			var lexBuff = LexBuffer<char>.FromString(input);
			var logger = new MessageLogger();
			var symbolsAst = ParserUtils.ParseSymbols(lexBuff, logger, "testInput");

			if (logger.ErrorOccurred) {
				Console.WriteLine("in: " + input);
				Assert.Fail("Failed to parse symbols. " + logger.ToString());
			}

			return symbolsAst;
		}

		public static ImmutableList<SemanticModel.Symbol<IExpression>> CompileSymbols(ImmutableList<Ast.LsystemSymbol> symbolsAst) {

			var compiler = new CompilersContainer().Resolve<ISymbolCompiler>();
			var logger = new MessageLogger();
			var symbols = compiler.CompileList<Ast.LsystemSymbol, Malsys.SemanticModel.Symbol<IExpression>>(symbolsAst, logger);

			if (logger.ErrorOccurred) {
				Assert.Fail("Failed to compile symbols. " + logger.ToString());
			}

			return symbols;
		}

		public static ImmutableList<SemanticModel.Symbol<IExpression>> CompileSymbols(string input) {
			var parsed = ParseSymbols(input);
			return CompileSymbols(parsed);
		}


		public static Ast.Expression ParseExpression(string input) {

			var lexBuff = LexBuffer<char>.FromString(input);
			var logger = new MessageLogger();
			var result = ParserUtils.ParseExpression(lexBuff, logger, "testInput");
			if (logger.ErrorOccurred) {
				Console.WriteLine(logger.ToString());
				Assert.Fail("Failed to parse expression: " + input);
			}

			return result;
		}

		public static IExpression CompileExpression(string input) {

			var parsedExpr = ParseExpression(input);
			return CompileExpression(parsedExpr);
		}

		public static IExpression CompileExpression(Ast.Expression input) {

			var logger = new MessageLogger();
			var compiledExpr = new CompilersContainer().ResolveExpressionCompiler().Compile(input, logger);

			if (logger.ErrorOccurred) {
				Console.WriteLine(logger.ToString());
				Assert.Fail("Failed to compile expression");
			}

			return compiledExpr;
		}

		public static IValue EvaluateExpression(string input) {

			return ExpressionEvaluatorContext.Evaluate(CompileExpression(input));
		}

		public static IValue EvaluateExpression(string input, IExpressionEvaluatorContext eec) {

			return eec.Evaluate(CompileExpression(input));
		}


		public static Ast.InputBlock ParseLsystem(string input) {

			var lexBuff = LexBuffer<char>.FromString(input);
			var logger = new MessageLogger();
			var result = ParserUtils.ParseInputNoComents(lexBuff, logger, "testInput");
			if (logger.ErrorOccurred) {
				Console.WriteLine(logger.ToString());
				Assert.Fail("Failed to parse input: " + input);
			}

			return result;
		}

		public static InputBlock CompileInput(string input) {
			return CompileInput(ParseLsystem(input));
		}

		public static InputBlock CompileInput(Ast.InputBlock input) {

			var logger = new MessageLogger();
			var compiled = new CompilersContainer().ResolveInputCompiler().Compile(input, logger);

			if (logger.ErrorOccurred) {
				Console.WriteLine(logger.ToString());
				Assert.Fail("Failed to compile input");
			}

			return compiled;
		}


		public static InputBlockEvaled EvaluateLsystem(InputBlock input) {

			return new EvaluatorsContainer(ExpressionEvaluatorContext).EvaluateInput(input, ExpressionEvaluatorContext);
		}

		public static InputBlockEvaled EvaluateLsystem(string input) {

			return new EvaluatorsContainer(ExpressionEvaluatorContext).EvaluateInput(CompileInput(input), ExpressionEvaluatorContext);
		}


		public static LsystemEvaled EvaluateLsystem(LsystemEvaledParams lsystem) {

			return new EvaluatorsContainer(ExpressionEvaluatorContext).EvaluateLsystem(lsystem, ImmutableList<IValue>.Empty, ExpressionEvaluatorContext);
		}

		public static string Print(IValue val) {

			var writer = new IndentStringWriter();
			new CanonicPrinter(writer).Print(val);
			return writer.GetResult();

		}

		public static string Print(SemanticModel.Constant c) {

			return Print((IValue)c);

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

		public static string Print(InputBlockEvaled input) {

			var writer = new IndentStringWriter();
			new CanonicPrinter(writer).Print(input);
			return writer.GetResult();

		}

		public static string Print(ImmutableList<SemanticModel.Symbol<IValue>> symbols) {

			var writer = new IndentStringWriter();
			new CanonicPrinter(writer).Print(symbols);
			return writer.GetResult();

		}

	}
}
