
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Identificator : IToken, IExpressionMember {

		public readonly string Name;


		public Identificator(string name, Position pos) {
			Name = name;
			Position = pos;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion

		#region IAstExpressionVisitable Members

		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}
