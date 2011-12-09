using Malsys.SemanticModel.Evaluated;

namespace Malsys.Web.Models {
	public interface IInputProcessesRepository {

		void AddInput(InputBlock input, long outputSize, string userName);

	}
}