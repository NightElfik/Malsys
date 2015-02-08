using System.Collections.Generic;
using Malsys.Compilers;
using Malsys.Evaluators;

namespace Malsys.Web.Areas.Documentation.Models {
	public class PredefinedConstantsModel {

		public IEnumerable<CompilerConstant> CompilerConstants { get; set; }

		public IEnumerable<VariableInfo> StdLibConstants { get; set; }

	}
}