
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ConstantDefinition : IInputStatement, ILsystemStatement, IFunctionStatement {

		public readonly Identificator NameId;

		public readonly Expression ValueExpr;

		public readonly bool IsComponentAssign;


		public ConstantDefinition(Identificator name, Expression value, Position pos) {
			NameId = name;
			ValueExpr = value;
			IsComponentAssign = false;
			Position = pos;
		}

		public ConstantDefinition(Identificator name, Expression value, bool isComponentAssign, Position pos) {
			NameId = name;
			ValueExpr = value;
			IsComponentAssign = isComponentAssign;
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
