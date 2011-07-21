using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Malsys.Ast {
	public class ValuesArray : IValue, IExpressionMember {

		public IValue this[int i] { get { return Values[i]; } }
		public int Length { get { return Values.Count; } }

		public readonly ReadOnlyCollection<IValue> Values;


		public ValuesArray(Position pos) {
			Values = new ReadOnlyCollection<IValue>(new IValue[0]);
			Position = pos;
		}

		public ValuesArray(IList<IValue> values, Position pos) {
			Values = new ReadOnlyCollection<IValue>(values);
			Position = pos;
		}

		#region IToken Members

		public Position Position { get; private set; }

		#endregion

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			throw new System.NotImplementedException();
		}

		#endregion

		#region IExpressionMember Members

		public bool IsConstant { get { return false; } }
		public bool IsVariable { get { return false; } }
		public bool IsArray { get { return true; } }
		public bool IsOperator { get { return false; } }
		public bool IsFunction { get { return false; } }
		public bool IsIndexer { get { return false; } }
		public bool IsBracketedExpression { get { return false; } }

		#endregion

		#region IValue Members

		public bool IsExpression { get { return false; } }
		//public bool IsArray { get { return true; } }

		#endregion
	}
}
