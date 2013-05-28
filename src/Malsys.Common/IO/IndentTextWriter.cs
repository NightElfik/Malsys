using System;
// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.IO;

namespace Malsys.IO {
	/// <summary>
	/// Implementation of the IndentWriter that writes to the TextWriter.
	/// </summary>
	public class IndentTextWriter : IndentWriter, IDisposable {

		private TextWriter writer;


		public IndentTextWriter(TextWriter tw) {
			writer = tw;
		}


		protected override void write(string str) {
			writer.Write(str);
		}

		protected override void newLine() {
			writer.WriteLine();
		}

		public void Close() {
			writer.Close();
		}

		public void Dispose() {
			writer.Dispose();
		}

	}
}
