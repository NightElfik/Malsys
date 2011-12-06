using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Malsys.Processing;

namespace Malsys.Web.Models {
	public class ProcessLsystemResultModel {

		public string SourceCode { get; set; }

		public MessageLogger Messages { get; set; }

		public IEnumerable<OutputFile> OutputFiles { get; set; }

	}
}