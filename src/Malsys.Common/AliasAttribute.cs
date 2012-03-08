using System;

namespace Malsys {
	/// <summary>
	/// Adds one or more aliases to member.
	/// </summary>
	[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
	public sealed class AliasAttribute : Attribute {

		public bool IgnoreMemberName { get; set; }

		public string[] Aliases { get; set; }


		public AliasAttribute() { }

		public AliasAttribute(params string[] aliases) {
			IgnoreMemberName = false;
			Aliases = aliases;
		}

		public AliasAttribute(bool ignoreMemberName, params string[] aliases) {
			IgnoreMemberName = ignoreMemberName;
			Aliases = aliases;
		}

	}
}
