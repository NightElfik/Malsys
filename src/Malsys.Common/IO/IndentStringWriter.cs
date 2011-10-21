using System.Text;

namespace Malsys.IO {
	public class IndentStringWriter : IndentTextWriter {

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


		protected override void write(string str) {
			strBuilder.Append(str);
		}

		protected override void newLine() {
			strBuilder.AppendLine();
		}
	}
}
