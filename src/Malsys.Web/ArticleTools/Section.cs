using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Malsys.Web.ArticleTools {
	public class Section : IFigure {

		private static readonly Regex nonAsciiRegex = new Regex("[^a-z0-9]", RegexOptions.Compiled);
		private static readonly Regex multiDash = new Regex("[-]+", RegexOptions.Compiled);

		private SectionsManager parentMgr;
		private int sectionCounter = 1;
		private List<Section> childSections = new List<Section>();

		public Section(SectionsManager parentMgr) {
			this.parentMgr = parentMgr;
			Level = 1;
		}

		/// <summary>
		/// One-based incremental Id.
		/// </summary>
		public string Id { get; private set; }

		/// <summary>
		/// One-based level (depth) of this section.
		/// </summary>
		public int Level { get; private set; }

		/// <summary>
		/// Cumulative dot-separated Ids.
		/// </summary>
		public string CumulativeId { get; private set; }

		/// <summary>
		/// Name of this section.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Name of this section including the number.
		/// </summary>
		public string FullName { get { return CumulativeId + " " + Name; } }

		/// <summary>
		/// Returns string "Chapter" for base level sections and "Section" otherwise.
		/// </summary>
		public string SectionBaseName { get { return Level == 2 ? "Chapter" : "Section"; } }

		/// <summary>
		/// Returns SectionBaseName + FullName.
		/// </summary>
		public string SectionFullName { get { return SectionBaseName + " " + CumulativeId + ": " + Name; } }

		/// <summary>
		/// Returns list of subsections of this section.
		/// </summary>
		public List<Section> Subsections { get { return childSections; } }

		/// <summary>
		/// Returns true if this section should be printed.
		/// Should be used only on main sections, not subsections.
		/// </summary>
		public bool IsVisible { get { return parentMgr.CurrentSection == this; } }

		/// <summary>
		/// Returns HTML code for reference on this section.
		/// </summary>
		public HtmlString Ref {
			get {
				return new HtmlString("<a href=\"{0}\" title=\"Go to {3} {1}: {2}\">{3} {1} {2}</a>"
					.Fmt(GetHref(), CumulativeId, Name, SectionBaseName));
			}
		}

		/// <summary>
		/// Returns HTML code for reference on this section with only name.
		/// </summary>
		public HtmlString BareRef {
			get {
				return new HtmlString("<a href=\"{0}\" title=\"{1}\">{1}</a>"
					.Fmt(GetHref(), Name));
			}
		}

		/// <summary>
		/// Returns HTML ID of this section.
		/// </summary>
		public string HtmlId { get; private set; }

		/// <summary>
		/// Returns parent section or null of no parent section exists.
		/// </summary>
		public Section Parent { get; private set; }

		public Section PreviousSection { get; private set; }

		public Section NextSection { get; private set; }

		public bool IsFirst { get { return PreviousSection == null; } }
		public bool IsLast { get { return NextSection == null; } }

		/// <summary>
		/// Returns HTML for this section.
		/// </summary>
		public HtmlString Html {
			get {
				return new HtmlString("<h{0} id='{1}' class='section'>{2} {3}</h{0}>"
					.Fmt(Level, HtmlId, CumulativeId, Name));
			}
		}

		/// <summary>
		/// Returns HTML for this section without the section number.
		/// </summary>
		public HtmlString PlainHtml {
			get {
				return new HtmlString("<h{0} id='{1}' class='section'>{2}</h{0}>"
					.Fmt(Level, HtmlId, Name));
			}
		}


		public string GetHref() {
			return parentMgr.GetHref(this);
		}
		/// <summary>
		/// Returns count of subsections of this section.
		/// </summary>
		public int SubsectionsCount { get { return childSections.Count; } }

		/// <summary>
		/// Returns total count of all subsections (including sub-sub sections etc.).
		/// </summary>
		public int TotalSubsectionsCount(int level, int maxLevel) {
			int sum = childSections.Count;
			if (level < maxLevel) {
				foreach (var child in childSections) {
					sum += child.TotalSubsectionsCount(level + 1, maxLevel);
				}
			}
			return sum;
		}

		/// <summary>
		/// Creates sub-section of this section.
		/// </summary>
		public Section Subsection(string name) {
			var existing = childSections.FirstOrDefault(x => x.Name == name);
			if (existing != null) {
				return existing;
			}

			string id = getNextId();
			string cumulId = (this.CumulativeId == null ? id : this.CumulativeId + "." + id);
			var sec = new Section(parentMgr) {
				Id = id,
				Level = this.Level + 1,
				CumulativeId = cumulId,
				Name = name,
				HtmlId = StaticUrl.UrlizeString(cumulId + "-" + name),
				Parent = this,
				PreviousSection = childSections.LastOrDefault(),
			};
			if (childSections.Count > 0) {
				childSections.Last().NextSection = sec;
			}
			childSections.Add(sec);
			return sec;
		}

		private string getNextId() {
			string id = sectionCounter.ToString();
			++sectionCounter;
			return id;
		}

		public override string ToString() {
			return "DID YOU MEANT TO CALL REF()?";
		}

	}
}