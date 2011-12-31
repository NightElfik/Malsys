using System;
using System.Collections.Generic;
using Malsys.Processing;
using Malsys.SemanticModel.Evaluated;
using Malsys.Web.Entities;

namespace Malsys.Web.Models {
	public interface IMalsysInputRepository {

		IInputDb InputDb { get; }

		InputProcess AddInputProcess(InputBlock input, int? parentId, IEnumerable<OutputFile> outputs, string userName, TimeSpan duration);

		SavedInput SaveInput(string source, int? parentId, IEnumerable<OutputFile> outputs, string userName, TimeSpan duration);

		void CleanProcessOutputs(string workDirFullPath, int maxFilesCount);

	}
}