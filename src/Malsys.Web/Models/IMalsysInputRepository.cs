using System;
using System.Collections.Generic;
using Malsys.Processing.Output;
using Malsys.SemanticModel.Evaluated;
using Malsys.Web.Entities;

namespace Malsys.Web.Models {
	public interface IMalsysInputRepository {

		IInputDb InputDb { get; }

		InputProcess AddInputProcess(InputBlockEvaled input, int? parentId, IEnumerable<OutputFile> outputs, string userName, TimeSpan duration);

		SavedInput SaveInput(string source, int? parentId, IEnumerable<OutputFile> outputs, string userName, TimeSpan duration);

		void CleanProcessOutputs(string workDirFullPath, int maxFilesCount, int filesToDelete);

		/// <summary>
		/// Gets tags with given names.
		/// Tags which do not exist are created.
		/// </summary>
		IEnumerable<Tag> GetTags(params string[] tags);

		bool Vote(string urlId, string userName, bool upVote);

		bool? GetUserVote(string urlId, string userName);

	}
}