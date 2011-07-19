﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Malsys.Ast {
	public class FloatConstant : IToken, IAstVisitable, IExpressionMember {
		public readonly double Value;

		public FloatConstant(double value, Position pos) {
			Value = value;
			Position = pos;
		}

		#region IToken Members

		public Position Position { get; private set; }

		#endregion

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}
