using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Malsys.Ast {
	public class ExpressionFunction : IToken, IAstVisitable, IExpressionMember {

		public byte Arity {
			get {
				Debug.Assert(Arguments.Count < byte.MaxValue, "Too many arguments.");
				return (byte)Arguments.Count;
			}
		}

		public readonly Identificator NameId;
		public readonly ReadOnlyCollection<IValue> Arguments;

		public ExpressionFunction(Identificator name, IList<IValue> args, Position pos) {
			NameId = name;
			Arguments = new ReadOnlyCollection<IValue>(args);
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

		#region IExpressionMember Members

		public bool IsConstant { get { return false; } }
		public bool IsVariable { get { return false; } }
		public bool IsArray { get { return false; } }
		public bool IsOperator { get { return false; } }
		public bool IsFunction { get { return true; } }
		public bool IsIndexer { get { return false; } }
		public bool IsBracketedExpression { get { return false; } }

		#endregion
	}
}
