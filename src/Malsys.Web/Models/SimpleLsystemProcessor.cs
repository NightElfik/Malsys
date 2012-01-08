using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.Processing;
using Malsys.Processing.Output;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Web.Models {
	public class SimpleLsystemProcessor {

		private readonly ProcessManager processManager;
		private readonly InputBlock stdLib;

		public SimpleLsystemProcessor(ProcessManager processManager, InputBlock stdLib) {

			this.processManager = processManager;
			this.stdLib = stdLib;
		}


		public IEnumerable<string> Process(string input) {

			var logger = new MessageLogger();

			var inEvaled = processManager.CompileAndEvaluateInput(input, logger);
			if (logger.ErrorOcured) {
				return new string[] { logger.AllMessagesToFullString() };
			}

			inEvaled = stdLib.JoinWith(inEvaled);
			var outputProvider = new InMemoryOutputProvider();

			processManager.ProcessLsystems(inEvaled, outputProvider, logger, new TimeSpan(0, 0, 1));
			if (logger.ErrorOcured) {
				return new string[] { logger.AllMessagesToFullString() };
			}

			var utfEncoding = new UTF8Encoding();
			return outputProvider.GetOutputs().Select(x => utfEncoding.GetString(x.OutputData));
		}

	}
}