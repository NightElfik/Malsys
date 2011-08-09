﻿using System;
using System.Threading;

namespace Malsys.SourceCode.Highlighters {
	class PositionMark : IComparable<PositionMark> {

		private static int InstCounter = 0;


		public readonly int Generation;

		public MarkType Type;
		public int Line;
		public int Column;
		public bool Begin;
		public int PairId;
		public object[] Data;

		/// <summary>
		/// Returns new position mark with <c>PairId</c> set to <c>Generation</c>.
		/// </summary>
		public PositionMark(MarkType type, int line, int col, bool begin) {

			Generation = Interlocked.Increment(ref InstCounter);

			Type = type;
			Line = line;
			Column = col;
			Begin = begin;
			PairId = Generation;
			Data = null;
		}

		public PositionMark(MarkType type, int line, int col, bool begin, int pairId) {

			Generation = Interlocked.Increment(ref InstCounter);

			Type = type;
			Line = line;
			Column = col;
			Begin = begin;
			PairId = pairId;
			Data = null;
		}


		#region IComparable<PositionMark> Members

		/// <summary>
		/// Comparison is not perfect. Biggest problem is when mark begins and ends on same line and column.
		/// Better way to order marks will be to order them in time of creation (inserting them to serted linked list).
		/// </summary>
		public int CompareTo(PositionMark other) {
			if (Line != other.Line) {
				return Line.CompareTo(other.Line);
			}

			if (Column != other.Column) {
				return Column.CompareTo(other.Column);
			}

			if (Begin ^ other.Begin) {
				return Begin ? 1 : -1;
				return Generation.CompareTo(other.Generation);
			}
			else {
				if (Begin) {
					return Generation.CompareTo(other.Generation);
				}
				else {
					return other.Generation.CompareTo(Generation);
				}
			}
		}

		#endregion
	}
}
