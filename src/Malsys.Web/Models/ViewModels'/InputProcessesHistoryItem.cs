using System;

namespace Malsys.Web.Models {
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