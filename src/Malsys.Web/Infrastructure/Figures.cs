﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace Malsys.Web.Infrastructure {
	public class Figures {

		private Dictionary<string, string> figures = new Dictionary<string, string>();

		private int figureNumber = 1;

		public HtmlString Image(string id, string imgSrc, int width, int height, string caption, FigureFloat figureFloat = FigureFloat.None) {

			if (figures.ContainsKey(id)) {
				return new HtmlString("<b>Figure with id `{0}` already exists.</b>");
			}

			string figNum = getNextFigNum();
			figures.Add(id, figNum);

			return new HtmlString(
				StringHelper.JoinLines(
					"<div class=\"figure{6}\" id=\"{5}\">",
					"<img src=\"{0}\" width=\"{1}px\" height=\"{2}px\" alt=\"{3}\" />",
					"<p>Figure {4}: {3}</p>",
					"</div>")
				.Fmt(imgSrc, width, height, caption, figNum, getFigNumId(figNum),
					figureFloat == FigureFloat.None ? "" : (figureFloat == FigureFloat.Left ? " left" : " right")));

		}

		public SubFigureBuilder SubFigure(string id = null, string caption = null) {
			return new SubFigureBuilder(this, id, getNextFigNum(), caption);
		}

		public HtmlString Ref(string id, bool shorted = false) {

			string figNum;
			if (!figures.TryGetValue(id, out figNum)) {
				return new HtmlString("<b>Figure with id `{0}` not found exists.</b>");
			}

			return new HtmlString("<a href=\"#{0}\" title=\"Go to Figure {2}\">{1} {2}</a>".Fmt(getFigNumId(figNum), shorted ? "Fig." : "Figure", figNum));
		}


		private string getNextFigNum() {
			return (figureNumber++).ToString();
		}


		private static string getFigNumId(string figNum) {
			return "figure-" + figNum;
		}


		public class SubFigureBuilder {

			private char subFigureNumber = 'a';

			private StringBuilder sb = new StringBuilder();
			private Figures parent;

			private string figId;
			private string figNum;
			private string subFigCaption;


			public SubFigureBuilder(Figures parentFig, string id, string figureNum, string caption) {

				figId = id;
				figNum = figureNum;
				subFigCaption = caption;
				parent = parentFig;

				if (figId != null) {
					if (parent.figures.ContainsKey(figId)) {
						sb.AppendLine("<b>Figure with id `{0}` already exists.</b>");
					}
					parent.figures.Add(figId, figNum);
				}

				sb.AppendLine("<div class=\"figure\" id=\"{0}\"><div class=\"clearfix\">".Fmt(Figures.getFigNumId(figNum)));
			}

			public static implicit operator HtmlString(SubFigureBuilder builder) {
				return builder.ToHtml();
			}


			public SubFigureBuilder SubImage(string id, string imgSrc, int width, int height, string caption = null, int floatHeight = -1) {

				string subfigNum = getNextSubFigNum();
				string wholeSubfigNum = figNum + subfigNum;

				if (id != null) {
					if (parent.figures.ContainsKey(id)) {
						sb.AppendLine("<b>Figure with id `{0}` already exists.</b>");
						return this;
					}
					parent.figures.Add(id, wholeSubfigNum);
				}

				bool compensateHeight = floatHeight > height;

				sb.AppendLine("<div class=\"subfigure\" id=\"{0}\" style=\"width:{1}px;\">".Fmt(Figures.getFigNumId(wholeSubfigNum), width));
				if (compensateHeight) {
					sb.AppendLine("<div style=\"margin:{0}px 0;\">".Fmt((floatHeight - height) / 2));
				}
				sb.AppendLine("<img src=\"{0}\" width=\"{1}px\" height=\"{2}px\" alt=\"{3}\" />".Fmt(imgSrc, width, height, caption ?? "")); if (compensateHeight) {
					sb.AppendLine("</div>");
				}
				sb.AppendLine("<p>({0}) {1}</p>".Fmt(subfigNum, caption ?? ""));
				sb.AppendLine("</div>");

				return this;
			}


			private string getNextSubFigNum() {
				return (subFigureNumber++).ToString();
			}

			public HtmlString ToHtml() {
				string contents = sb.ToString();
				contents += "</div><p>Figure {0}{1}{2}</p></div>".Fmt(figNum, subFigCaption != null ? ": " : "", subFigCaption ?? "");
				return new HtmlString(contents);
			}

		}

	}

	public enum FigureFloat {
		None,
		Left,
		Right,
	}
}