using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Malsys.Processing {
	public class FilesManager {

		private const int maxRandInt = 1000;

		private string root;
		private List<string> outFilePaths = new List<string>();
		private List<string> tmpFilePaths = new List<string>();
		private Random rndGenerator = new Random();


		public string FilesPrefix { get; set; }


		public FilesManager(string workDirPath) {

			root = Path.GetFullPath(workDirPath);
			FilesPrefix = "";

			if (!Directory.Exists(workDirPath)) {
				Directory.CreateDirectory(root);
			}
		}

		public string GetNewOutputFilePath(string suffix = ".out") {
			string path = getNewFilePath(suffix);
			outFilePaths.Add(path);
			return path;
		}

		public string GetNewTempFilePath(string suffix = ".tmp") {
			string path = getNewFilePath(suffix);
			tmpFilePaths.Add(path);
			return path;
		}

		public IEnumerable<string> GetOutputFilePaths() {
			return outFilePaths;
		}

		public IEnumerable<string> GetTempFilePaths() {
			return tmpFilePaths;
		}

		public bool TryDeleteAllTempFiles() {

			bool allDeleted = true;

			for (int i = 0; i < tmpFilePaths.Count; i++) {

				if (File.Exists(tmpFilePaths[i])) {
					tmpFilePaths.RemoveAt(i);
					i--;
					continue;
				}

				try {
					File.Delete(tmpFilePaths[i]);
					tmpFilePaths.RemoveAt(i);
					i--;
					continue;
				}
				catch (Exception) {
					allDeleted = false;
				}
			}

			return allDeleted;
		}


		private string getNewFilePath(string suffix) {

			string timeStamp = DateTime.Now.ToString("s", CultureInfo.InvariantCulture.DateTimeFormat);  // s: 2008-06-15T21:15:07
			int randInt = rndGenerator.Next(0, maxRandInt);

			string filePath;
			do {
				randInt = (randInt + 1) % maxRandInt;
				string fileName = "{0}{1}.{2}{3}".Fmt(FilesPrefix, timeStamp, randInt, suffix);
				filePath = Path.Combine(root, fileName);
				filePath = Path.GetFullPath(filePath);
				if (!filePath.StartsWith(root)) {
					throw new Exception();
				}
			} while (File.Exists(filePath));

			return filePath;
		}

	}
}
