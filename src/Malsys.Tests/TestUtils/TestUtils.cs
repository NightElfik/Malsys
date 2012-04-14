using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Malsys.Compilers;
using Malsys.Evaluators;
using Malsys.IO;
using Malsys.Parsing;
using Malsys.Processing;
using Malsys.Processing.Components;
using Malsys.Reflection;
using Malsys.Resources;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;
using Malsys.SourceCode.Printers;
using Microsoft.FSharp.Text.Lexing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests {
	internal static class TestUtils {

		public static readonly IExpressionEvaluatorContext ExpressionEvaluatorContext;

		public static readonly InputBlockEvaled StdLib;

		public static readonly ComponentResolver StdResolver;

		public static readonly KnownConstOpProvider DefaultKnownStuffProvider;

		public static readonly CompilersContainer CompilersContainer;


		static TestUtils() {

			var logger = new MessageLogger();
			DefaultKnownStuffProvider = new KnownConstOpProvider();
			ExpressionEvaluatorContext = new ExpressionEvaluatorContext();
			StdResolver = new ComponentResolver();

			var loader = new MalsysLoader();
			loader.LoadMalsysStuffFromAssembly(Assembly.GetAssembly(typeof(MalsysLoader)), DefaultKnownStuffProvider, DefaultKnownStuffProvider,
				ref ExpressionEvaluatorContext, StdResolver, logger);


			if (logger.ErrorOccurred) {
				throw new Exception("Failed to register Malsys stuff. " + logger.AllMessagesToFullString());
			}

			CompilersContainer = new CompilersContainer(DefaultKnownStuffProvider, DefaultKnownStuffProvider);

			string resName = ResourcesHelper.StdLibResourceName;
			using (Stream stream = new ResourcesReader().GetResourceStream(resName)) {
				using (TextReader reader = new StreamReader(stream)) {
					var inCompiled = new CompilersContainer(DefaultKnownStuffProvider, DefaultKnownStuffProvider).CompileInput(reader, resName, logger);
					var stdLib = new EvaluatorsContainer(TestUtils.ExpressionEvaluatorContext).EvaluateInput(inCompiled, logger);
					if (!logger.ErrorOccurred) {
						StdLib = stdLib;
					}
				}
			}

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

		public static ImmutableList<Symbol<IExpression>> CompileSymbols(ImmutableList<Ast.LsystemSymbol> symbolsAst) {

			var compiler = CompilersContainer.Resolve<ISymbolCompiler>();
			var logger = new MessageLogger();
			var symbols = compiler.CompileList<Ast.LsystemSymbol, Symbol<IExpression>>(symbolsAst, logger);

			if (logger.ErrorOccurred) {
				Assert.Fail("Failed to compile symbols. " + logger.ToString());
			}

			return symbols;
		}

		public static ImmutableList<Symbol<string>> CompileSymbolsAsPattern(ImmutableList<Ast.LsystemSymbol> symbolsAst) {

			var compiler = CompilersContainer.Resolve<ISymbolCompiler>();
			var logger = new MessageLogger();
			var symbols = compiler.CompileList<Ast.LsystemSymbol, Symbol<string>>(symbolsAst, logger);

			if (logger.ErrorOccurred) {
				Assert.Fail("Failed to compile symbols. " + logger.ToString());
			}

			return symbols;
		}

		public static ImmutableList<Symbol<IExpression>> CompileSymbols(string input) {
			var parsed = ParseSymbols(input);
			return CompileSymbols(parsed);
		}

		public static ImmutableList<Symbol<string>> CompileSymbolsAsPattern(string input) {
			var parsed = ParseSymbols(input);
			return CompileSymbolsAsPattern(parsed);
		}


		public static ImmutableList<Symbol<IValue>> EvaluateSymbols(ImmutableList<Symbol<IExpression>> input) {
			return input.Select(s => new Symbol<IValue>(s.Name, ExpressionEvaluatorContext.EvaluateList(s.Arguments))).ToImmutableList();
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
			var compiledExpr = CompilersContainer.ResolveExpressionCompiler().Compile(input, logger);

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
			var compiled = CompilersContainer.ResolveInputCompiler().Compile(input, logger);

			if (logger.ErrorOccurred) {
				Console.WriteLine(logger.ToString());
				Assert.Fail("Failed to compile input");
			}

			return compiled;
		}


		public static InputBlockEvaled EvaluateInput(InputBlock input) {
			var logger = new MessageLogger();
			var result = new EvaluatorsContainer(ExpressionEvaluatorContext).EvaluateInput(input, ExpressionEvaluatorContext, logger);
			if (logger.ErrorOccurred) {
				throw new Exception(logger.AllMessagesToFullString());
			}
			return result;
		}

		public static InputBlockEvaled EvaluateInput(string input) {
			var logger = new MessageLogger();
			var result = new EvaluatorsContainer(ExpressionEvaluatorContext).EvaluateInput(CompileInput(input), ExpressionEvaluatorContext, logger);
			if (logger.ErrorOccurred) {
				throw new Exception(logger.AllMessagesToFullString());
			}
			return result;
		}


		public static LsystemEvaled EvaluateLsystem(LsystemEvaledParams lsystem) {

			var logger = new MessageLogger();
			var result = new EvaluatorsContainer(ExpressionEvaluatorContext).ResolveLsystemEvaluator().Evaluate(lsystem, ImmutableList<IValue>.Empty,
				ExpressionEvaluatorContext, new BaseLsystemResolver(), logger);

			if (logger.ErrorOccurred) {
				Console.WriteLine(logger.ToString());
				Assert.Fail("Failed to evaluate input");
			}
			return result;
		}

		public static string Print(IValue val) {

			var writer = new IndentStringWriter();
			new CanonicPrinter(writer).Print(val);
			return writer.GetResult();

		}

		public static string Print(Constant c) {

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

		public static string Print(ImmutableList<Symbol<IValue>> symbols) {

			var writer = new IndentStringWriter();
			new CanonicPrinter(writer).Print(symbols);
			return writer.GetResult();

		}

	}
}
