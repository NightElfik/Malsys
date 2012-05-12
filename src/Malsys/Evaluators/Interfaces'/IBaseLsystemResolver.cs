/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using Malsys.SemanticModel.Compiled;

namespace Malsys.Evaluators {

	public interface IBaseLsystemResolver {

		LsystemEvaledParams Resolve(string name, IMessageLogger logger);

	}


}
