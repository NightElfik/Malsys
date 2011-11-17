using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Malsys.SemanticModel.Compiled {
	public interface IInputStatement {

		InputStatementType StatementType { get; }

	}

	public enum InputStatementType {
		Constant,
		Function,
		Lsystem,
	}
}
