using System;

namespace Malsys.Web.Areas.Documentation.Models {
	public class HelpArticle {
		public string Name { get; private set; }
		public string Url { get; private set; }
		public string DisqusId { get; private set; }
		public DateTime Date { get; private set; }
		public string ViewName { get; private set; }


		public static readonly HelpArticle Basics = new HelpArticle() {
			Name = "Introduction and basics",
			Url = "Introduction-and-basics",
			//DisqusId = "IntroductionAndBasics",
			Date = new DateTime(2014, 6, 16),
			ViewName = MVC.Documentation.Articles.Views.Articles.IntroductionAndBasics,
		};

	}
}