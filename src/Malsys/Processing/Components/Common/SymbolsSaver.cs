using System.IO;
using Malsys.IO;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;
using Malsys.SourceCode.Printers;

namespace Malsys.Processing.Components.Common {
	[Component("Symbol saver", ComponentGroupNames.Interpreters)]
	public class SymbolsSaver : ISymbolProcessor {

		private IOutputProvider outProvider;
		private string outputName;

		private IndentTextWriter writer;
		private CanonicPrinter printer;

		bool notMeasuring;


		#region ISymbolProcessor Members

		public void ProcessSymbol(Symbol<IValue> symbol) {
			if (notMeasuring) {
				printer.Print(symbol);
				writer.Write(" ");
			}
		}

		public bool RequiresMeasure { get { return false; } }

		public void Initialize(ProcessContext ctxt) {
			outProvider = ctxt.OutputProvider;
			outputName = "Symbols from `{0}`".Fmt(ctxt.Lsystem.Name);
		}

		public void Cleanup() { }

		public void BeginProcessing(bool measuring) {
			notMeasuring = !measuring;

			if (notMeasuring) {
				writer = new IndentTextWriter(new StreamWriter(outProvider.GetOutputStream<SymbolsSaver>(outputName, MimeType.Text.Plain)));
				printer = new CanonicPrinter(writer);
			}
		}

		public void EndProcessing() {
			if (notMeasuring) {
				writer.Dispose();
				writer = null;
				printer = null;
			}
		}

		#endregion
	}
}
