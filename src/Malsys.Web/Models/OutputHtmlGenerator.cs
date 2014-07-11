using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Malsys.Processing;
using Malsys.Web.Entities;
using Malsys.Web.Models.Lsystem;

namespace Malsys.Web.Models {
	public class OutputHtmlGenerator {

		public HtmlString GetOutputHtml(UrlHelper url, SavedInput input, ref int maxWidth, ref int maxHeight,
				bool fillHeight = false, bool pan3d = false, bool zoom3d = false) {
			var metadata = OutputMetadataHelper.DeserializeMetadata(input.OutputThnMetadata);
			int width, height;

			string content;

			switch (input.MimeType) {

				case MimeType.Image.SvgXml:
					width = OutputMetadataHelper.TryGetValue(metadata, OutputMetadataKeyHelper.OutputWidth, -1);
					height = OutputMetadataHelper.TryGetValue(metadata, OutputMetadataKeyHelper.OutputHeight, -1);

					if (width > 0 && height > 0) {
						MathHelper.ScaleSizeToFit(ref width, ref height, maxWidth, maxHeight);
					}
					else {
						height = maxHeight;
						width = -1;  // Do now put width;
					}
					content = "<img src=\"{0}\" {1} height=\"{2}px\" alt=\"{3}\" />".Fmt(
						url.Action(MVC.Gallery.GetThumbnail(input.UrlId, input.EditDate.Hash())),
						width > 0 ? "width=\"" + width + "px\"" : "",
						height,
						input.PublishName);
					break;

				case MimeType.Image.Png:
				case MimeType.Image.Jpeg:
				case MimeType.Image.Gif:
					width = OutputMetadataHelper.TryGetValue(metadata, OutputMetadataKeyHelper.OutputWidth, -1);
					height = OutputMetadataHelper.TryGetValue(metadata, OutputMetadataKeyHelper.OutputHeight, -1);

					if (width > 0 && height > 0) {
						MathHelper.ScaleSizeToFit(ref width, ref height, maxWidth, maxHeight);
					}
					else {
						height = maxHeight;
					}
					content = "<img src=\"{0}\" {1} height=\"{2}px\" alt=\"{3}\" />".Fmt(
						url.Action(MVC.Gallery.GetThumbnail(input.UrlId, input.EditDate.Hash())),
						width > 0 ? "width=\"" + width + "px\"" : "",
						height,
						input.PublishName);
					break;

				case MimeType.Application.Zip:
					string objMeta = OutputMetadataHelper.TryGetValue(metadata, OutputMetadataKeyHelper.ObjMetadata, "");
					if (string.IsNullOrWhiteSpace(objMeta)) {
						goto default;  // Unknown ZIP archive
					}

					width = maxWidth;
					height = maxHeight;

					content = ("<div class=\"threeJsScene\" data-url=\"{0}\" {4}{5} style=\"width: {1}px; height: {2}px;\">"
							+ "<div class=\"clearfix\"><p class=\"loading\">Loading 3D model<br>of {3}<br>"
							+ "<span class=\"dots\"></p></div></div>").Fmt(
						url.Action(MVC.Gallery.GetThumbnail(input.UrlId, input.EditDate.Hash())),
						maxWidth,
						maxHeight,
						input.PublishName,
						pan3d ? "data-pan=\"true\"" : "",
						zoom3d ? "data-zoom=\"true\"" : "");

					StaticHtml.RequireScript(Links.Js.ThreeJs.Three_js, LoadingOrder.Default);
					StaticHtml.RequireScript(Links.Js.ThreeJs.Detector_js, LoadingOrder.Default);
					StaticHtml.RequireScript(Links.Js.ThreeJs.Stats_js, LoadingOrder.Default);
					StaticHtml.RequireScript(Links.Js.ThreeJs.TrackballControls_js, LoadingOrder.Default);
					StaticHtml.RequireScript(Links.Js.ThreeJs.MTLLoader_js, LoadingOrder.Default);
					StaticHtml.RequireScript(Links.Js.ThreeJs.OBJMTLLoader_js, LoadingOrder.Default);
					StaticHtml.RequireScript(Links.Js.ThreeJs.Malsys_three_js, LoadingOrder.Default);
					break;

				default:
					width = maxWidth / 2;
					height = maxHeight;

					content = ("<div class=\"clearfix\"><p>{0}</p><p>Thumbnail for this type of output is not supported. "
							+ "Please click to see the output.</p></div>").Fmt(input.PublishName);

					break;

			}

			if (fillHeight) {
				height = maxHeight;
			}

			string widthStr = width > 0 ? "width:" + width + "px; " : "";

			maxWidth = width;
			maxHeight = height;

			return new HtmlString("<div style=\"{0}height:{1}px; margin:auto;\">{2}</div>".
				Fmt(widthStr, height, content));
		}
	}
}