using System.Diagnostics;

namespace Malsys {
	public static class EnumHelper {
		/// <summary>
		/// Returns string value from StringValueAttribute of given enum value.
		/// </summary>
		public static string GetStringVal<T>(T enumValue) where T : struct {

			Debug.Assert(typeof(T).IsEnum, "T must be an enumerated type.");

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
	}
}
