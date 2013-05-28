// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Text;

namespace Malsys.IO {
	/// <summary>
	/// Implementation of the IndentWriter that writes to the string.
	/// </summary>
	public class IndentStringWriter : IndentWriter {

		private StringBuilder strBuilder;


		public IndentStringWriter() {
			strBuilder = new StringBuilder();
		}

		public IndentStringWriter(StringBuilder sb) {
			strBuilder = sb;
		}


		public string GetResult() {
			return strBuilder.ToString();
		}

		public void Clear() {
			strBuilder.Clear();
		}

		public string GetResultAndClear() {
			string result = strBuilder.ToString();
			strBuilder.Clear();
			return result;
		}


		protected override void write(string str) {
			strBuilder.Append(str);
		}

		protected override void newLine() {
			strBuilder.AppendLine();
		}
	}
}
