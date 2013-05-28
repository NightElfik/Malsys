// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.FSharp.Collections;

namespace Malsys.Processing.Output {
	public class InMemoryOutputProvider : IOutputProvider {

		private List<ManagedOutput> managedFiles = new List<ManagedOutput>();


		public Stream GetOutputStream<TCaller>(string outputName, string mimeType,
				bool temp = false, IDictionary<string, object> metadata = null) {

			MemoryStream stream = new MemoryStream(1024);

			var mf = new ManagedOutput() {
				Name = outputName,
				Stream = stream,
				IsTemporary = temp,
				MimeType = mimeType,
				Caller = typeof(TCaller),
				Metadata = metadata != null ? new Dictionary<string, object>(metadata) : new Dictionary<string, object>()
			};

			managedFiles.Add(mf);

			return stream;
		}

		public void AddMetadata(Stream outputStream, string key, object value) {
			var managedFile = managedFiles.Where(x => x.Stream == outputStream).SingleOrDefault();
			if (managedFile == null) {
				throw new ArgumentException("Given output stream does not exist.");
			}
			managedFile.Metadata[key] = value;
		}

		public FSharpMap<string, object> GetMetadata(Stream outputStream) {
			return managedFiles
				.Where(x => x.Stream == outputStream)
				.Select(x => x.Metadata.ToFsharpMap(y => y.Key, y => y.Value))
				.FirstOrDefault();
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
					x.Metadata.ToFsharpMap(y => y.Key, y => y.Value)));
		}
		
		public int OutputsCount {
			get { return managedFiles.Where(x => !x.IsTemporary).Count(); }
		}

		private class ManagedOutput {

			public string Name;
			public MemoryStream Stream;
			public bool IsTemporary;
			public string MimeType;
			public Type Caller;
			public Dictionary<string, object> Metadata;

		}


	}
}
