// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using Malsys.Web.Models.Lsystem;

namespace Malsys.Web.Areas.Documentation.Models {
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
			string src = SourceCodeTemplate.FmtInvariant(Args);
			src = src.Replace("; ", ";\n");
			var lines = src.SplitToLines();
			return StringHelper.AppendLinesAutoIndent(' ', 4, lines);
		}


		public LsystemExampleViewModel WithArgs(params object[] args) {
			Args = args;
			return this;
		}
	}
}