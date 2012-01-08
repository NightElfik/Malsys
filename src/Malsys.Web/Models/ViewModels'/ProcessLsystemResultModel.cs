﻿using System.Collections.Generic;
using Malsys.Processing.Output;

namespace Malsys.Web.Models {
	public class ProcessLsystemResultModel {

		public string SourceCode { get; set; }

		public int? ReferenceId { get; set; }

		public MessageLogger Logger { get; set; }

		public IEnumerable<OutputFile> OutputFiles { get; set; }

		public string SavedInputId { get; set; }

	}
}