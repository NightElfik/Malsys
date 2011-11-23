using System.Collections.Generic;
using Malsys.Compilers.Expressions;
using Malsys.Expressions;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	public partial class ExpressionCompiler : MessagesLogger<ExpressionCompilerMessageType> {

		private static KnownConstFunOpProvider knownStuffProvider = new KnownConstFunOpProvider();

		static ExpressionCompiler() {
			knownStuffProvider.LoadFromType(typeof(KnownConstant));
			knownStuffProvider.LoadFromType(typeof(FunctionCore));
			knownStuffProvider.LoadFromType(typeof(OperatorCore));
		}


		public IKnownConstantsProvider KnownConstants { get; set; }
		public IKnownFunctionsProvider KnownFunctions { get; set; }
		public IKnownOperatorsProvider KnownOperators { get; set; }


		public ExpressionCompiler(MessagesCollection msgs) : base(msgs) {

			KnownConstants = knownStuffProvider;
			KnownFunctions = knownStuffProvider;
			KnownOperators = knownStuffProvider;
		}


		public IExpression CompileExpression(Ast.Expression expr) {
			var visitor = new ExpressionCompilerVisitor(this);
			return visitor.Compile(expr);
		}

		public ImmutableList<IExpression> CompileList(IList<Ast.Expression> exprsList) {

			IExpression[] resultArr = new IExpression[exprsList.Count];

			for (int i = 0; i < resultArr.Length; i++) {
				resultArr[i] = CompileExpression(exprsList[i]);
			}

			return new ImmutableList<IExpression>(resultArr, true);
		}


		public override string GetMessageTypeId(ExpressionCompilerMessageType msgType) {
			return msgType.ToString();
		}

		protected override string resolveMessage(ExpressionCompilerMessageType msgType, out MessageType type, params object[] args) {

			type = MessageType.Error;

			switch (msgType) {
				case ExpressionCompilerMessageType.InternalError:
					return "Internal compiler error. {0}".Fmt(args);
				case ExpressionCompilerMessageType.UnexcpectedEndOfExpression:
					return "Unexcpected end of expression.";
				case ExpressionCompilerMessageType.UnexcpectedOperand:
					return "Unexcpected operand `{0}`, excpecting operator.".Fmt(args);
				case ExpressionCompilerMessageType.UnexcpectedOperator:
					return "Unexcpected `{0}` operator, excpecting operand.".Fmt(args);
				case ExpressionCompilerMessageType.TooFewOperands:
					return "Too few operands in expression.";
				case ExpressionCompilerMessageType.TooManyOperands:
					return "Too many operands in expression.";
				case ExpressionCompilerMessageType.UnknownUnaryOperator:
					return "Unknown unary operator `{0}`.".Fmt(args);
				case ExpressionCompilerMessageType.UnknownBinaryOperator:
					return "Unknown binary operator `{0}`.".Fmt(args);
				default:
					return "Unknown error.";
			}

		}
	}

	public enum ExpressionCompilerMessageType {
		Unknown,
		InternalError,
		UnexcpectedEndOfExpression,
		UnexcpectedOperand,
		UnexcpectedOperator,
		TooFewOperands,
		TooManyOperands,
		UnknownUnaryOperator,
		UnknownBinaryOperator,
	}
}
