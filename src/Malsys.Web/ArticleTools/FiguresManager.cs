using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Web;

namespace Malsys.Web.ArticleTools {
	public class FiguresManager {

		public const string DefaultFigureClass = "figure";
		private int figuresCounter = 1;
		private int chartsCounter = 1;


		public CustomFigureData CustomFigure(string caption) {
			var sb = new StringBuilder();
			string id = getNextId();

			figureBegin(sb, id);
			string startHtml = sb.ToString();

			sb.Clear();
			figureEnd(sb, id, caption);
			string endHtml = sb.ToString();

			return new CustomFigureData(createHtmlId(id), createRefHtml(id, caption), new HtmlString(startHtml), new HtmlString(endHtml));
		}

		public IFigure HtmlFigure(string caption, string html) {
			var sb = new StringBuilder();
			string id = getNextId();
			figureBegin(sb, id);
			sb.Append(html);
			figureEnd(sb, id, caption);

			return new SimpleFigure() {
				Html = new HtmlString(sb.ToString()),
				Ref = createRefHtml(id, caption),
				HtmlId = createHtmlId(id),
			};
		}

		public IFigure HtmlFigure(string caption, HtmlString html) {
			return HtmlFigure(caption, html.ToString());
		}

		public MultiFigureBuilder MultiFigure(string caption) {
			return new MultiFigureBuilder(this, getNextId(), caption);
		}


		//public IFigure Image(string caption, int maxWidth, int maxHeight, UrlHelper url, string imagePath) {
		//	var sb = new StringBuilder();
		//	sb.Append("<div class=\"img\">");
		//	StaticHtml.ThnWithFullLink(sb, caption, new Size(maxWidth, maxHeight), url, imagePath);
		//	sb.Append("</div>");
		//	return HtmlFigure(caption, sb.ToString());
		//}

		public IFigure Image(string caption, int width, int height, string imagePath) {
			var sb = new StringBuilder();
			string cleanAlt = StaticHtml.StripHtmlTagsAndEncode(caption);

			sb.Append("<div class='img'>");
			sb.AppendFormat("<a href='{0}' title='{1}'>", imagePath, cleanAlt);
			sb.AppendFormat("<img src='{0}' alt='{1}' width='{2}' height='{3}' />", imagePath, cleanAlt, width, height);
			sb.Append("</a></div>");
			return HtmlFigure(caption, sb.ToString());
		}

		//public IFigure ImagesStrip(string caption, int height, UrlHelper url, params Tuple<string, string>[] imgAltsAndPaths) {
		//	return ImagesStrip(caption, height, url, (IEnumerable<Tuple<string, string>>)imgAltsAndPaths);
		//}

		//public IFigure ImagesStrip(string caption, int height, UrlHelper url, IEnumerable<Tuple<string, string>> imgAltsAndPaths) {
		//	return HtmlFigure(caption,
		//		StaticHtml.ImagesStrip(height, url, imgAltsAndPaths));
		//}


		private string getNextId() {
			string id = figuresCounter.ToString();
			++figuresCounter;
			return id;
		}

		private string createHtmlId(string id) {
			return "figure-" + id;
		}


		private HtmlString createRefHtml(string id, string caption) {
			return new HtmlString("<a href=\"#{0}\" title=\"Go to figure {1}: {2}\">Figure {1}</a>"
				.Fmt(createHtmlId(id), id, StaticHtml.StripHtmlTagsAndEncode(caption)));
		}

		private void figureBegin(StringBuilder sb, string id, string htmlClass = DefaultFigureClass) {
			sb.Append("<div class=\"");
			sb.Append(htmlClass);
			sb.Append(" galCont\" id=\"");
			sb.Append(createHtmlId(id));
			sb.Append("\">");
		}

		private void figureEnd(StringBuilder sb, string id, string caption) {
			if (caption != null) {
				sb.Append("<h5 class=\"hideLinks\">Figure ");
				sb.Append(id);
				if (caption.Length > 0) {
					sb.Append(": ");
					sb.Append(caption);
				}
				sb.Append("</h5>");
			}
			sb.Append("</div>");
		}


		public class MultiFigureBuilder : IFigure {

			private string id;
			private string caption;
			private FiguresManager parent;

			private int subfiguresCounter = 0;

			private StringBuilder sb = new StringBuilder();


			public MultiFigureBuilder(FiguresManager parent, string id, string caption) {
				this.parent = parent;
				this.id = id;
				this.caption = caption;

				parent.figureBegin(sb, id);
				//sb.Append("<div class=\"clearfix\">");
			}

			public HtmlString Ref {
				get { return parent.createRefHtml(id, caption); }
			}

			public string Caption { get { return caption; } }

			public string HtmlId {
				get { return parent.createHtmlId(id); }
			}

			public HtmlString Html {
				get {
					Debug.Assert(sb.Length > 0, "Only one call of Html supported. Need more? Fix it!");
					parent.figureEnd(sb, id, caption);
					string html = sb.ToString();
					sb.Clear();  // To prevent problems with multiple calls of Html (multiple figure endings).
					return new HtmlString(html);
				}
			}

			public IFigure SubHtml(string caption, string html, int maxWidth = int.MaxValue) {
				string subfigId = getNextId();
				string subfigFullId = createFullSubfigId(subfigId);
				string id = createSubfigHtmlId(subfigId);

				subfigureBegin(sb, subfigId, maxWidth);
				sb.Append(html);
				subfigureEnd(sb, subfigId, caption);

				return new SimpleFigure() {
					Html = null,  // HTML of sub-figures is not printable alone.
					Ref = parent.createRefHtml(subfigFullId, caption),
					HtmlId = parent.createHtmlId(id),
				};
			}

			public IFigure SubHtml(string caption, HtmlString html, int maxWidth = int.MaxValue) {
				return SubHtml(caption, html.ToString(), maxWidth);
			}

			//public IFigure SubImage(string caption, int maxWidth, int maxHeight, UrlHelper url, string imagePath, int subfigureMaxWidth = int.MaxValue) {
			//	var sb = new StringBuilder();
			//	sb.Append("<div class=\"img\">");
			//	string alt = caption.TrimEnd('.') + " — " + this.caption;
			//	StaticHtml.ThnWithFullLink(sb, alt, new Size(maxWidth, maxHeight), url, imagePath);
			//	sb.Append("</div>");
			//	return SubHtml(caption, sb.ToString(), subfigureMaxWidth);
			//}

			public IFigure SubImage(string caption, int width, int height, string imagePath, int subfigureMaxWidth = int.MaxValue) {
				var sb = new StringBuilder();
				string alt = caption == null ? "" : (caption.TrimEnd('.') + " — " + this.caption);
				string cleanAlt = StaticHtml.StripHtmlTagsAndEncode(alt);

				sb.Append("<div class='img'>");
				sb.AppendFormat("<a href='{0}' title='{1}'>", imagePath, cleanAlt);
				sb.AppendFormat("<img src='{0}' alt='{1}' width='{2}' height='{3}' />", imagePath, cleanAlt, width, height);
				sb.Append("</a></div>");
				return SubHtml(caption, sb.ToString(), subfigureMaxWidth);
			}

			private string getNextId() {
				string id = ((char)(subfiguresCounter + 'a')).ToString();
				++subfiguresCounter;
				return id;
			}

			private string createSubfigHtmlId(string subfigId) {
				return parent.createHtmlId(createFullSubfigId(subfigId));
			}

			private string createFullSubfigId(string subfigId) {
				return id + "-" + subfigId;
			}

			private void subfigureBegin(StringBuilder sb, string subfigId, int maxWidth = int.MaxValue) {
				sb.Append("<div class=\"subfigure\" id=\"");
				sb.Append(createSubfigHtmlId(subfigId));
				if (maxWidth != int.MaxValue) {
					sb.Append("\" style=\"max-width: ");
					sb.Append(maxWidth);
					sb.Append("px");
				}
				sb.Append("\">");
			}

			private void subfigureEnd(StringBuilder sb, string subfigId, string caption) {
				if (caption != null) {
					sb.Append("<h6>(");
					sb.Append(subfigId);
					sb.Append(")");
					if (caption.Length > 0) {
						sb.Append(" ");
						sb.Append(caption);
					}
					sb.Append("</h6>");
				}
				sb.Append("</div>");
			}

			public override string ToString() {
				return "DID YOU MEANT TO CALL REF()?";
			}
		}


		public ChartBuilder Chart(int width, int height, string caption, string vAxisCaption, string[] legend,
				bool displayLegend = true, bool logVAxis = false, bool logHAxis = false, string vAxisFormat = null,
				string hAxisFormat = null, double vAxisMinValue = double.NaN, double vAxisMaxValue = double.NaN,
				double vAxisBase = double.NaN, double vAxisGridCount = double.NaN, int extraLeftPerc = 0,
				string chartType = "LineChart") {

			return new ChartBuilder(this, (chartsCounter++).ToString(), new Size(width, height), caption, vAxisCaption, legend) {
				ChartType = chartType,
				DisplayLegend = displayLegend,
				LogVAxis = logVAxis,
				LogHAxis = logHAxis,
				VAxisFormat = vAxisFormat,
				HAxisFormat = hAxisFormat,
				VAxisMinValue = vAxisMinValue,
				VAxisMaxValue = vAxisMaxValue,
				VAxisBase = vAxisBase,
				VAxisGridCount = vAxisGridCount,
				ExtraLeft = extraLeftPerc,
			};
		}

		public class CustomFigureData : IFigure {

			public CustomFigureData(string htmlId, HtmlString refHtml, HtmlString startHtml, HtmlString endHtml) {
				HtmlId = htmlId;
				StartHtml = startHtml;
				EndHtml = endHtml;
				Ref = refHtml;
			}


			public HtmlString StartHtml { get; private set; }

			public HtmlString EndHtml { get; private set; }

			public HtmlString Ref { get; private set; }

			public HtmlString Html {
				get { throw new NotImplementedException(); }
			}

			public string HtmlId { get; private set; }

		}

		public class ChartBuilder : IFigure {

			private const string drawChartFuncArrayVarName = "chartDrawFunctions";
			private const string buildChartFuncArrayVarName = "chartBuildFunctions";

			private FiguresManager parent;
			private string id;
			private Size size;
			private string caption;
			private string vAxisCaption;
			private string[] legend;
			private List<string> captions = new List<string>();
			private List<double[]> data = new List<double[]>();

			public string ChartType = "LineChart";
			public bool DisplayLegend = true;
			public bool LogVAxis = false;
			public bool LogHAxis = false;
			public double VAxisMinValue = double.NaN;
			public double VAxisMaxValue = double.NaN;
			public double VAxisBase = double.NaN;
			public double VAxisGridCount = double.NaN;
			public string VAxisFormat = null;
			public string HAxisFormat = null;
			public int ExtraLeft = 0;

			public ChartBuilder(FiguresManager parent, string id, Size size, string caption,
					string vAxisCaption, string[] legend) {

				this.parent = parent;
				this.id = id;
				this.size = size;
				this.caption = caption;
				this.vAxisCaption = vAxisCaption;
				this.legend = legend;

				//Ref = parent.createRefHtml(id, caption);
				//HtmlId = parent.createHtmlId(id);
				ChartId = createGrahpId(id);
				ChartTableId = ChartId + "-table";

				StaticHtml.RequireScript("https://www.google.com/jsapi");
				StaticHtml.InlineScript(LoadingOrder.Sooner, "var ", drawChartFuncArrayVarName, " = [];", "var ", buildChartFuncArrayVarName, " = [];");
				StaticHtml.InlineScript(LoadingOrder.Later, "google.load(\"visualization\", \"1\", { packages: ['corechart', 'table'] });",
					"google.setOnLoadCallback(function() {",
						"for (var i = 0; i < ", buildChartFuncArrayVarName, ".length; ++i) {",
							buildChartFuncArrayVarName, "[i]();",
							drawChartFuncArrayVarName, "[i]();",
						"}",
					"});",
					"$(window).resize(function() {",
						"for (var i = 0; i < ", drawChartFuncArrayVarName, ".length; ++i) {",
							drawChartFuncArrayVarName, "[i]();",
						"}",
					"});");
			}

			public ChartBuilder AddDataRow(double caption, params double[] values) {
				captions.Add(caption.ToStringInvariant());
				data.Add(values);
				return this;
			}

			public ChartBuilder AddDataRow(string caption, params double[] values) {
				captions.Add("'" + caption + "'");
				data.Add(values);
				return this;
			}

			public HtmlString Ref { get; private set; }

			public string HtmlId { get; private set; }

			public string ChartId { get; private set; }

			public string ChartTableId { get; private set; }

			public HtmlString Html {
				get {
					Debug.Assert(parent != null, "Only one call of Html is currently supported. Need more? Fix it!");
					StaticHtml.InlineScript(generateJs());
					var html = generateHtml();
					parent = null;
					return html;
				}
			}

			private string createGrahpId(string id) {
				return "chart-" + id;
			}

			private HtmlString generateHtml() {
				var sb = new StringBuilder();
				//parent.figureBegin(sb, id);
				sb.AppendFormat("<div class='flipContainer' style='width:{0};height:{1}px;'>",
					size.Width == int.MaxValue ? "100%" : (size.Width + "px"),
					size.Height);
				sb.Append("<div class='flipper' style='width:100%;height:100%'><div class='front' style='width:100%;height:100%'>");
				sb.Append("<span class='flipLink' style='display:none;'>Raw data</span>");
				sb.AppendFormat("<div id='{0}' class='chart' style='width:100%;height:100%'>JavaScript is needed for visualization of this chart.</div>",
					ChartId);
				sb.Append("</div>");
				sb.Append("<div class='back' style='display:none;width:100%;height:100%'>");
				sb.Append("<span class='flipLink'>Back to chart</span>");
				sb.AppendFormat("<div id='{0}' class='chartTable'>JavaScript is needed for displaying of this chart data.</div>",
					ChartTableId);
				sb.Append("</div></div></div>");
				//parent.figureEnd(sb, id, caption);
				return new HtmlString(sb.ToString());
			}

			private string generateJs() {
				var sb = new StringBuilder();

				sb.Append(buildChartFuncArrayVarName);
				sb.Append(".push(function() {");

				sb.Append("var data = google.visualization.arrayToDataTable([[");
				for (int i = 0; i < legend.Length; ++i) {
					if (i != 0) {
						sb.Append(",");
					}
					sb.Append("'");
					sb.Append(legend[i]);
					sb.Append("'");
				}
				sb.Append("]");

				Debug.Assert(captions.Count == data.Count);
				for (int i = 0; i < captions.Count; i++) {
					sb.Append(",[");
					sb.Append(captions[i]);
					foreach (var d in data[i]) {
						sb.Append(",");
						sb.Append(d.ToStringInvariant());
					}
					sb.Append("]");
				}
				sb.Append("]);");  // Close data definition.

				sb.Append("var options = { pointSize: 5, vAxis: { title: '");
				sb.Append(vAxisCaption);
				sb.Append("'");
				if (LogVAxis) {
					sb.Append(", logScale: true");
				}
				if (!string.IsNullOrEmpty(VAxisFormat)) {
					sb.Append(", format: '");
					sb.Append(VAxisFormat);
					sb.Append("'");
				}
				if (!double.IsNaN(VAxisMinValue)) {
					sb.Append(", minValue: ");
					sb.Append(VAxisMinValue);
				}
				if (!double.IsNaN(VAxisBase)) {
					sb.Append(", baseline: ");
					sb.Append(VAxisBase);
				}
				if (!double.IsNaN(VAxisMaxValue)) {
					sb.Append(", maxValue: ");
					sb.Append(VAxisMaxValue);
				}
				if (!double.IsNaN(VAxisGridCount)) {
					sb.Append(", gridlines: { count:");
					sb.Append(VAxisGridCount);
					sb.Append("}");
				}
				sb.Append(" }, hAxis: { title: '");
				sb.Append(legend[0]);
				sb.Append("'");
				if (LogHAxis) {
					sb.Append(", logScale: true");
				}
				if (!string.IsNullOrEmpty(HAxisFormat)) {
					sb.Append(", format: '");
					sb.Append(HAxisFormat);
					sb.Append("'");
				}
				sb.Append(" }, chartArea: { top: '10%', left: '");
				sb.Append(15 + ExtraLeft);
				sb.Append("%', width: '");
				sb.Append(80 - ExtraLeft);
				sb.Append("%' }, legend: { position: '");

				sb.Append(DisplayLegend ? "bottom" : "none");
				sb.Append("'} };");  // Close options definition.

				sb.Append("var tableOptions = { showRowNumber: true };");

				sb.Append("var chart = new google.visualization.");
				sb.Append(ChartType);
				sb.Append("(document.getElementById('");
				sb.Append(ChartId);
				sb.Append("'));");

				sb.Append("var chartTable = new google.visualization.Table(document.getElementById('");
				sb.Append(ChartTableId);
				sb.Append("'));");

				sb.Append(drawChartFuncArrayVarName);
				sb.Append(".push(function() { chart.draw(data, options); chartTable.draw(data, tableOptions) });");

				sb.Append("});");

				return sb.ToString();
			}

			public override string ToString() {
				return "DID YOU MEANT TO CALL REF()?";
			}
		}



	}
}