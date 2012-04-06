using Malsys.SemanticModel.Compiled;

namespace Malsys.Evaluators {

	public interface IBaseLsystemResolver {

		LsystemEvaledParams Resolve(string name, IMessageLogger logger);

	}


}
