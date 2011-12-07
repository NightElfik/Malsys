using Malsys.Compilers.Expressions;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	internal partial class ExpressionCompiler : IExpressionCompiler {

		// used by visitor
		private IKnownConstantsProvider knownConstants;
		private IKnownFunctionsProvider knownFunctions;
		private IKnownOperatorsProvider knownOperators;


		public ExpressionCompiler(IKnownConstantsProvider constants, IKnownFunctionsProvider functions, IKnownOperatorsProvider operators) {

			knownConstants = constants;
			knownFunctions = functions;
			knownOperators = operators;
		}


		public IExpression Compile(Ast.Expression expr, IMessageLogger logger) {
			var visitor = new Visitor(this, logger);
			return visitor.Compile(expr);
		}


		public enum Message {

			[Message(MessageType.Error, "Expression compiler internal error. {0}")]
			InternalError,
			[Message(MessageType.Error, "Unexpected end of expression.")]
			UnexcpectedEndOfExpression,
			[Message(MessageType.Error, "Unexpected operand `{0}`, expecting operator.")]
			UnexcpectedOperand,
			[Message(MessageType.Error, "Unexpected `{0}` operator, expecting operand.")]
			UnexcpectedOperator,
			[Message(MessageType.Error, "Too few operands in expression.")]
			TooFewOperands,
			[Message(MessageType.Error, "Too many operands in expression.")]
			TooManyOperands,
			[Message(MessageType.Error, "Unknown unary operator `{0}`.")]
			UnknownUnaryOperator,
			[Message(MessageType.Error, "Unknown binary operator `{0}`.")]
			UnknownBinaryOperator,

		}

	}
}
