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

		public ExpressionMemberType MemberType { get { return ExpressionMemberType.Array; } }

		#endregion

		#region IValue Members

		public bool IsExpression { get { return false; } }
		public bool IsArray { get { return true; } }

		#endregion
	}
}
