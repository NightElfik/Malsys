using System.Collections.Generic;
using System.Linq;

namespace Malsys.Rewriters {
	public static class RewriterUtils {

		public static Dictionary<string, RewriteRule[]> CreateRrulesMap(ImmutableList<RewriteRule> rrules) {

			return rrules
				.GroupBy(rr => rr.SymbolPattern.Name)
				.ToDictionary(g => g.Key, g => g.ToArray());
		}

	}
}
