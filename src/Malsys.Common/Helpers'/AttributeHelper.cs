// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys {
	public static class AttributeHelper {

		public static TAttr GetAttribute<TAttr>(object obj, bool inherit = false) where TAttr : class {

			var attrs = obj.GetType().GetCustomAttributes(typeof(TAttr), inherit);
			if (attrs.Length > 0) {
				return ((TAttr)attrs[0]);
			}
			else {
				return null;
			}

		}

	}
}
