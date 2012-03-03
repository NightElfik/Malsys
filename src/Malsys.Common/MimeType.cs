using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Malsys {
	/// <summary>
	/// Common mime types.
	/// </summary>
	/// <remarks>
	/// http://en.wikipedia.org/wiki/Internet_media_type
	/// Mime types are constants to allow use in switch clauses.
	/// </remarks>
	public static class MimeType {

		public static class Application {

			/// <summary>
			/// Arbitrary binary data.
			/// </summary>
			public const string OctetStream = "application/octet-stream";

			/// <summary>
			/// ZIP archive files.
			/// </summary>
			public const string Zip = "application/zip";

			/// <summary>
			/// GZip.
			/// </summary>
			public const string XGzip = "application/x-gzip";

			/// <summary>
			/// Json (JavaScript Object Notation).
			/// </summary>
			public const string Json = "application/json";

		}

		public static class Image {

			/// <summary>
			/// GIF image.
			/// </summary>
			public const string Gif = "image/gif";

			/// <summary>
			/// JPEG image.
			/// </summary>
			public const string Jpeg = "image/jpeg";

			/// <summary>
			/// PNG image.
			/// </summary>
			public const string Png = "image/png";

			/// <summary>
			/// SVG vector image.
			/// </summary>
			public const string SvgXml = "image/svg+xml";

		}

		public static class Text {

			/// <summary>
			/// Textual data.
			/// </summary>
			public const string Plain = "text/plain";

			/// <summary>
			/// XML data.
			/// </summary>
			public const string Xml = "text/xml";

		}


		public static string ToFileExtension(string mimeType) {

			Contract.Requires(mimeType != null);
			Contract.Ensures(Contract.Result<string>() != null);
			Contract.Ensures(Contract.Result<string>().Length >= 2);
			Contract.Ensures(Contract.Result<string>().StartsWith("."));

			switch (mimeType) {
				case Application.Zip: return ".zip";
				case Application.XGzip: return ".gz";
				case Application.Json: return ".js";
				case Image.Gif: return ".gif";
				case Image.Jpeg: return ".jpg";
				case Image.Png: return ".png";
				case Image.SvgXml: return ".svg";
				case Text.Plain: return ".txt";
				case Text.Xml: return ".xml";
				default: return ".bin";
			}
		}

	}
}
