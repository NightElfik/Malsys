// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class Comment : IAstNode {

		public readonly string Text;


		public Comment(string text, PositionRange pos) {
			Text = text;
			Position = pos;
		}


		public PositionRange Position { get; private set; }

	}
}
