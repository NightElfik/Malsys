// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Collections.Generic;
using System.Reflection;

namespace Malsys.Reflection {
	public static class MemberInfoExtensions {

		/// <summary>
		/// Returns access names for this member.
		/// If member is marked by AccessNameAttribute the names from it are returned.
		/// Otherwise actual member name is returned.
		/// </summary>
		public static IEnumerable<string> GetAccessNames(this MemberInfo memberInfo, bool inherit = false) {

			bool includeMemberName = true;

			foreach (AccessNameAttribute aliasAttr in memberInfo.GetCustomAttributes(typeof(AccessNameAttribute), false)) {
				includeMemberName &= aliasAttr.IncludeMemberName;
				foreach (var alias in aliasAttr.Aliases) {
					yield return alias;
				}
			}

			if (includeMemberName) {
				yield return memberInfo.Name;
			}

		}

	}
}
