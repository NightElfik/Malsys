using System.Collections.Generic;
using Malsys.Expressions;
using Malsys.SemanticModel.Compiled;
using Malsys.Compilers.Expressions;

namespace Malsys.Compilers {
	public class ExpressionCompiler {

		private static KnownConstFunOpProvider knownStuffProvider = new KnownConstFunOpProvider();

		static ExpressionCompiler() {
			knownStuffProvider.LoadFromType(typeof(KnownConstant));
			knownStuffProvider.LoadFromType(typeof(FunctionCore));
			knownStuffProvider.LoadFromType(typeof(OperatorCore));
		}


		public IKnownConstantsProvider KnownConstants { get; set; }
		public IKnownFunctionsProvider KnownFunctions { get; set; }
		public IKnownOperatorsProvider KnownOperators { get; set; }

		private MessagesCollection msgs;


		public ExpressionCompiler(MessagesCollection msgsColl) {
			msgs = msgsColl;

			KnownConstants = knownStuffProvider;
			KnownFunctions = knownStuffProvider;
			KnownOperators = knownStuffProvider;
		}


		public IExpression CompileExpression(Ast.Expression expr) {
			var visitor = new ExpressionCompilerVisitor(this, msgs);
			return visitor.Compile(expr);
		}

		public ImmutableList<IExpression> CompileList(IList<Ast.Expression> exprsList) {

			IExpression[] resultArr = new IExpression[exprsList.Count];

			for (int i = 0; i < resultArr.Length; i++) {
				resultArr[i] = CompileExpression(exprsList[i]);
			}

			return new ImmutableList<IExpression>(resultArr, true);
		}

	}
}
