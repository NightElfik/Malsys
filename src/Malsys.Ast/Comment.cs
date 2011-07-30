﻿
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Comment : IToken {

		public readonly string Text;


		public Comment(string text, Position pos) {
			Text = text;
			Position = pos;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion
	}
}
