/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.SemanticModel.Compiled {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ProcessComponent {

		public readonly string Name;
		public readonly string TypeName;


		public ProcessComponent(string name, string typeName) {

			Name = name;
			TypeName = typeName;
		}

	}
}
