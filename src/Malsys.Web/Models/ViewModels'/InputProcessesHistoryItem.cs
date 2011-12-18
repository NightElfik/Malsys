using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Malsys.Web.Models {
	public class InputProcessesHistoryItem {

		public int ProcessId { get; set; }
		public string User { get; set; }
		public DateTime Date { get; set; }
		public int SourceId { get; set; }
		public string Source { get; set; }

	}
}