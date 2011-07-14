
namespace Malsys.Ast {
	public class Expression : Token {
		public readonly ExpressionNode RootNode;

		public Expression(ExpressionNode rootNode, int beginLine, int beginColumn, int endLine, int endColumn)
			: base(beginLine, beginColumn, endLine, endColumn) {

			RootNode = rootNode;
		}
	}

	[System.Serializable]
	public class MyException : System.Exception {
		public MyException() { }
		public MyException(string message) : base(message) { }
		public MyException(string message, System.Exception inner) : base(message, inner) { }
		protected MyException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}

	public abstract class ExpressionNode : Token {
		public ExpressionNode(int beginLine, int beginColumn, int endLine, int endColumn)
			: base(beginLine, beginColumn, endLine, endColumn) { }
	}

	public class ParenthesisExpressionNode : ExpressionNode {
		public readonly ExpressionNode Node;

		public ParenthesisExpressionNode(ExpressionNode node, int beginLine, int beginColumn, int endLine, int endColumn)
			: base(beginLine, beginColumn, endLine, endColumn) {

			Node = node;
		}
	}

	public class ConstantExpressionNode : ExpressionNode {
		public readonly double Value;

		public ConstantExpressionNode(double value, int beginLine, int beginColumn, int endLine, int endColumn)
			: base(beginLine, beginColumn, endLine, endColumn) {
			Value = value;
		}
	}

	public class VariableExpressionNode : ExpressionNode {
		public readonly string Name;

		public VariableExpressionNode(string name, int beginLine, int beginColumn, int endLine, int endColumn)
			: base(beginLine, beginColumn, endLine, endColumn) {

			Name = name;
		}
	}

	public class FunctionExpressionNode : ExpressionNode {
		public readonly string Syntax;
		public readonly byte Arity;
		public readonly ExpressionNode[] Arguments;

		public FunctionExpressionNode(string syntax, byte arity, int beginLine, int beginColumn, int endLine, int endColumn, params ExpressionNode[] arguments)
			: base(beginLine, beginColumn, endLine, endColumn) {

			Syntax = syntax;
			Arity = arity;
			Arguments = arguments;
		}
	}
}
