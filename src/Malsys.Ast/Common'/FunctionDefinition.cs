
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class FunctionDefinition : NameParamsStatements<IFunctionStatement>, IInputStatement, ILsystemStatement {

		public FunctionDefinition(Identificator name, ImmutableListPos<OptionalParameter> prms,
				ImmutableListPos<IFunctionStatement> statements, Position pos)
			: base(name, prms, statements, pos) { }


		public int ParametersCount { get { return Parameters.Length; } }



		public void Accept(IInputVisitor visitor) {
			visitor.Visit(this);
		}

		public void Accept(ILsystemVisitor visitor) {
			visitor.Visit(this);
		}

	}
}
