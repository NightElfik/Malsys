using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Malsys.Compilers;
using Malsys.Evaluators;
using Malsys.Processing;
using Malsys.Processing.Output;
using Malsys.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Malsys.Resources;
using Malsys.SemanticModel.Evaluated;
using Malsys.SemanticModel;

namespace Malsys.Tests.Processing {
	[TestClass]
	public class MinimalWorkingExample {
		[TestMethod]
		public void Test() {

			string sourceCode = string.Join("\n",
				"lsystem Fibonacci {",
				"	set iterations = 6;",
				"	set interpretEveryIteration = true;",
				"	set symbols axiom = A(0) B(1);",
				"	rewrite          A(a) { B(b) } to A(b);",
				"	rewrite { A(a) } B(b)          to B(a @ b);",
				"}",
				"process all with SymbolPrinter;");

			var logger = new MessageLogger();
			var knownStuffProvider = new KnownConstOpProvider();
			IExpressionEvaluatorContext evalCtxt = new ExpressionEvaluatorContext();
			var componentResolver = new ComponentResolver();

			var loader = new MalsysLoader();
			loader.LoadMalsysStuffFromAssembly(Assembly.GetAssembly(typeof(MalsysLoader)),
				knownStuffProvider, knownStuffProvider, ref evalCtxt, componentResolver, logger);

			if (logger.ErrorOccurred) {
				throw new Exception("Failed to register Malsys stuff. "
					+ logger.AllMessagesToFullString());
			}

			knownStuffProvider.AddOperator(new OperatorCore("@", 300, 320,
				ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant,
				(l, r) => ((Constant)l + (Constant)r).ToConst()));

			var compiler = new CompilersContainer(knownStuffProvider, knownStuffProvider);
			var evaluator = new EvaluatorsContainer(evalCtxt);
			var processMgr = new ProcessManager(compiler, evaluator, componentResolver);


			var evaledInput = processMgr.CompileAndEvaluateInput(sourceCode, "testInput", logger);

			if (logger.ErrorOccurred) {
				throw new Exception("Failed to evaluate input. " + logger.AllMessagesToFullString());
			}

			string stdlibSource;
			using (Stream stream = new ResourcesReader().GetResourceStream(ResourcesHelper.StdLibResourceName)) {
				using (TextReader reader = new StreamReader(stream)) {
					stdlibSource = reader.ReadToEnd();
				}
			}

			var stdLib = processMgr.CompileAndEvaluateInput(stdlibSource, "stdLib", logger);
			if (logger.ErrorOccurred) {
				throw new Exception("Failed to build std lib. " + logger.AllMessagesToFullString());
			}

			evaledInput = stdLib.JoinWith(evaledInput);

			var outProvider = new InMemoryOutputProvider();

			processMgr.ProcessInput(evaledInput, outProvider, logger, new TimeSpan(0, 0, 5));

			if (logger.ErrorOccurred) {
				throw new Exception("Failed to process input. " + logger.AllMessagesToFullString());
			}

			var encoding = new UTF8Encoding();
			var outputs = outProvider.GetOutputs().Select(x => encoding.GetString(x.OutputData));

			foreach (var o in outputs) {
				Console.WriteLine(o);
			}

		}
	}
}
