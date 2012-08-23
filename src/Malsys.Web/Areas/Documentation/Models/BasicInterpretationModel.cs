/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using Malsys.Reflection.Components;
using Malsys.SemanticModel.Evaluated;
using Malsys.Web.Models.Lsystem;

namespace Malsys.Web.Areas.Documentation.Models {
	public class BasicInterpretationModel {

		public SimpleLsystemProcessor SimpleLsystemProcessor { get; set; }

		public InputBlockEvaled StdLib { get; set; }

		public ComponentMetadata Interpreter { get; set; }

	}
}