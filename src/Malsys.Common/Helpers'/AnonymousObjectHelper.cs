using System;

namespace Malsys {
	public static class AnonymousObjectHelper {

		public static Nullable<TProperty> GetPropertyStruct<TProperty>(object obj, string name) where TProperty : struct {

			var pi = obj.GetType().GetProperty(name, typeof(TProperty));
			if (pi != null) {
				return (TProperty)pi.GetValue(obj, null);
			}
			else {
				return null;
			}

		}

		public static TProperty GetProperty<TProperty>(object obj, string name) where TProperty : class {

			var pi = obj.GetType().GetProperty(name, typeof(TProperty));
			if (pi != null) {
				return (TProperty)pi.GetValue(obj, null);
			}
			else {
				return null;
			}

		}

	}
}
