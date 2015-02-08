
namespace Malsys.Ast {
	public class EmptyStatement : IStatement, IInputStatement, ILsystemStatement, IProcessConfigStatement {

		public PositionRange Position { get; private set; }


		public EmptyStatement(PositionRange pos) {
			Position = pos;
		}



		InputStatementType IInputStatement.StatementType {
			get { return InputStatementType.EmptyStatement; }
		}

		LsystemStatementType ILsystemStatement.StatementType {
			get { return LsystemStatementType.EmptyStatement; }
		}

		ProcessConfigStatementType IProcessConfigStatement.StatementType {
			get { return ProcessConfigStatementType.EmptyStatement; }
		}

	}
}
