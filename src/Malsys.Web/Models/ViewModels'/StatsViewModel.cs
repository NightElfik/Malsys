using System.Collections.Generic;

namespace Malsys.Web.Models {
	public class StatsViewModel {

		public IEnumerable<KeyValuePair<string, int>> ProcessHistByMonth { get; set; }

		public IEnumerable<KeyValuePair<string, int>> SavedLsysByMonth { get; set; }

	}
}