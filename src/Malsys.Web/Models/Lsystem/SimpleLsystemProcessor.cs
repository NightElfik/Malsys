// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
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
				foreach (var x in logger.Select(x => x.GetFullMessage())) {					
					yield return x;
				}
				yield break;
			}

			inEvaled.Append(stdLib);
			var outputProvider = new InMemoryOutputProvider();

			processManager.ProcessInput(inEvaled, outputProvider, logger, new TimeSpan(0, 0, 1));

			if (logger.ErrorOccurred) {
				foreach (var x in logger.Select(x => x.GetFullMessage())) {
					yield return x;
				}
				yield break;
			}

			var utfEncoding = new UTF8Encoding();

			foreach (InMemoryOutput imo in outputProvider.GetOutputs()) {
				switch (imo.MimeType) {
					case MimeType.Image.SvgXml:

						break;

					case MimeType.Text.Plain:
					default:
						yield return utfEncoding.GetString(imo.OutputData);
						break;
				}
			}
		}

	}
}