using System.ComponentModel.DataAnnotations;

namespace Malsys.Web.Models {
	public class NewDiscusThreadModel {

		[Required]
		public int CategoryId { get; set; }

		[Required]
		[StringLength(64, MinimumLength = 4)]
		[Display(Name = "Thread title")]
		public string Title { get; set; }

		[StringLength(32, MinimumLength = 4)]
		[Display(Name = "Author name")]
		public string AuthorNameNonRegistered { get; set; }

		[Required]
		[StringLength(4096, MinimumLength = 2)]
		public string FirstMessage { get; set; }

	}
}