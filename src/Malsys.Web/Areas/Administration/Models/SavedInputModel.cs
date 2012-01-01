using System;

namespace Malsys.Web.Areas.Administration.Models {
	public class SavedInputModel {

		public int SavedInputId { get; set; }
		public string RandomId { get; set; }
		public string User { get; set; }
		public DateTime Date { get; set; }
		public long Duration { get; set; }
		public string Source { get; set; }

	}
}