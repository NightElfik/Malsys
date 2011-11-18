using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.SourceCode.Printers;
using Malsys.IO;
using System.IO;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components.Common {
	public class SymbolsSaver : ISymbolProcessor {

		private FilesManager filesMgr;

		private TextWriter writer;
		private CanonicPrinter printer;

		bool notMeasuring;


		#region ISymbolProcessor Members

		public void ProcessSymbol(Symbol<IValue> symbol) {
			if (notMeasuring) {
				printer.Print(symbol);
				writer.Write(" ");
			}
		}

		#endregion

		#region IComponent Members

		public ProcessContext Context {
			set { filesMgr = value.FilesManager; }
		}

		public void BeginProcessing(bool measuring) {
			notMeasuring = !measuring;

			if (notMeasuring) {
				var path = filesMgr.GetNewOutputFilePath(".txt");
				writer = new StreamWriter(path);
				printer = new CanonicPrinter(new IndentTextWriter(writer));
			}
		}

		public void EndProcessing() {
			if (notMeasuring) {
				writer.Close();
			}
		}

		#endregion
	}
}
