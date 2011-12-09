using System;

namespace Malsys.Processing {

	public interface IComponentResolver {

		Type ResolveComponent(string name);

	}


	public interface IComponentContainer {

		void RegisterComponent(string name, Type type, bool replaceIfExists);

	}

}
