
namespace Malsys.Ast {
	public class ExpressionsArray : ListPos<Expression>, IExpressionMember {


		public ExpressionsArray(PositionRange pos) {
			Position = pos;
		}

		public ExpressionsArray(ListPos<Expression> vals, PositionRange beginSep, PositionRange endSep)
			: base(vals, beginSep, endSep, vals.Position) {

		}


		public ExpressionMemberType MemberType {
			get { return ExpressionMemberType.ExpressionsArray; }
		}

	}
}
