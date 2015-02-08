using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Malsys.Processing;
using Malsys.Processing.Output;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Web.Models.Lsystem {
	public class SimpleLsystemProcessor {

		private readonly ProcessManager processManager;
		private readonly InputBlockEvaled stdLib;

		public SimpleLsystemProcessor(ProcessManager processManager, InputBlockEvaled stdLib) {

			this.processManager = processManager;
			this.stdLib = stdLib;
		}


		public IEnumerable<HtmlString> Process(string input) {

			var logger = new MessageLogger();

			var inEvaled = processManager.CompileAndEvaluateInput(input, "simpleWebInput", logger);
			if (logger.ErrorOccurred) {
				foreach (var x in logger.Select(x => x.GetFullMessage())) {
					yield return new HtmlString(HttpUtility.HtmlEncode(x));
				}
				yield break;
			}

			inEvaled.Append(stdLib);
			var outputProvider = new InMemoryOutputProvider();

			// Set plain text rendering for SVG renderer (in case there is SVG rendering).
			foreach (var lsys in inEvaled.Lsystems) {
				lsys.Value.Statements.Add(new ConstantDefinition(null) {
					Name = "compressSvg",
					Value = Constant.False,
					IsComponentAssign = true,
				});
			}

			processManager.ProcessInput(inEvaled, outputProvider, logger, new TimeSpan(0, 0, 1));

			if (logger.ErrorOccurred) {
				foreach (var x in logger.Select(x => x.GetFullMessage())) {
					yield return new HtmlString(HttpUtility.HtmlEncode(x));
				}
				yield break;
			}

			var utfEncoding = new UTF8Encoding();

			foreach (InMemoryOutput imo in outputProvider.GetOutputs()) {
				switch (imo.MimeType) {
					case MimeType.Image.SvgXml: {
							var sb = new StringBuilder();
							foreach (var str in utfEncoding.GetString(imo.OutputData).SplitToLines().Skip(2)) {
								sb.AppendLine(str);
							}
							yield return new HtmlString(sb.ToString());
							break;
						}

					case MimeType.Text.Plain:
					default: {
							var sb = new StringBuilder();
							sb.Append("<pre>");
							sb.Append(HttpUtility.HtmlEncode(utfEncoding.GetString(imo.OutputData).Trim()));
							sb.Append("</pre>");
							yield return new HtmlString(sb.ToString());
							break;
						}
				}
			}
		}

	}
}