/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using System.Collections.Generic;
using Malsys.Processing.Output;

namespace Malsys.Web.Models {
	public class ProcessLsystemResultModel {

		public bool IsEmpty { get; set; }

		public bool NoProcessStatement { get; set; }

		public string SourceCode { get; set; }

		public string CompiledSourceCode { get; set; }

		public int? ReferenceId { get; set; }

		public MessageLogger Logger { get; set; }

		public List<OutputFile> OutputFiles { get; set; }

		public string SavedInputUrlId { get; set; }

		public TimeSpan MaxProcessDuration { get; set; }

		public TimeSpan ProcessDuration { get; set; }

		public string[] UsedProcessConfigurationsNames { get; set; }

	}
}