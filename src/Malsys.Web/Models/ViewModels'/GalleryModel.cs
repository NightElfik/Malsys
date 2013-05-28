// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using Malsys.Web.Entities;
using MvcContrib.Pagination;

namespace Malsys.Web.Models {
	public class GalleryModel {

		public IPagination<SavedInput> Inputs;

		public string UserFilter;

		public Tag TagFilter;

	}
}