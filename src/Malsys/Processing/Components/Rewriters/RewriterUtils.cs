using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;
using System;

namespace Malsys.Processing.Components.Rewriters {
	public static class RewriterUtils {

		public static Dictionary<string, RewriteRule[]> CreateRrulesMap(IEnumerable<RewriteRule> rrules) {

			Contract.Requires<ArgumentNullException>(rrules != null);
			Contract.Ensures(Contract.Result<Dictionary<string, RewriteRule[]>>() != null);

			return rrules
				.GroupBy(rr => rr.SymbolPattern.Name)
				.ToDictionary(g => g.Key, g => g.ToArray());
		}

	}
}
