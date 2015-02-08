
namespace Malsys.Ast {
	public class Comment : IAstNode {

		public string Text;

		public PositionRange Position { get; private set; }


		public Comment(string text, PositionRange pos) {
			Text = text;
			Position = pos;
		}

	}
}
