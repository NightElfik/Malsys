using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Malsys.Expressions {
	public interface IPostfixExpressionMember {
		bool IsConstant { get; }
		bool IsVariable { get; }
		bool IsEvaluable { get; }
	}
}
