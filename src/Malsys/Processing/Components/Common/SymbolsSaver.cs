/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.IO;
using Malsys.IO;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;
using Malsys.SourceCode.Printers;

namespace Malsys.Processing.Components.Common {
	/// <summary>
	/// Prints all processed symbols (with their parameters) as the text.
	/// Symbols are delimited with a space.
	/// </summary>
	/// <name>Symbols saver</name>
	/// <group>General</group>
	public class SymbolsSaver : ISymbolProcessor {

		private IOutputProvider outProvider;
		private string outputName;

		private IndentTextWriter writer;
		private CanonicPrinter printer;

		bool notMeasuring;


		public IMessageLogger Logger { get; set; }


		public bool RequiresMeasure { get { return false; } }

		public void Initialize(ProcessContext ctxt) {
			outProvider = ctxt.OutputProvider;
			outputName = "Symbols from `{0}`".Fmt(ctxt.Lsystem.Name);
		}

		public void Cleanup() {
			outProvider = null;
		}


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


		public void ProcessSymbol(Symbol<IValue> symbol) {
			if (notMeasuring) {
				printer.Print(symbol);
				writer.Write(" ");
			}
		}

	}
}
