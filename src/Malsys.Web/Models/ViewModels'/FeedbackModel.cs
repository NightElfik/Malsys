using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;

namespace Malsys.Web.Models {
	public class FeedbackModel {

		[Required]
		[StringLength(100)]
		public string Subject { get; set; }

		[Email]
		[Display(Name = "E-mail (optional)")]
		public string Email { get; set; }

		[Required]
		[StringLength(10000)]
		[DataType(DataType.MultilineText)]
		public string Message { get; set; }

	}
}
