// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;

namespace Malsys {
	/// <summary>
	/// Sets access name for marked member.
	/// Member actual name is not included by default.
	/// </summary>
	[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
	public sealed class AccessNameAttribute : Attribute {

		public bool IncludeMemberName { get; set; }

		public string[] Aliases { get; set; }


		public AccessNameAttribute() { }

		public AccessNameAttribute(params string[] aliases) {
			IncludeMemberName = false;
			Aliases = aliases;
		}

		public AccessNameAttribute(bool includeMemberName, params string[] aliases) {
			IncludeMemberName = includeMemberName;
			Aliases = aliases;
		}

	}
}
