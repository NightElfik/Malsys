using System.Collections.Generic;
using System.Linq;

namespace Malsys.Ast {
	public interface ILsystemStatement : IAstVisitable { }

	/// <summary>
	/// Immutable.
	/// </summary>
	public class Lsystem : IToken, IInputFileStatement {

		public ILsystemStatement this[int i] { get { return statements[i]; } }

		public readonly Keyword Keyword;
		public readonly Identificator Name;
		public readonly int ParametersCount;
		public readonly int StatementsCount;

		private OptionalParameter[] parameters;
		private ILsystemStatement[] statements;


		public Lsystem(Keyword keyword, Identificator name, IEnumerable<ILsystemStatement> satetmnts, Position pos) {
			Keyword = keyword;
			Name = name;
			parameters = null;
			statements = satetmnts.ToArray();
			Position = pos;

			ParametersCount = 0;
			StatementsCount = statements.Length;
		}

		public Lsystem(Keyword keyword, Identificator name, IEnumerable<OptionalParameter> prms, IEnumerable<ILsystemStatement> satetmnts, Position pos) {
			Keyword = keyword;
			Name = name;
			parameters = prms.ToArray();
			statements = satetmnts.ToArray();
			Position = pos;

			ParametersCount = parameters.Length;
			StatementsCount = statements.Length;
		}

		public OptionalParameter GetOptionalParameter(int i) {
			return parameters[i];
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}
