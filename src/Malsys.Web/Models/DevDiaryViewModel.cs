using System.Collections.Generic;

namespace Malsys.Web.Models {
	public class DevDiaryViewModel : ArticleModelBase {

		public DevDiaryEntry DisplayedEntry { get; set; }

		public IEnumerable<DevDiaryEntry> AllEntries;

	}
}
