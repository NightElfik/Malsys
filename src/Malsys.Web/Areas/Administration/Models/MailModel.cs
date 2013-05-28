// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;

namespace Malsys.Web.Areas.Administration.Models {
	public class MailModel {

		[Email]
		[Required]
		public string Address { get; set; }

		[Required]
		[StringLength(100)]
		public string Subject { get; set; }

		[DataType(DataType.MultilineText)]
		public string Body { get; set; }

	}
}