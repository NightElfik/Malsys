
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class OptionalParameter : IToken, IAstVisitable {

		public readonly Identificator Name;
		public readonly Expression OptionalValue;


		public OptionalParameter(Identificator name, Position pos) {
			Name = name;
			OptionalValue = null;
			Position = pos;
		}

		public OptionalParameter(Identificator name, Expression optionalValue, Position pos) {
			Name = name;
			OptionalValue = optionalValue;
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
