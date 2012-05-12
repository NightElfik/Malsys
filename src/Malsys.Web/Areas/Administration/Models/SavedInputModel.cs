/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;

namespace Malsys.Web.Areas.Administration.Models {
	public class SavedInputModel {

		public int SavedInputId { get; set; }
		public string UrlId { get; set; }
		public string User { get; set; }
		public DateTime EditDate { get; set; }
		public long Duration { get; set; }
		public string SourceCode { get; set; }

	}
}