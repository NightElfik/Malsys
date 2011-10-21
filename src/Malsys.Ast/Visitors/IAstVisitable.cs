using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Malsys.Ast {
	public interface IAstVisitable {
		void Accept(IAstVisitor visitor);
	}
}
