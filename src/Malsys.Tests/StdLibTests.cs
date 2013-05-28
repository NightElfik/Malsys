// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.IO;
using Malsys.Compilers;
using Malsys.Evaluators;
using Malsys.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests {
	[TestClass]
	public class StdLibTests {

		[TestMethod]
		public void BuildStdLibTest() {

			string resName = ResourcesHelper.StdLibResourceName;

			var logger = new MessageLogger();

			using (Stream stream = new ResourcesReader().GetResourceStream(resName)) {
				using (TextReader reader = new StreamReader(stream)) {
					var inCompiled = TestUtils.CompilersContainer.CompileInput(reader, resName, logger);
					var stdLib = new EvaluatorsContainer(TestUtils.ExpressionEvaluatorContext).EvaluateInput(inCompiled, logger);
					if (logger.ErrorOccurred) {
						foreach (var msg in logger) {
							Console.WriteLine(msg.GetFullMessage());
						}
						Assert.Fail();
					}
					Assert.AreNotEqual(stdLib, null);
				}
			}

		}

	}
}
