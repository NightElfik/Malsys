
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class LsystemDefinition : NameParamsStatements<ILsystemStatement>, IInputStatement {

		public LsystemDefinition(Identificator name, ImmutableListPos<OptionalParameter> prms,
			ImmutableListPos<ILsystemStatement> statements, Position pos) : base(name, prms, statements, pos) {		}


		#region IInputVisitable Members

		public void Accept(IInputVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}
