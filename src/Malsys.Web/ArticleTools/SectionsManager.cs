using System;
using System.Diagnostics.Contracts;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Malsys.Web.ArticleTools {
	public class SectionsManager {

		private readonly Func<Section, UrlHelper, string> hrefResolver;
		private readonly UrlHelper urlHelper;


		public SectionsManager(Func<Section, UrlHelper, string> hrefResolver) {
			this.hrefResolver = hrefResolver;
			RootSection = new Section(this);
			CurrentSectionNumber = -1;
			urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
		}


		public Section RootSection { get; private set; }

		public int CurrentSectionNumber { get; private set; }

		public Section CurrentSection { get; private set; }


		public bool SetDisplayOfSection(int sectionNumber) {
			CurrentSectionNumber = sectionNumber;
			if (sectionNumber < 0 || sectionNumber >= RootSection.SubsectionsCount) {
				return false;
			}

			CurrentSection = RootSection.Subsections[sectionNumber];
			return true;
		}

		public void SetCurrentSection(Section sec) {
			Contract.Requires(sec != null);
			Contract.Ensures(CurrentSectionNumber >= 0 && CurrentSectionNumber < RootSection.Subsections.Count);

			CurrentSection = sec;
			CurrentSectionNumber = RootSection.Subsections.IndexOf(sec);
		}

		public HtmlString TableOfContents(bool twoColumns, int maxLevel) {
			var sb = new StringBuilder();
			if (twoColumns) {
				int nodesCount = RootSection.TotalSubsectionsCount(1, maxLevel);
				sb.Append("<div class=\"grid-50 mobile-grid-100\"><ul class=\"topLevel\">");
				tableOfContents(sb, RootSection, 1, maxLevel, (int)Math.Ceiling(nodesCount / 2.0),
					"</ul></div><div class=\"grid-50 mobile-grid-100\"><ul class=\"topLevel\">");
				sb.Append("</ul></div>");
			}
			else {
				sb.Append("<ul class=\"topLevel\">");
				tableOfContents(sb, RootSection, 1, maxLevel);
				sb.Append("</ul>");
			}
			return new HtmlString(sb.ToString());
		}

		private int tableOfContents(StringBuilder sb, Section section, int currentLevel, int maxLevel,
				int splitAfter = int.MaxValue, string splitDivisionString = null) {

			int sum = 0;
			foreach (var child in section.Subsections) {
				if (sum >= splitAfter) {
					sb.Append(splitDivisionString);
					sum = 0;
				}
				sb.Append("<li");
				if (CurrentSection == child) {
					sb.Append(" class=\"current\"");
				}
				sb.Append(">");
				sb.Append("<a href=\"");
				sb.Append(child.GetHref());
				sb.Append("\" title=\"");
				sb.Append(child.FullName);
				sb.Append("\">");
				sb.Append(child.FullName);
				sb.Append("</a>");
				if (child.Subsections.Count > 0 && currentLevel < maxLevel) {
					sb.Append("<ul>");
					sum += tableOfContents(sb, child, currentLevel + 1, maxLevel);
					sb.Append("</ul>");
				}
				sb.Append("</li>");
				++sum;
			}
			return sum;
		}

		public string GetHref(Section sec) {
			return hrefResolver(sec, urlHelper);
		}

		public HtmlString Navigation(string previousPrefix, string nextPrefix, string homeString) {
			var sb = new StringBuilder();
			sb.Append("<div class=\"grid-100 mobile-grid-100 grid-parent partsNavigation\"><div class=\"grid-33 mobile-grid-100 hideLinks\">");
			if (CurrentSection.IsFirst) {
				sb.Append("&nbsp;");
			}
			else {
				StaticHtml.Link(sb, "« " + previousPrefix + ": " + CurrentSection.PreviousSection.FullName + " «",
					null, GetHref(CurrentSection.PreviousSection), null, false);
			}

			sb.Append("</div><div class=\"grid-33 mobile-grid-100 hideLinks center\">");

			if (CurrentSection.IsFirst) {
				sb.Append("&nbsp;");
			}
			else {
				StaticHtml.Link(sb, homeString, null, GetHref(RootSection.Subsections[0]), null, false);
			}

			sb.Append("</div><div class=\"grid-33 mobile-grid-100 hideLinks right\">");

			if (CurrentSection.IsLast) {
				sb.Append("&nbsp;");
			}
			else {
				StaticHtml.Link(sb, "» " + nextPrefix + ": " + CurrentSection.NextSection.FullName + " »",
					null, GetHref(CurrentSection.NextSection), null, false);
			}

			sb.Append("</div></div>");

			return new HtmlString(sb.ToString());
		}
	}
}
