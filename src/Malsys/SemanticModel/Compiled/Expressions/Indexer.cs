
namespace Malsys.SemanticModel.Compiled.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Indexer : IExpression {


		public readonly IExpression Array;

		public readonly IExpression Index;

		public readonly Ast.ExpressionIndexer AstNode;


		public Indexer(IExpression array, IExpression index, Ast.ExpressionIndexer astNode) {

			Array = array;
			Index = index;

			AstNode = astNode;
		}



		public bool IsEmptyExpression { get { return false; } }


		public void Accept(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}

	}
}
