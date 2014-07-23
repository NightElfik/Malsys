using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Malsys.Processing;

namespace Malsys.Web {
	public static class OutputHelper {

		public static bool HandleAdvancedOutput(string filePath, string extra,
				KeyValuePair<string, object>[] metadata, ref string downloadFileName, out MemoryStream ms,
				out string mimeType) {

			ms = null;
			mimeType = null;

			if (extra == "obj" || extra == "mtl" || extra == "meta") {
				object obj = metadata.FirstOrDefault(x => x.Key == OutputMetadataKeyHelper.ObjMetadata).Value;
				if (obj == null || !(obj is string) || string.IsNullOrWhiteSpace((string)obj)) {
					return true;
				}
				string[] paths = ((string)obj).Split(' ');
				if (paths.Length != 3) {
					return true;
				}

				string path;
				switch (extra) {
					case "obj": path = paths[0]; break;
					case "mtl": path = paths[1]; break;
					case "meta": path = paths[2]; break;
					default: return true;
				}

				ms = new MemoryStream();

				using (Package package = Package.Open(filePath, FileMode.Open, FileAccess.Read)) {
					var part = package.GetParts().FirstOrDefault(p => p.Uri.OriginalString.TrimStart('/') == path);
					if (part == null) {
						return true;
					}

					using (Stream source = part.GetStream(FileMode.Open, FileAccess.Read)) {
						source.CopyTo(ms);
					}
				}

				mimeType = MimeType.Text.Plain;
				downloadFileName += "." + extra;
				ms.Seek(0, SeekOrigin.Begin);
				return true;
			}


			return false;
		}

	}
}