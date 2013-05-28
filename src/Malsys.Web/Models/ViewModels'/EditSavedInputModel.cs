// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.ComponentModel.DataAnnotations;

namespace Malsys.Web.Models {
	public class EditSavedInputModel {

		[PreventRendering]
		public string UrlId { get; set; }

		[Required]
		[StringLength(32)]
		public string Name { get; set; }

		[StringLength(100)]
		public string Tags { get; set; }

		[Required]
		[StringLength(10000)]
		[DataType(DataType.MultilineText)]
		public string SourceCode { get; set; }

		[StringLength(10000)]
		[DataType(DataType.MultilineText)]
		public string SourceCodeThn { get; set; }

		[Required]
		public bool Publish { get; set; }

		[StringLength(10000)]
		[DataType(DataType.MultilineText)]
		public string Description { get; set; }

	}
}