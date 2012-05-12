/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.IO;
using System.Reflection;

namespace Malsys.Resources {
	public class ResourcesReader {

		private static readonly string assemblyPath = typeof(ResourcesReader).Namespace;


		public string GetResourceString(string resourceFileName) {

			using (Stream stream = GetResourceStream(resourceFileName)) {
				using (StreamReader reader = new StreamReader(stream)) {
					return reader.ReadToEnd();
				}
			}
		}

		public Stream GetResourceStream(string resourceFileName) {

			Assembly ass = this.GetType().Assembly;
			return ass.GetManifestResourceStream(assemblyPath + "." + resourceFileName);
		}

	}
}
