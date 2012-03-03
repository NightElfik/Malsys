using System;

namespace Malsys.Processing.Components {
	/// <summary>
	/// Adds one or more aliases to member.
	/// Alias can be assigned to any member whose name is used by user in input code.
	/// </summary>
	[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
	public sealed class AliasAttribute : Attribute {

		public readonly string[] Aliases;

		public AliasAttribute(params string[] aliases) {
			Aliases = aliases;
		}

	}
}
