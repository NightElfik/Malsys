using System.Collections.Generic;
using System.Web.Mvc;
using Malsys.Processing;

namespace Malsys.Web.Models {
	public class ProcessLsystemResultModel {

		public string SourceCode { get; set; }

		[HiddenInput(DisplayValue = false)]
		public int? ReferenceId { get; set; }

		public MessageLogger Logger { get; set; }

		public IEnumerable<OutputFile> OutputFiles { get; set; }

	}
}