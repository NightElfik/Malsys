using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbol = Malsys.Symbol<Malsys.Expressions.IValue>;
using System.Diagnostics.Contracts;
using Malsys.Expressions;

namespace Malsys.Interpreters {

	public interface IInterpreter {

		void Initialize();

	}
}
