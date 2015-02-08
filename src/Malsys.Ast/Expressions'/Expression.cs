using System.Collections;
using System.Collections.Generic;

namespace Malsys.Ast {
	public class Expression : IEnumerable<IExpressionMember>, IFunctionStatement {

		public List<IExpressionMember> Members;


		public IExpressionMember this[int i] { get { return Members[i]; } }

		public int MembersCount { get { return Members.Count; } }

		public bool IsEmpty { get { return Members.Count == 0; } }


		public PositionRange Position { get; private set; }


		public Expression(PositionRange pos) {
			Members = new List<IExpressionMember>();
			Position = pos;
		}

		public Expression(IEnumerable<IExpressionMember> mmbrs, PositionRange pos) {
			Members = new List<IExpressionMember>(mmbrs);
			Position = pos;
		}


		IEnumerator IEnumerable.GetEnumerator() {
			return Members.GetEnumerator();
		}

		public IEnumerator<IExpressionMember> GetEnumerator() {
			return ((IEnumerable<IExpressionMember>)Members).GetEnumerator();
		}


		public FunctionStatementType StatementType {
			get { return FunctionStatementType.Expression; }
		}

	}
}
