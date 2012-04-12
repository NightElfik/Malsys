using System.Reflection;

namespace Malsys.Compilers {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class CompilerConstant {

		public readonly string Name;
		public readonly double Value;

		public readonly string SummaryDoc;
		public readonly string GroupDoc;


		public CompilerConstant(string name, double value, string summaryDoc, string groupDoc) {
			Name = name;
			Value = value;
			SummaryDoc = summaryDoc;
			GroupDoc = groupDoc;
		}


	}
}
