
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class FunctionDefinition : NameParamsStatements<IFunctionStatement>, IInputStatement, ILsystemStatement {

		public FunctionDefinition(Identificator name, ImmutableListPos<OptionalParameter> prms,
				ImmutableListPos<IFunctionStatement> statements, Position pos)
			: base(name, prms, statements, pos) { }


		public int ParametersCount { get { return Parameters.Length; } }


		#region IAstInputVisitable Members

		public void Accept(IInputVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion

		#region IAstLsystemVisitable Members

		public void Accept(ILsystemVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}
