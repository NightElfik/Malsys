using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Malsys.IO {
	public abstract class IndentWriter {

		protected bool lineIndented = false;

		[ContractPublicPropertyName("IndentLevel")]
		protected int indentLevel = 0;


		public IndentWriter(string indentString = "\t") {

			Contract.Requires<ArgumentNullException>(indentString != null);

			IndentString = indentString;
		}

		[ContractInvariantMethod]
		private void ObjectInvariant() {
			Contract.Invariant(IndentString != null);
			Contract.Invariant(indentLevel >= 0);
		}

		public string IndentString { get; set; }

		public int IndentLevel { get { return indentLevel; } }



		public void Write(string str) {
			Contract.Requires<ArgumentNullException>(str != null);
			writeIndented(str);
		}

		public void WriteWithNewLines(string str) {

			Contract.Requires<ArgumentNullException>(str != null);
			if (str.Contains('\n') || str.Contains('\r')) {
				writeLines(str.SplitToLines(), false);
				return;
			}

			writeIndented(str);
		}

		public void NewLine() {
			newLine();
			lineIndented = false;
		}

		public void WriteLine(string str) {
			Contract.Requires<ArgumentNullException>(str != null);
			writeIndented(str);
			NewLine();
		}

		public void WriteLineWithNewLines(string str) {

			Contract.Requires<ArgumentNullException>(str != null);
			if (str.Contains('\n') || str.Contains('\r')) {
				writeLines(str.SplitToLines(), true);
				return;
			}

			writeIndented(str);
			NewLine();
		}

		public void WriteLines(params string[] lines) {

			Contract.Requires<ArgumentNullException>(lines != null);
			Contract.ForAll(lines, l => l != null);

			foreach (var line in lines) {
				WriteLine(line);
			}
		}

		public void Indent() {
			indentLevel++;
		}

		public void Unindent() {
			Contract.Requires<InvalidOperationException>(indentLevel > 0);
			indentLevel--;
		}


		protected abstract void write(string str);
		protected abstract void newLine();


		private void writeLines(IEnumerable<string> lines, bool newLineAfterLast) {

			Contract.Requires<ArgumentNullException>(lines != null);

			bool first = true;

			foreach (var line in lines) {
				if (first) {
					first = false;
				}
				else {
					NewLine();
				}
				writeIndented(line);
			}

			if (newLineAfterLast) {
				NewLine();
			}
		}

		private void writeIndented(string line) {
			if (!lineIndented) {
				writeIndent(indentLevel);
				lineIndented = true;
			}

			write(line);
		}

		private void writeIndent(int level) {
			write(IndentString.Repeat(level));
		}
	}
}
