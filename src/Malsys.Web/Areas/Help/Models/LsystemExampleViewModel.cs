using Malsys.Web.Models.Lsystem;

namespace Malsys.Web.Areas.Help.Models {
	public class LsystemExampleViewModel {

		public SimpleLsystemProcessor LsystemProcessor { get; set; }

		public string SourceCodeTemplate { get; set; }

		public int[] UnimportantLines { get; set; }

		public object[] Args { get; set; }


		public LsystemExampleViewModel() {
			UnimportantLines = new int[0];
			Args = new object[0];
		}


		public string SourceCodeWithArgsAutoIndented() {

			var lines = SourceCodeTemplate.FmtInvariant(Args).SplitToLines();

			return StringHelper.AppendLinesAutoIndent(' ', 4, lines);
		}


		public LsystemExampleViewModel WithArgs(params object[] args) {
			Args = args;
			return this;
		}
	}
}