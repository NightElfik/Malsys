/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Malsys {
	public static class EnumHelper {
		/// <summary>
		/// Returns string value from StringValueAttribute of given enum value.
		/// </summary>
		public static string GetStringVal<T>(T enumValue) where T : struct {

			Contract.Requires<ArgumentException>(typeof(T).IsEnum, "T must be an enumerated type.");

			var member = typeof(T).GetField(enumValue.ToString());
			if (member == null) {
				Debug.Fail("T enumValue; typeof(T).GetField(enumValue.ToString()) returned null.");
				return null;
			}

			var attrs = member.GetCustomAttributes(typeof(StringValueAttribute), false);
			if (attrs.Length > 0) {
				return ((StringValueAttribute)attrs[0]).String;
			}
			else {
				return null;
			}

		}

		public static TAttr GetAttrFromEnumVal<TEnum, TAttr>(TEnum enumValue)
			where TEnum : struct
			where TAttr : class {

			Contract.Requires<ArgumentException>(typeof(TEnum).IsEnum, "T must be an enumerated type.");

			var member = typeof(TEnum).GetField(enumValue.ToString());
			if (member == null) {
				Debug.Fail("Value `{0}` is not value of `{1}` enumeration.".Fmt(enumValue, typeof(TEnum).Name));
				return null;
			}

			var attrs = member.GetCustomAttributes(typeof(TAttr), false);
			if (attrs.Length > 0) {
				return (TAttr)attrs[0];
			}
			else {
				return null;
			}

		}
	}
}
