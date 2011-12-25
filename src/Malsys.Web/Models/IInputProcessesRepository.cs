using System.Collections.Generic;
using Malsys.Processing;
using Malsys.SemanticModel.Evaluated;
using Malsys.Web.Entities;

namespace Malsys.Web.Models {
	public interface IInputProcessesRepository {

		IInputDb InputDb { get; }

		InputProcess AddInput(InputBlock input, int? parentId, IEnumerable<OutputFile> outputs, string userName);

		void CleanProcessOutputs(string workDirFullPath, int maxFilesCount);

	}
}