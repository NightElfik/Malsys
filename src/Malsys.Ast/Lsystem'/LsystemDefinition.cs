/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class LsystemDefinition : NameParamsStatements<ILsystemStatement>, IInputStatement {

		public readonly bool IsAbstract;

		public readonly ImmutableListPos<BaseLsystem> BaseLsystems;


		public LsystemDefinition(Identificator name, bool isAbstract, ImmutableListPos<OptionalParameter> prms,
				ImmutableListPos<BaseLsystem> baseLsystems, ImmutableListPos<ILsystemStatement> statements, Position pos)
			: base(name, prms, statements, pos) {

			IsAbstract = isAbstract;
			BaseLsystems = baseLsystems;
		}


		public InputStatementType StatementType {
			get { return InputStatementType.LsystemDefinition; }
		}

	}
}
