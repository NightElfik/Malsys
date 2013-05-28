// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;

namespace Malsys.Web.Areas.Administration.Models {
	public class InputProcessesHistoryItem {

		public int ProcessId { get; set; }
		public int? ParentProcessId { get; set; }
		public string User { get; set; }
		public DateTime Date { get; set; }
		public long Duration { get; set; }
		public int SourceId { get; set; }
		public string Source { get; set; }

	}
}