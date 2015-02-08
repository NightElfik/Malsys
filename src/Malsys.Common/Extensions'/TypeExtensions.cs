using System;
using System.Linq;

namespace Malsys {
	public static class TypeExtensions {

		/// <summary>
		/// Returns generic type name in C# syntax.
		/// </summary>
		/// <remarks>
		/// Property FullName should return name of the Type, including the namespace of the Type but not the assembly.
		/// http://msdn.microsoft.com/en-us/library/system.type.aspx
		/// BUT <code>typeof(List{string})</code> returns type with assembly.
		/// This is probably BUG in .NET because documentation and behavior is not the same.
		/// </remarks>
		public static string GetPrettyGenericName(this Type type) {

			if (type.GetGenericArguments().Length == 0) {
				return type.Name;
			}

			var genericArguments = type.GetGenericArguments();
			var typeDefeninition = type.Name;
			var unmangledName = typeDefeninition.Substring(0, typeDefeninition.IndexOf("`"));
			return unmangledName + "<" + String.Join(",", genericArguments.Select(GetPrettyGenericName)) + ">";

		}

	}
}
