using System.IO;
using Malsys.IO;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;
using Malsys.SourceCode.Printers;

namespace Malsys.Processing.Components.Common {
	public class SymbolsSaver : ISymbolProcessor {

		private IOutputProvider outProvider;

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
		}

		public void Cleanup() { }

		public void BeginProcessing(bool measuring) {
			notMeasuring = !measuring;

			if (notMeasuring) {
				writer = new IndentTextWriter(new StreamWriter(outProvider.GetOutputStream<SymbolsSaver>(".txt")));
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
