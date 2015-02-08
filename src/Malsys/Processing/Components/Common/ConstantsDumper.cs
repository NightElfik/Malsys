using System.IO;
using System.Linq;
using Malsys.SemanticModel;

namespace Malsys.Processing.Components.Common {
	/// <summary>
	/// Prints all defined constants from the global scope.
	/// To be able to use this component you will need to process some L-system with it.
	/// It is possible to define an empty L-system or you can use the Constants L-system from the standard library.
	/// The process statement may look like this: <code>process Constants with ConstantDumper;</code>
	/// </summary>
	/// <name>Constants dumper</name>
	/// <group>Special</group>
	public class ConstantsDumper : IProcessStarter {

		private ProcessContext context;



		/// <summary>
		/// Default behavior is to print only constants in main input.
		/// If this is set to true all constants will be printed.
		/// </summary>
		/// <expected>true or false</expected>
		/// <default>false</default>
		[UserSettable]
		public Constant DumpAllConstants { get; set; }


		public IMessageLogger Logger { get; set; }


		public void Reset() {
			context = null;
			DumpAllConstants = Constant.False;
		}

		public void Initialize(ProcessContext ctxt) {
			context = ctxt;
		}

		public void Cleanup() { }

		public void Dispose() { }


		public void Start(bool doMeasure) {
			dumpConstants();
		}

		public void Abort() {

		}


		private void dumpConstants() {

			var constants = context.InputData.ExpressionEvaluatorContext.GetAllStoredVariables().ToList();
			bool dumpAll = DumpAllConstants.IsTrue;

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
