using System;
using System.Collections.Generic;
using System.Reflection;

namespace Malsys.Reflection {
	public static class TypeExtensions {

		public static IEnumerable<Tuple<FieldInfo, TAttr>> GetFieldsHavingAttr<TAttr>(this Type type, bool inherit = true) where TAttr : class {

			foreach (var fi in type.GetFields()) {
				var attrs = fi.GetCustomAttributes(typeof(TAttr), inherit);
				if (attrs.Length != 1) {
					continue;
				}

				yield return new Tuple<FieldInfo, TAttr>(fi, (TAttr)attrs[0]);
			}
		}

		public static IEnumerable<Tuple<PropertyInfo, TAttr>> GetPropertiesHavingAttr<TAttr>(this Type type, bool inherit = true) where TAttr : class {

			foreach (var pi in type.GetProperties()) {
				var attrs = pi.GetCustomAttributes(typeof(TAttr), inherit);
				if (attrs.Length != 1) {
					continue;
				}

				yield return new Tuple<PropertyInfo, TAttr>(pi, (TAttr)attrs[0]);
			}
		}

		public static IEnumerable<Tuple<MethodInfo, TAttr>> GetMethodsHavingAttr<TAttr>(this Type type, bool inherit = true) where TAttr : class {

			foreach (var mi in type.GetMethods()) {
				var attrs = mi.GetCustomAttributes(typeof(TAttr), inherit);
				if (attrs.Length != 1) {
					continue;
				}

				yield return new Tuple<MethodInfo, TAttr>(mi, (TAttr)attrs[0]);
			}
		}

		public static bool TryGetAttribute<TAttr>(this Type type, out TAttr result, bool inherit = true) where TAttr : class {

			var attrs = type.GetCustomAttributes(typeof(TAttr), inherit);
			if (attrs.Length == 1) {
				result = (TAttr)attrs[0];
				return true;
			}
			else {
				result = null;
				return false;
			}
		}

	}
}
