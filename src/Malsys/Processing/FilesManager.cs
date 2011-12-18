using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing {
	public class FilesManager : IFilesManager, IDisposable {

		private const int maxRandInt = 1000;

		private string root;
		private List<ManagedFile> managedFiles = new List<ManagedFile>();

		private Random rndGenerator = new Random();

		private bool dirty = false;


		public string FilesPrefix { get; set; }
		public LsystemEvaled CurrentLsystem { get; set; }


		public FilesManager(string workDirPath) {

			root = Path.GetFullPath(workDirPath);
			FilesPrefix = "";

			if (!Directory.Exists(workDirPath)) {
				Directory.CreateDirectory(root);
			}
		}



		#region IFilesManager Members

		public FileStream GetOutputStream<TCaller>(string fileNameSuffix, bool temp = false) {

			string path = getNewFilePath(fileNameSuffix);
			var stream = new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Delete, 4096,
				temp ? FileOptions.DeleteOnClose : FileOptions.None);

			if (!temp) {
				var mf = new ManagedFile() {
					OutputFile = new OutputFile(path, CurrentLsystem.Name, typeof(TCaller).FullName),
					Stream = stream
				};

				managedFiles.Add(mf);
				dirty = true;
			}

			return stream;
		}

		public void Cleanup() {

			if (!dirty) {
				return;
			}

			for (int i = 0; i < managedFiles.Count; i++) {
				if (managedFiles[i].Stream.CanWrite) {
					managedFiles[i].Stream.Dispose();
				}
			}

			dirty = false;
		}

		#endregion


		public IEnumerable<OutputFile> GetOutputFilePaths() {
			Cleanup();
			return managedFiles.Select(mf => mf.OutputFile);
		}



		private string getNewFilePath(string suffix) {

			string filePath;
			do {
				string timeStamp = DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss", CultureInfo.InvariantCulture.DateTimeFormat);
				int randInt = rndGenerator.Next(0, maxRandInt);
				string fileName = "{0}{1}.{2}{3}".Fmt(FilesPrefix, timeStamp, randInt, suffix);
				filePath = Path.Combine(root, fileName);
				filePath = Path.GetFullPath(filePath);
				if (!filePath.StartsWith(root)) {
					throw new Exception();
				}
			} while (File.Exists(filePath));

			return filePath;
		}


		private class ManagedFile {

			public OutputFile OutputFile { get; set; }
			public FileStream Stream { get; set; }

		}


		public void Dispose() {
			Cleanup();
		}
	}
}
