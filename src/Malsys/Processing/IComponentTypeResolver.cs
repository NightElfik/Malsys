using System;
using System.Collections.Generic;

namespace Malsys.Processing {

	public interface IComponentTypeResolver {

		Type ResolveComponentType(string name);

	}


	public interface IComponentTypeContainer {

		void RegisterComponentType(string name, Type type, bool ignoreConflicts);

		IEnumerable<KeyValuePair<string, Type>> GetAllRegisteredComponentTypes();

	}

}
