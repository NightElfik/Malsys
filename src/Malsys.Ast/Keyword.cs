using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Malsys.Ast {
	public class Keyword : Token {
		public Keyword(int beginLine, int beginColumn, int endLine, int endColumn)
			: base(beginLine, beginColumn, endLine, endColumn) {
		}
	}
}
