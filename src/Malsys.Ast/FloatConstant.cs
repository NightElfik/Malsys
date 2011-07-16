using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Malsys.Ast {
	public class FloatConstant : Token, IAstVisitable {
		public readonly double Value;

		public FloatConstant(double value, int beginLine, int beginColumn, int endLine, int endColumn)
			: base(beginLine, beginColumn, endLine, endColumn) {

			Value = value;
		}

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}
