using System.Text;
using System.IO;

namespace Malsys.IO {
	public class IndentTextWriter : IndentWriter {

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
	}
}
