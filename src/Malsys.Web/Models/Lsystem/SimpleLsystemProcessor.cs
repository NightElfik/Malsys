/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.Processing;
using Malsys.Processing.Output;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Web.Models.Lsystem {
	public class SimpleLsystemProcessor {

		private readonly ProcessManager processManager;
		private readonly InputBlockEvaled stdLib;

		public SimpleLsystemProcessor(ProcessManager processManager, InputBlockEvaled stdLib) {

			this.processManager = processManager;
			this.stdLib = stdLib;
		}


		public IEnumerable<string> Process(string input) {

			var logger = new MessageLogger();

			var inEvaled = processManager.CompileAndEvaluateInput(input, "simpleWebInput", logger);
			if (logger.ErrorOccurred) {
				return logger.Select(x => x.GetFullMessage());
			}

			inEvaled = stdLib.JoinWith(inEvaled);
			var outputProvider = new InMemoryOutputProvider();

			processManager.ProcessInput(inEvaled, outputProvider, logger, new TimeSpan(0, 0, 1));

			if (logger.ErrorOccurred) {
				return logger.Select(x => x.GetFullMessage());
			}


			var utfEncoding = new UTF8Encoding();
			return outputProvider.GetOutputs().Select(x => utfEncoding.GetString(x.OutputData));
		}

	}
}