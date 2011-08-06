using System;

namespace Malsys.SourceCode.Highlighters {
	class PositionMark : IComparable<PositionMark> {

		private static uint InstCounter = 0;


		public readonly uint Generation = InstCounter++;

		public int Line;
		public int Column;
		public bool Begin;
		public MarkType Type;


		public PositionMark(MarkType type, int line, int col, bool begin) {
			Type = type;
			Line = line;
			Column = col;
			Begin = begin;
		}


		#region IComparable<PositionMark> Members

		public int CompareTo(PositionMark other) {
			if (Line != other.Line) {
				return Line.CompareTo(other.Line);
			}

			if (Column != other.Column) {
				return Column.CompareTo(other.Column);
			}

			return Generation.CompareTo(other.Generation);
		}

		#endregion
	}
}
