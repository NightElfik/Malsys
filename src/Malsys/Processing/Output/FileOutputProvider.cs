﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Threading;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;

namespace Malsys.Processing.Output {
	public class FileOutputProvider : IOutputProvider, IDisposable {

		private string root;
		private List<ManagedFile> managedFiles = new List<ManagedFile>();

		private bool allStreamsClosed = true;


		public string FilesPrefix { get; set; }
		public LsystemEvaled CurrentLsystem { get; set; }


		public FileOutputProvider(string workDirPath) {

			root = Path.GetFullPath(workDirPath);
			FilesPrefix = "";

			if (!Directory.Exists(workDirPath)) {
				Directory.CreateDirectory(root);
			}
		}


		#region IOutputProvider Members

		/// <remarks>
		/// If in <paramref name="metadata" /> is key OutputMetadataKeyHelper.OutputIsGZipped with value true,
		/// ".gz" will be added to file extension and files with mime type MimeType.Image_SvgXml will have extension ".svgz".
		/// </remarks>
		public Stream GetOutputStream<TCaller>(string outputName, string mimeType, bool temp = false, IDictionary<string, object> metadata = null) {

			Dictionary<string, object> meta;

			if (metadata == null) {
				meta = new Dictionary<string, object>();
			}
			else {
				meta = metadata.ToDictionaryOverwrite(x => x.Key, x => x.Value);
			}

			string ext;

			if (meta.ContainsKey(OutputMetadataKeyHelper.OutputSuggestedFileExt)
					&& !string.IsNullOrWhiteSpace(meta[OutputMetadataKeyHelper.OutputSuggestedFileExt].ToString())) {

				ext = meta[OutputMetadataKeyHelper.OutputSuggestedFileExt].ToString().Trim();
			}
			else {
				ext = MimeType.ToFileExtension(mimeType);
			}

			if (meta.ContainsValue(OutputMetadataKeyHelper.OutputIsGZipped, true)) {
				if (mimeType == MimeType.Image.SvgXml) {
					ext = ".svgz";
				}
				else {
					ext += ".gz";
				}
			}

			string path = allocateNewFile(ext);
			Stream stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.Delete, 4096,
				temp ? FileOptions.DeleteOnClose : FileOptions.None);

			var mf = new ManagedFile() {
				Name = outputName,
				Stream = stream,
				FilePath = path,
				IsTemporary = temp,
				MimeType = mimeType,
				Caller = typeof(TCaller),
				Metadata = meta
			};

			managedFiles.Add(mf);
			allStreamsClosed = false;

			return stream;
		}

		public void AddMetadata(Stream outputStream, string key, object value) {
			var managedFile = managedFiles.Where(x => x.Stream == outputStream).Single();
			managedFile.Metadata.Add(key, value);
		}

		public FSharpMap<string, object> GetMetadata(Stream outputStream) {
			return managedFiles
				.Where(x => x.Stream == outputStream)
				.Select(x => x.Metadata.ToFsharpMap(y => y.Key, y => y.Value))
				.FirstOrDefault();
		}

		#endregion IOutputProvider Members


		public int OutputsCount {
			get { return managedFiles.Where(x => !x.IsTemporary).Count(); }
		}


		/// <summary>
		/// Closes all opened output streams and deletes all temporary outputs.
		/// </summary>
		public void CloseAllOutputStreams() {

			if (allStreamsClosed) {
				return;
			}

			for (int i = 0; i < managedFiles.Count; ++i) {

				var file = managedFiles[i];

				if (file.Stream.CanWrite) {
					file.Stream.Flush();
					file.Stream.Dispose();
				}

				if (file.IsTemporary) {
					// deleting of temporary file should be automatic because of FileOptions.DeleteOnClose
					managedFiles.RemoveAt(i);
					--i;
				}
			}

			allStreamsClosed = true;
		}


		/// <summary>
		/// Returns all output files.
		/// </summary>
		public IEnumerable<OutputFile> GetOutputFiles() {

			CloseAllOutputStreams();

			return managedFiles
				.Select(x => new OutputFile(
					x.Name,
					x.FilePath,
					x.MimeType,
					x.Caller,
					x.Metadata.ToArray()));
		}

		/// <summary>
		/// Adds all output files into zip archive and returns it as single output.
		/// Output files are deleted from file system.
		/// </summary>
		public IEnumerable<OutputFile> GetOutputFilesAsZipArchive() {

			CloseAllOutputStreams();

			string packagePath = allocateNewFile(".zip");
			using (Package package = Package.Open(packagePath, FileMode.Create)) {

				foreach (var file in managedFiles) {

					Uri fileUri = PackUriHelper.CreatePartUri(new Uri(Path.GetFileName(file.FilePath), UriKind.Relative));

					PackagePart packagePart = package.CreatePart(fileUri, file.MimeType, CompressionOption.Normal);
					using (var fs = new FileStream(file.FilePath, FileMode.Open, FileAccess.Read)) {
						fs.CopyTo(packagePart.GetStream());
					}

					try {
						File.Delete(file.FilePath);
					}
					catch (Exception) {
						// TODO: handle it somehow
						Debug.Fail("File not deleted.");
					}

				}
			}

			managedFiles.Clear();
			FSharpMap<string, object> additionalData = MapModule.Empty<string, object>();
			additionalData = additionalData.Add(OutputMetadataKeyHelper.PackedOutputs, true);
			return new OutputFile[] {
				new OutputFile("Packed output files", packagePath, MimeType.Application.Zip, typeof(FileOutputProvider), additionalData.ToArray())
			};

		}



		private string allocateNewFile(string suffix) {
			Contract.Ensures(Contract.Result<string>().StartsWith(root));

			int tryNumber = 0;
			string filePath;
			while (true) {
				string timeStamp = DateTime.Now.ToString("yyyy-MM-dd--HH-mm-ss-fffff", CultureInfo.InvariantCulture.DateTimeFormat);
				string fileName = "{0}{1}{2}".Fmt(FilesPrefix, timeStamp, suffix);
				filePath = Path.Combine(root, fileName);
				filePath = Path.GetFullPath(filePath);  // canonicalize path

				if (!filePath.StartsWith(root)) {
					throw new Exception("File path escaped from root directory.");
				}

				// TODO: fix this, it is not secure (thread safe)
				if (!File.Exists(filePath)) {
					try {
						File.Create(filePath).Dispose();
						break;
					}
					catch (IOException) {
						// to avoid locking, catch exceptions in the file was already been created by another thread
					}
				}

				Thread.Sleep(1);  // DateTime.Now is not as detailed as I expected

				if (tryNumber++ > 16) {
					throw new Exception("Unable to find unique file name.");
				}
			}

			return filePath;
		}


		private class ManagedFile {

			public string Name;
			public Stream Stream;
			public string FilePath;
			public bool IsTemporary;
			public string MimeType;
			public Type Caller;
			public Dictionary<string, object> Metadata;

		}


		public void Dispose() {
			CloseAllOutputStreams();
		}

	}
}
