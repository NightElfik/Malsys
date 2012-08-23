using System.ComponentModel.DataAnnotations;

namespace Malsys.Web.Models {
	public class NewDiscusMessageModel {

		[Required]
		public int ThreadId { get; set; }

		[Required]
		[StringLength(4096, MinimumLength = 2)]
		public string Message { get; set; }

		[StringLength(32, MinimumLength = 4)]
		[Display(Name = "Author name")]
		public string AuthorNameNonRegistered { get; set; }

	}
}