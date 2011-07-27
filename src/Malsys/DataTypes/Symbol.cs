using System.Collections.Generic;

namespace Malsys {
	public class Symbol<T> {
		public readonly string Name;
		public readonly ImmutableList<T> Arguments;


		public Symbol(string name) {
			Name = name;
			Arguments = ImmutableList<T>.Empty;
		}

		public Symbol(string name, IEnumerable<T> args) {
			Name = name;
			Arguments = new ImmutableList<T>(args);
		}

		public Symbol(string name, ImmutableList<T> args) {
			Name = name;
			Arguments = args;
		}
	}
}
