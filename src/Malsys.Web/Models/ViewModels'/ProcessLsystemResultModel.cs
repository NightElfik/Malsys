using System.Collections.Generic;
using Malsys.Processing.Output;
using System;

namespace Malsys.Web.Models {
	public class ProcessLsystemResultModel {

		public bool NoProcessStatement { get; set; }

		public string SourceCode { get; set; }

		public int? ReferenceId { get; set; }

		public MessageLogger Logger { get; set; }

		public List<OutputFile> OutputFiles { get; set; }

		public string SavedInputId { get; set; }

		public TimeSpan ProcessDuration { get; set; }

	}
}