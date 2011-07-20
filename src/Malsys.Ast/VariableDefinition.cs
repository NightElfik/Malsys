using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Malsys.Ast {
	public interface IVariableValue { }

	public class VariableDefinition : IToken, IInputFileStatement, ILsystemStatement {
		public readonly Keyword Keyword;
		public readonly Identificator Name;
		public readonly IVariableValue Value;

		public VariableDefinition(Keyword keyword, Identificator name, IVariableValue value, Position pos) {
			Keyword = keyword;
			Name = name;
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


	public class VariableValuesArray : IVariableValue {
		public readonly ReadOnlyCollection<IVariableValue> Values;

		public VariableValuesArray() {
			Values = new ReadOnlyCollection<IVariableValue>(new IVariableValue[0]);
		}

		public VariableValuesArray(IList<IVariableValue> values) {
			Values = new ReadOnlyCollection<IVariableValue>(values);
		}
	}
}
