using System.IO;
using System.Linq;
using Malsys.SemanticModel;

namespace Malsys.Processing.Components.Common {
	/// <summary>
	/// Writes all defined constants from global scope.
	/// </summary>
	public class ConstantsDumper : IProcessStarter {

		private ProcessContext context;

		[UserSettable]
		public Constant DumpAllConstants { get; set; }


		#region IComponent Members

		public void Initialize(ProcessContext ctxt) {
			context = ctxt;
		}

		public void Cleanup() {
			context = null;
		}

		#endregion


		#region IProcessStarter Members

		public void Start(bool doMeasure) {
			dumpConstants();
		}

		public void Abort() {

		}

		#endregion


		private void dumpConstants() {

			var constants = context.InputData.ExpressionEvaluatorContext.GetAllStoredVariables().ToList();
			bool dumpAll = DumpAllConstants != null && !DumpAllConstants.IsZero;

			using (TextWriter writer = new StreamWriter(context.OutputProvider.GetOutputStream<ConstantsDumper>("Constants dump", MimeType.Text.Plain))) {

				foreach (var c in constants) {

					if (!dumpAll && c.Metadata != null && c.Metadata is Ast.ConstantDefinition) {
						if (((Ast.ConstantDefinition)c.Metadata).Position.SourceName != context.InputData.SourceName) {
							continue;  // current constant definition is not from current source
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
