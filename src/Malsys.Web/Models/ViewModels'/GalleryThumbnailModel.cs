// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using Malsys.Web.Entities;

namespace Malsys.Web.Models {
	public class GalleryThumbnailModel {

		public SavedInput SavedInput;

		public int MaxWidth;

		public int MaxHeight;

		/// <summary>
		/// This member is set by partial view and should be read by caller a include Three.js.
		/// </summary>
		public bool NeedsThreeJs;

	}
}