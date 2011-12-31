using System;
using System.Collections.Generic;

namespace Malsys.Processing {

	public interface IComponentResolver {

		Type ResolveComponent(string name);

	}


	public interface IComponentContainer {

		void RegisterComponent(string name, Type type, bool ignoreConflicts);

		IEnumerable<KeyValuePair<string, Type>> GetAllRegisteredComponents();

	}

}
