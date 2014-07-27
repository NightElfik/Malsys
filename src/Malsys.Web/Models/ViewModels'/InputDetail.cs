// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Collections.Generic;
using Malsys.Web.Entities;

namespace Malsys.Web.Models {
	public class InputDetail {

		public SavedInput Input { get; set; }

		public bool IsAuthor { get; set; }

		public bool CanEdit { get; set; }

		public int? UserVote { get; set; }

		public string FilePath { get; set; }

		public string ThnFilePath { get; set; }

		public KeyValuePair<string, object>[] Metadata { get; set; }

	}
}