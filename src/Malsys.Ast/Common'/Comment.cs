﻿
namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class Comment : IToken {

		public readonly string Text;


		public Comment(string text, Position pos) {
			Text = text;
			Position = pos;
		}


		public Position Position { get; private set; }

	}
}
