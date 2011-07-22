using Malsys.Expressions;

namespace Malsys {
	public class Symbol {
		public string Syntax { get; set; }
		public IExpression[] Arguments { get; set; }
	}
}
