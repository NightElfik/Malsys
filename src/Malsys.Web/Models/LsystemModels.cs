using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Malsys.Processing;

namespace Malsys.Web.Models {

	public class ProcessLsystemModel {

		[Display(Name = "Source code")]
		public string SourceCode { get; set; }

	}

	public class ProcessLsystemResultModel : ProcessLsystemModel {

		public MessagesCollection Messages { get; set; }

		public IEnumerable<OutputFile> OutputFiles { get; set; }

	}
}