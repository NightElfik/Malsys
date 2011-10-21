
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class OptionalParameter : IToken {

		public readonly Identificator NameId;
		public readonly Expression OptionalValue;


		public OptionalParameter(Identificator name, Position pos) {
			NameId = name;
			OptionalValue = null;
			Position = pos;
		}

		public OptionalParameter(Identificator name, Expression optionalValue, Position pos) {
			NameId = name;
			OptionalValue = optionalValue;
			Position = pos;
		}


		public bool IsOptional { get { return OptionalValue != null; } }


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
