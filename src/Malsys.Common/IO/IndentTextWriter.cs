using System;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Malsys.IO {
	public abstract class IndentTextWriter {

		protected bool lineIndented = false;
		protected int indentLevel = 0;


		public IndentTextWriter(string indentString = "\t") {
			IndentString = indentString;
		}

		public string IndentString { get; set; }

		public int IndentLevel { get { return indentLevel; } }



		public void Write(string str) {
			Contract.Requires<ArgumentException>(!str.Contains('\n') && !str.Contains('\r'));
			writeIndented(str);
		}

		public void NewLine() {
			newLine();
			lineIndented = false;
		}

		public void WriteLine(string str) {
			Contract.Requires<ArgumentException>(!str.Contains('\n') && !str.Contains('\r'));
			writeIndented(str);
			NewLine();
		}

		public void WriteLines(params string[] lines) {
			foreach (var line in lines) {
				WriteLine(line);
			}
		}

		public void Indent() {
			indentLevel++;
		}

		public void Unindent() {
			Contract.Requires<InvalidOperationException>(IndentLevel > 0);
			indentLevel--;
		}

		protected abstract void write(string str);
		protected abstract void newLine();

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
