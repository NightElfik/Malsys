using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Malsys.Ast {
	public interface IInputFileStatement : IAstVisitable { }

	public class InputFile : IAstVisitable {
		public readonly ReadOnlyCollection<IInputFileStatement> Statements;

		public InputFile(IList<IInputFileStatement> statements) {
			Statements = new ReadOnlyCollection<IInputFileStatement>(statements);
		}

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}
