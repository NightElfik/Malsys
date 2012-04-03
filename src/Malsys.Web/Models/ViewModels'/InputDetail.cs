using System.Collections.Generic;

namespace Malsys.Web.Models {
	public class InputDetail {

		public string UrlId { get; set; }

		public string Name { get; set; }

		public string AuthorName { get; set; }

		public bool CurrentUserIsOwner { get; set; }

		public bool IsPublished { get; set; }

		public int Rating { get; set; }

		public bool? UserVote { get; set; }

		public string[] Tags { get; set; }

		public string SourceCode { get; set; }

		public string FilePath { get; set; }

		public string MimeType { get; set; }

		public string Description { get; set; }

		public KeyValuePair<string, object>[] Metadata { get; set; }

	}
}