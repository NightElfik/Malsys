using System;
using LexPos = Microsoft.FSharp.Text.Lexing.Position;

namespace Malsys.Ast {
	public interface IToken {
		Position Position { get; }
	}

	/// <summary>
	/// Immutable.
	/// </summary>
	public struct Position {
		public static readonly Position Unknown = new Position(-1, -1, -1, -1);

		public readonly int BeginLine;
		public readonly int BeginColumn;
		public readonly int EndLine;
		public readonly int EndColumn;


		public Position(int beginLine, int beginColumn, int endLine, int endColumn) {
			BeginLine = beginLine;
			BeginColumn = beginColumn;
			EndLine = endLine;
			EndColumn = endColumn;
		}

		public Position(LexPos begin, LexPos end) {
			BeginLine = begin.Line;
			BeginColumn = begin.Column;
			EndLine = end.Line;
			EndColumn = end.Column;
		}

		public Position(Tuple<LexPos, LexPos> range) {
			BeginLine = range.Item1.Line;
			BeginColumn = range.Item1.Column;
			EndLine = range.Item2.Line;
			EndColumn = range.Item2.Column;
		}
	}
}
