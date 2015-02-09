using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Malsys.Processing;
using Malsys.Processing.Output;
using Malsys.Web.Entities;
using Malsys.Web.Models.Lsystem;

namespace Malsys.Web.Models {
	public class OutputHtmlGenerator {


		public HtmlString GetOutputHtml(UrlHelper url, OutputFile output, ref int maxWidth, ref int maxHeight,
				bool fillHeight = false, bool noPan3d = false, bool noZoom3d = false, bool noFullScreen = false,
				bool showCameraCoords = false, bool autoRotate = false) {

			return GetOutputHtml(url.Action(MVC.ProcessOutput.Show(Path.GetFileName(output.FilePath))),
				output.Name, output.MimeType, output.Metadata, ref maxWidth, ref maxHeight, fillHeight, noPan3d,
				noZoom3d, noFullScreen, showCameraCoords, autoRotate);
		}

		public HtmlString GetOutputHtml(UrlHelper url, SavedInput input, bool thumbnail, ref int maxWidth, ref int maxHeight,
				bool fillHeight = false, bool noPan3d = false, bool noZoom3d = false, bool noFullScreen = false,
				bool showCameraCoords = false, bool autoRotate = false) {

			return GetOutputHtml(
				thumbnail
					? url.Action(MVC.Gallery.GetThumbnail(input.UrlId, input.EditDate.Hash()))
					: url.Action(MVC.Gallery.GetOutput(input.UrlId, input.EditDate.Hash())),
				input.PublishName,
				input.MimeType,
				OutputMetadataHelper.DeserializeMetadata(thumbnail ? input.OutputThnMetadata : input.OutputMetadata),
				ref maxWidth, ref maxHeight, fillHeight, noPan3d, noZoom3d, noFullScreen, showCameraCoords, autoRotate);
		}


		public HtmlString GetOutputHtml(string url, string name, string mimeType, KeyValuePair<string, object>[] metadata,
				ref int maxWidth, ref int maxHeight, bool fillHeight = false, bool noPan3d = false,
				bool noZoom3d = false, bool noFullScreen = false, bool showCameraCoords = false, bool autoRotate = false) {

			int width, height;

			string content;
			string extraStyle = "";

			switch (mimeType) {

				case MimeType.Text.Plain:
					width = maxWidth;
					height = maxHeight;
					int readChars = 8192;

					// Make an internal request to retrieve data request to avoid any JS
					// that would have to make the request anyways.
					try {
						var webClient = new WebClient();
						content = webClient.DownloadString(StaticUrl.ToAbsolute(url));
					}
					catch (Exception /*ex*/) {
						goto default;
					}

					string msg = "";
					if (content.Length > readChars) {
						msg = "... output trimmed from {0} to {1} characters.".Fmt(content.Length, readChars - 192);
						content = content.Substring(0, readChars - 192);
					}
					content = content.Replace("\r", "").Replace("\n", "<br>").Replace(" ", "&nbsp;");
					var sb = new StringBuilder();
					sb.Append("<pre style='text-decoration: none;' class='asciiArt'>");
					sb.Append(content);
					sb.Append(msg);
					sb.Append("</pre>");
					content = sb.ToString();
					break;

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
					content = "<img src='{0}' {1} height='{2}' alt='{3}' />".Fmt(
						url,
						width > 0 ? "width='" + width + "'" : "",
						height,
						name);
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
					content = "<img src='{0}' {1} height='{2}' alt='{3}' />".Fmt(
						url,
						width > 0 ? "width='" + width + "'" : "",
						height,
						name);
					break;

				case MimeType.Application.Zip:
					string objMeta = OutputMetadataHelper.TryGetValue(metadata, OutputMetadataKeyHelper.ObjMetadata, "");
					if (string.IsNullOrWhiteSpace(objMeta)) {
						bool packedOutputs = OutputMetadataHelper.TryGetValue(metadata, OutputMetadataKeyHelper.PackedOutputs, false);
						if (packedOutputs) {
							width = maxWidth / 2;
							height = maxHeight;
							content = ("<div class='clearfix'><p>Too many outputs to display. All outputs were packed. "
								+ "Please download the package to see the results.</p></div>");
							break;
						}
						goto default;  // Unknown ZIP archive
					}

					width = maxWidth;
					height = maxHeight;

					content = ("<div class='threeJsScene' data-url='{0}' {4} {5} {6} {7} style='width: {1}px; height: {2}px;'>"
							+ "<div class='clearfix'><p class='loading'>Loading 3D model<br>of {3}<br>"
							+ "<span class='dots'></span></p></div></div>").Fmt(
						url,
						maxWidth,
						maxHeight,
						name,
						noPan3d ? "data-no-pan='true'" : "",
						noZoom3d ? "data-no-zoom='true'" : "",
						showCameraCoords ? "data-show-cam-coords='true'" : "",
						autoRotate ? "data-auto-rotate='true'" : "");

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

					content = ("<div class='clearfix'><p>{0}</p><p>Unknown type of output.</p></div>").Fmt(name);
					break;

			}

			if (fillHeight) {
				height = maxHeight;
			}

			string widthStr = width > 0 ? "width:" + width + "px; " : "";

			maxWidth = width;
			maxHeight = height;

			noFullScreen = true;  // Disable FS for now.

			return new HtmlString("<div class='lsystemOutput' style='{0}height:{1}px; {3}'>{2}{4}</div>".Fmt(
				widthStr, height, content, extraStyle,
				noFullScreen ? "" : "<div class='fullScreenToggle'>&nbsp;</div>"
			));
		}
	}
}
