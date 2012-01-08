﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;

namespace Malsys.Processing.Output {
	public class FileOutputProvider : IOutputProvider, IDisposable {

		private const int maxRandInt = 1000;

		private string root;
		private List<ManagedFile> managedFiles = new List<ManagedFile>();

		private Random rndGenerator = new Random();

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

		/// <summary>
		///
		/// </summary>
		/// <remarks>
		/// If in <paramref name="additionalData" /> is key CommonAdditionalDataKeys.OutputIsGZipped with value true,
		/// ".gz" will be added to file extension and files with mime type MimeType.Image_SvgXml will have extension ".svgz".
		/// </remarks>
		public Stream GetOutputStream<TCaller>(string outputName, string mimeType,
				bool temp = false, FSharpMap<string, object> additionalData = null) {

			if (additionalData == null) {
				additionalData = MapModule.Empty<string, object>();
			}

			string ext = MimeType.ToFileExtension(mimeType);
			if (additionalData.ContainsValue(CommonAdditionalDataKeys.OutputIsGZipped, true)) {
				if (mimeType == MimeType.Image_SvgXml) {
					ext = ".svgz";
				}
				else {
					ext += ".gz";
				}
			}

			string path = getNewFilePath(ext);
			Stream stream = new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Delete, 4096,
				temp ? FileOptions.DeleteOnClose : FileOptions.None);

			var mf = new ManagedFile() {
				Name = outputName,
				Stream = stream,
				FilePath = path,
				IsTemporary = temp,
				MimeType = mimeType,
				Caller = typeof(TCaller),
				AdditionalData = additionalData
			};

			managedFiles.Add(mf);
			allStreamsClosed = false;

			return stream;
		}

		public void AddAdditionalData(Stream outputStream, string key, object value) {
			var managedFile = managedFiles.Where(x => x.Stream == outputStream).Single();
			managedFile.AdditionalData = managedFile.AdditionalData.Add(key, value);
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
					file.Stream.Close();
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
				.Where(x => !x.IsTemporary)
				.Select(x => new OutputFile(
					x.Name,
					x.FilePath,
					x.MimeType,
					x.Caller,
					x.AdditionalData));
		}



		private string getNewFilePath(string suffix) {

			Contract.Ensures(Contract.Result<string>().StartsWith(root));

			string filePath;
			do {
				string timeStamp = DateTime.Now.ToString("yyyy-MM-dd--HH-mm-ss", CultureInfo.InvariantCulture.DateTimeFormat);
				int randInt = rndGenerator.Next(0, maxRandInt);
				string fileName = "{0}{1}.{2:000}{3}".Fmt(FilesPrefix, timeStamp, randInt, suffix);
				filePath = Path.Combine(root, fileName);
				filePath = Path.GetFullPath(filePath);
				if (!filePath.StartsWith(root)) {
					throw new Exception("");
				}
			} while (File.Exists(filePath));

			return filePath;
		}


		private class ManagedFile {

			public string Name;
			public Stream Stream;
			public string FilePath;
			public bool IsTemporary;
			public string MimeType;
			public Type Caller;
			public FSharpMap<string, object> AdditionalData;

		}


		public void Dispose() {
			CloseAllOutputStreams();
		}
	}
}
