using System.IO;
using Malsys.SemanticModel.Evaluated;
using ConstsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;
using Microsoft.FSharp.Core;

namespace Malsys.Processing.Components.Common {
	public class ConstantsDumper {

		public void DumpConstants(InputBlock inBlock, IOutputProvider outputProvider, IMessageLogger logger, string onlyFromInput = null) {

			if (inBlock.GlobalConstants.Count == 0) {
				return;
			}

			using (TextWriter writer = new StreamWriter(outputProvider.GetOutputStream<ConstantsDumper>("Variables dump", MimeType.Text.Plain))) {

				foreach (var c in inBlock.GlobalConstants) {

					if (onlyFromInput != null) {
						var maybeConstAst = inBlock.GlobalConstantsAstNodes.TryFind(c.Key);
						if (OptionModule.IsSome(maybeConstAst)) {
							if (maybeConstAst.Value.Position.SourceName != onlyFromInput) {
								continue;
							}
						}
					}

					writer.Write(c.Key);
					writer.Write(" = ");
					writer.Write(c.Value);
					writer.WriteLine(";");
				}
			}

		}
	}
}
