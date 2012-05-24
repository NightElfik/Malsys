/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using LexPos = Microsoft.FSharp.Text.Lexing.Position;

namespace Malsys {
	/// <summary>
	/// Stores the position range (line and columns) and the source name.
	/// </summary>
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class PositionRange {

		public static readonly PositionRange Unknown = new PositionRange(-1, -1, -1, -1);


		public readonly int BeginLine;
		public readonly int BeginColumn;
		public readonly int EndLine;
		public readonly int EndColumn;

		public readonly string SourceName;


		public PositionRange(int beginLine, int beginColumn, int endLine, int endColumn, string sourceName = "") {
			BeginLine = beginLine;
			BeginColumn = beginColumn;
			EndLine = endLine;
			EndColumn = endColumn;
			SourceName = sourceName;
		}

		public PositionRange(LexPos begin, LexPos end) {
			BeginLine = begin.Line;
			BeginColumn = begin.Column;
			EndLine = end.Line;
			EndColumn = end.Column;
			SourceName = begin.FileName;
		}

		public PositionRange(Tuple<LexPos, LexPos> range) {
			BeginLine = range.Item1.Line;
			BeginColumn = range.Item1.Column;
			EndLine = range.Item2.Line;
			EndColumn = range.Item2.Column;
			SourceName = range.Item1.FileName;
		}


		public bool IsUnknown {
			get {
				return BeginLine < 0 || BeginColumn < 0 || EndLine < 0 || EndColumn < 0;
			}
		}

		public bool IsZeroLength {
			get {
				return BeginLine == EndLine && BeginColumn == EndColumn;
			}
		}

		public PositionRange ToNonZeroLength() {
			if (IsUnknown || !IsZeroLength) {
				return this;
			}

			return new PositionRange(BeginLine, BeginColumn, EndLine, EndColumn + 1);
		}


		public PositionRange GetBeginPos() {
			return new PositionRange(BeginLine, BeginColumn, BeginLine, BeginColumn, SourceName);
		}

		public PositionRange GetEndPos() {
			return new PositionRange(EndLine, EndColumn, EndLine, EndColumn, SourceName);
		}

		public override string ToString() {
			return "({0},{1}) - ({2},{3})".Fmt(BeginLine, BeginColumn, EndLine, EndColumn);
		}
	}
}
