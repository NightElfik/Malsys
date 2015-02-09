using System.Collections.Generic;
using Malsys.Processing;
using Malsys.Web.Models;
using Malsys.Web.Models.Lsystem;

namespace Malsys.Web.Areas.Documentation.Models {
	public class HelpArticleViewModel : ArticleModelBase {

		public HelpArticle DisplayedArticle { get; set; }

		public IEnumerable<HelpArticle> AllArticles;


		public SimpleLsystemProcessor LsystemProcessor;

		public ProcessManager ProcessManager;

	}
}
