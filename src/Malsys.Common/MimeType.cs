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

		#region Type application

		/// <summary>
		/// Arbitrary binary data.
		/// </summary>
		public const string Application_OctetStream = "application/octet-stream";

		/// <summary>
		/// ZIP archive files.
		/// </summary>
		public const string Application_Zip = "application/zip";

		/// <summary>
		/// GZip.
		/// </summary>
		public const string Application_XGzip = "application/x-gzip";

		/// <summary>
		/// Json (JavaScript Object Notation).
		/// </summary>
		public const string Application_Json = "application/json";

		#endregion

		#region Type image

		/// <summary>
		/// GIF image.
		/// </summary>
		public const string Image_Gif = "image/gif";

		/// <summary>
		/// JPEG image.
		/// </summary>
		public const string Image_Jpeg = "image/jpeg";

		/// <summary>
		/// PNG image.
		/// </summary>
		public const string Image_Png = "image/png";

		/// <summary>
		/// SVG vector image.
		/// </summary>
		public const string Image_SvgXml = "image/svg+xml";

		#endregion

		#region Type text

		/// <summary>
		/// Textual data.
		/// </summary>
		public const string Text_Plain = "text/plain";

		/// <summary>
		/// XML data.
		/// </summary>
		public const string Text_Xml = "text/xml";

		#endregion


		public static string ToFileExtension(string mimeType) {

			Contract.Requires(mimeType != null);
			Contract.Ensures(Contract.Result<string>() != null);
			Contract.Ensures(Contract.Result<string>().Length >= 2);
			Contract.Ensures(Contract.Result<string>().StartsWith("."));

			switch (mimeType) {
				case Application_Zip: return ".zip";
				case Application_XGzip: return ".gz";
				case Image_Gif: return ".gif";
				case Image_Jpeg: return ".jpg";
				case Image_Png: return ".png";
				case Image_SvgXml: return ".svg";
				case Text_Plain: return ".txt";
				case Text_Xml: return ".xml";
				default: return ".bin";
			}
		}

	}
}
