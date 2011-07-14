using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Malsys.Ast {
	public interface IInputFileStatement { }

	public class InputFile {
		public readonly ReadOnlyCollection<IInputFileStatement> Statements;

		public InputFile(IList<IInputFileStatement> statements) {
			Statements = new ReadOnlyCollection<IInputFileStatement>(statements);
		}
	}
}
