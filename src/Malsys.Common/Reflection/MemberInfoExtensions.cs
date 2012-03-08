using System.Collections.Generic;
using System.Reflection;

namespace Malsys.Reflection {
	public static class MemberInfoExtensions {

		public static IEnumerable<string> GetAliases(this MemberInfo memberInfo, bool inherit = true) {

			bool ignoreMemberName = false;

			foreach (AliasAttribute aliasAttr in  memberInfo.GetCustomAttributes(typeof(AliasAttribute), inherit)) {
				ignoreMemberName |= aliasAttr.IgnoreMemberName;
				foreach (var alias in aliasAttr.Aliases) {
					yield return alias;
				}
			}

			if (!ignoreMemberName) {
				yield return memberInfo.Name;
			}

		}

	}
}
