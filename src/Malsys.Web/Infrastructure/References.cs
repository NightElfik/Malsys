using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Malsys.Web.Infrastructure {
	public class References {

		private Dictionary<string, Reference> references = new Dictionary<string, Reference>();


		public HtmlString Cite(string key, HtmlString text = null) {
			return Cite(key, text.ToString());
		}

		public HtmlString Cite(string key, string text = null) {
			if (!references.ContainsKey(key)) {
				references.Add(key, new Reference(key) { Text = text });
			}

			return new HtmlString("<sup class=\"reference\"><a href=\"#cite_{0}\">[{0}]</a></sup>".Fmt(key));
		}

		public HtmlString PrintReferences() {

			var sb = new StringBuilder();

			sb.AppendLine("<ul class=\"referecnesList\">");

			foreach (var re in references) {
				sb.AppendFormat("<li id=\"cite_{0}\">[{0}] {1}</li>", re.Value.Key, re.Value.Text);
				sb.AppendLine();
			}

			sb.AppendLine("</ul>");

			return new HtmlString(sb.ToString());
		}




		public class Reference {

			public readonly string Key;

			public string Text;


			public Reference(string key) {
				Key = key;
			}

		}

	}
}