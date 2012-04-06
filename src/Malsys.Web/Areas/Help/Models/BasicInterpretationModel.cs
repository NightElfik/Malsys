using Malsys.Reflection.Components;
using Malsys.SemanticModel.Evaluated;
using Malsys.Web.Models.Lsystem;

namespace Malsys.Web.Areas.Help.Models {
	public class BasicInterpretationModel {

		public SimpleLsystemProcessor SimpleLsystemProcessor { get; set; }

		public InputBlockEvaled StdLib { get; set; }

		public ComponentMetadata Interpreter { get; set; }

	}
}