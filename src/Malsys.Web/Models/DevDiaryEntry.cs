using System;

namespace Malsys.Web.Models {
	public class DevDiaryEntry {
		public string Name { get; private set; }
		public string Url { get; private set; }
		public string DisqusId { get; private set; }
		public DateTime Date { get; private set; }
		public string ViewName { get; private set; }


		public static readonly DevDiaryEntry SvgOptimizations = new DevDiaryEntry() {
			Name = "SVG markup optimizations",
			Url = "SVG-markup-optimizations",
			//DisqusId = "",
			Date = new DateTime(2013, 5, 1),
			ViewName = MVC.DevDiary.Views.Entries.SvgOptimizations,
		};

		public static readonly DevDiaryEntry PngAnimationRenderer = new DevDiaryEntry() {
			Name = "Creation of PNG animation renderer",
			Url = "Creation-of-PNG-animation-renderer",
			//DisqusId = "",
			Date = new DateTime(2012, 7, 1),
			ViewName = MVC.DevDiary.Views.Entries.PngAnimationRenderer,
		};

	}
}