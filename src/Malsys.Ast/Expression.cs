
namespace Malsys.Ast {
	public class Expression : Token, IAstVisitable {
		public readonly ExpressionNode RootNode;

		public Expression(ExpressionNode rootNode, int beginLine, int beginColumn, int endLine, int endColumn)
			: base(beginLine, beginColumn, endLine, endColumn) {

			RootNode = rootNode;
		}

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}

	public abstract class ExpressionNode : Token, IAstVisitable {
		public ExpressionNode(int beginLine, int beginColumn, int endLine, int endColumn)
			: base(beginLine, beginColumn, endLine, endColumn) { }

		#region IAstVisitable Members

		public abstract void Accept(IAstVisitor visitor);

		#endregion
	}

	public class ParenthesisExpressionNode : ExpressionNode {
		public readonly ExpressionNode Node;

		public ParenthesisExpressionNode(ExpressionNode node, int beginLine, int beginColumn, int endLine, int endColumn)
			: base(beginLine, beginColumn, endLine, endColumn) {

			Node = node;
		}

		public override void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}
	}

	public class ConstantExpressionNode : ExpressionNode {
		public readonly double Value;

		public ConstantExpressionNode(double value, int beginLine, int beginColumn, int endLine, int endColumn)
			: base(beginLine, beginColumn, endLine, endColumn) {
			Value = value;
		}

		public override void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}
	}

	public class VariableExpressionNode : ExpressionNode {
		public readonly string Name;

		public VariableExpressionNode(string name, int beginLine, int beginColumn, int endLine, int endColumn)
			: base(beginLine, beginColumn, endLine, endColumn) {

			Name = name;
		}

		public override void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
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

		public override void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}
	}
}
