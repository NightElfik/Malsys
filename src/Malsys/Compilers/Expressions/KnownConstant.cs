using System;

namespace Malsys.Compilers.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class KnownConstant {


		public readonly string Name;
		public readonly double Value;

		public readonly string LongName;
		public readonly string Group;


		public KnownConstant(string longName, string group, string name, double value) {

			Name = name;
			Value = value;

			LongName = longName;
			Group = group;

		}


		public KnownConstant ChangeNameTo(string newName) {
			return new KnownConstant(LongName, Group, newName, Value);
		}
	}
}
