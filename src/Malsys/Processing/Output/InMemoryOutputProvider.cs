using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.FSharp.Collections;

namespace Malsys.Processing.Output {
	public class InMemoryOutputProvider : IOutputProvider {

		private List<ManagedOutput> managedFiles = new List<ManagedOutput>();


		public Stream GetOutputStream<TCaller>(string outputName, string mimeType,
				bool temp = false, FSharpMap<string, object> additionalData = null) {

			MemoryStream stream = new MemoryStream(1024);

			var mf = new ManagedOutput() {
				Name = outputName,
				Stream = stream,
				IsTemporary = temp,
				MimeType = mimeType,
				Caller = typeof(TCaller),
				AdditionalData = additionalData ?? MapModule.Empty<string, object>()
			};

			managedFiles.Add(mf);

			return stream;
		}

		public void AddAdditionalData(Stream outputStream, string key, object value) {
			var managedFile = managedFiles.Where(x => x.Stream == outputStream).Single();
			managedFile.AdditionalData = managedFile.AdditionalData.Add(key, value);
		}

		/// <summary>
		/// Returns all outputs.
		/// </summary>
		public IEnumerable<InMemoryOutput> GetOutputs() {
			return managedFiles
				.Where(x => !x.IsTemporary)
				.Select(x => new InMemoryOutput(
					x.Stream.ToArray(),  // copies array, potential performance loss
					x.MimeType,
					x.Caller,
					x.AdditionalData));
		}


		private class ManagedOutput {

			public string Name;
			public MemoryStream Stream;
			public bool IsTemporary;
			public string MimeType;
			public Type Caller;
			public FSharpMap<string, object> AdditionalData;

		}


	}
}
