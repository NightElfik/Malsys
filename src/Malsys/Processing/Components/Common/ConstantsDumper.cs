/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.IO;
using System.Linq;
using Malsys.SemanticModel;

namespace Malsys.Processing.Components.Common {
	/// <summary>
	/// Prints all defined constants from global scope.
	/// To use this component process input with some dummy L-system and
	/// with process configuration containing only this component.
	/// Standard library in Malsys offers predefined L-system and configuration for this.
	/// Just use following process statement.
	/// <code>process Constants with ConstantDumper;</code>
	/// </summary>
	/// <name>Constants dumper</name>
	/// <group>Special</group>
	public class ConstantsDumper : IProcessStarter {

		private ProcessContext context;


		public IMessageLogger Logger { get; set; }

		/// <summary>
		/// Default behavior is to print only constants in main input.
		/// If this is set to true all constants will be printed.
		/// </summary>
		/// <expected>true or false</expected>
		/// <default>false</default>
		[UserSettable]
		public Constant DumpAllConstants { get; set; }


		public void Initialize(ProcessContext ctxt) {
			context = ctxt;
		}

		public void Cleanup() {
			context = null;
			DumpAllConstants = Constant.False;
		}


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
