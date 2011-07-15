using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Malsys.Ast {
	public class Keyword : Token, IAstVisitable {
		public Keyword(int beginLine, int beginColumn, int endLine, int endColumn)
			: base(beginLine, beginColumn, endLine, endColumn) {
		}

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}
