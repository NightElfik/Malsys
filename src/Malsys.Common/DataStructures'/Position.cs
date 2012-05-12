/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using LexPos = Microsoft.FSharp.Text.Lexing.Position;

namespace Malsys {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	// TODO: rename to something better like range
	public class Position {

		public static readonly Position Unknown = new Position(-1, -1, -1, -1);


		public readonly int BeginLine;
		public readonly int BeginColumn;
		public readonly int EndLine;
		public readonly int EndColumn;

		public readonly string SourceName;


		public Position(int beginLine, int beginColumn, int endLine, int endColumn, string sourceName = "") {
			BeginLine = beginLine;
			BeginColumn = beginColumn;
			EndLine = endLine;
			EndColumn = endColumn;
			SourceName = sourceName;
		}

		public Position(LexPos begin, LexPos end) {
			BeginLine = begin.Line;
			BeginColumn = begin.Column;
			EndLine = end.Line;
			EndColumn = end.Column;
			SourceName = begin.FileName;
		}

		public Position(Tuple<LexPos, LexPos> range) {
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

		public Position ToNonZeroLength() {
			if (IsUnknown || !IsZeroLength) {
				return this;
			}

			return new Position(BeginLine, BeginColumn, EndLine, EndColumn + 1);
		}


		public Position GetBeginPos() {
			return new Position(BeginLine, BeginColumn, BeginLine, BeginColumn, SourceName);
		}

		public Position GetEndPos() {
			return new Position(EndLine, EndColumn, EndLine, EndColumn, SourceName);
		}

		public override string ToString() {
			return "({0},{1}) - ({2},{3})".Fmt(BeginLine, BeginColumn, EndLine, EndColumn);
		}
	}
}
