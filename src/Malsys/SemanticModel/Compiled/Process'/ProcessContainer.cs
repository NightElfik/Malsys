/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.SemanticModel.Compiled {
	public class ProcessContainer {

		public readonly string Name;
		public readonly string TypeName;
		public readonly string DefaultTypeName;


		public ProcessContainer(string name, string typeName, string defaultTypeName) {

			Name = name;
			TypeName = typeName;
			DefaultTypeName = defaultTypeName;
		}

	}
}
