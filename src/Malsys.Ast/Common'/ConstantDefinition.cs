
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ConstantDefinition : IInputStatement, ILsystemStatement, IFunctionStatement {

		public readonly Identificator NameId;
		public readonly Expression ValueExpr;


		public ConstantDefinition(Identificator name, Expression value, Position pos) {
			NameId = name;
			ValueExpr = value;
			Position = pos;
		}


		public Position Position { get; private set; }


		public void Accept(IInputVisitor visitor) {
			visitor.Visit(this);
		}

		public void Accept(ILsystemVisitor visitor) {
			visitor.Visit(this);
		}

		public void Accept(IFunctionVisitor visitor) {
			visitor.Visit(this);
		}

	}
}
