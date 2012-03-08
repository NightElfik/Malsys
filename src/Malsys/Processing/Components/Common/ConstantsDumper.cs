using System.IO;
using System.Linq;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components.Common {
	public class ConstantsDumper {

		public void DumpConstants(InputBlockEvaled inBlock, IOutputProvider outputProvider, IMessageLogger logger, string onlyFromInput = null) {

			var constants = inBlock.ExpressionEvaluatorContext.GetAllStoredVariables().ToList();

			if (constants.Count == 0) {
				return;
			}

			using (TextWriter writer = new StreamWriter(outputProvider.GetOutputStream<ConstantsDumper>("Variables dump", MimeType.Text.Plain))) {

				foreach (var c in constants) {

					if (onlyFromInput != null && c.Metadata != null && c.Metadata is Ast.ConstantDefinition) {
						if (((Ast.ConstantDefinition)c.Metadata).Position.SourceName != onlyFromInput) {
							continue;
						}
					}

					writer.Write(c.Name);
					writer.Write(" = ");
					writer.Write(c.ValueFunc());
					writer.WriteLine(";");
				}
			}

		}
	}
}
