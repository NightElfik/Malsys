// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Collections.Generic;

namespace Malsys.Ast {
	public class ListPos<T> : List<T>, IAstNode where T : IAstNode {

		public PositionRange BeginSeparator;
		public PositionRange EndSeparator;

		public PositionRange Position { get; set; }


		public ListPos() {
			BeginSeparator = PositionRange.Unknown;
			EndSeparator = PositionRange.Unknown;
			Position = PositionRange.Unknown;
		}

		public ListPos(ListPos<T> valuesPos)
			: base(valuesPos) {

			BeginSeparator = valuesPos.BeginSeparator;
			EndSeparator = valuesPos.EndSeparator;
			Position = valuesPos.Position;
		}

		public ListPos(IEnumerable<T> valuesPos, PositionRange beginSep, PositionRange endSep, PositionRange pos)
			: base(valuesPos) {

			BeginSeparator = beginSep;
			EndSeparator = endSep;
			Position = pos;
		}

		public ListPos(PositionRange beginSep, PositionRange endSep, PositionRange pos) {
			BeginSeparator = beginSep;
			EndSeparator = endSep;
			Position = pos;
		}

		public ListPos(IEnumerable<T> valuesPos, PositionRange pos)
			: this(valuesPos, PositionRange.Unknown, PositionRange.Unknown, pos) {
		}

		public ListPos(PositionRange pos) {
			Position = pos;
		}


		public ListPos<T> AddSeparators(PositionRange beginSep, PositionRange endSep) {
			BeginSeparator = beginSep;
			EndSeparator = endSep;
			return this;
		}

		public ListPos<T> AddSeparators(PositionRange beginSep, PositionRange endSep, PositionRange pos) {
			BeginSeparator = beginSep;
			EndSeparator = endSep;
			Position = pos;
			return this;
		}


	}
}
