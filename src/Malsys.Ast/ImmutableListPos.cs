using System.Collections.Generic;

namespace Malsys.Ast {
	public class ImmutableListPos<T> : ImmutableList<T>, IToken {

		public readonly Position BeginSeparator;
		public readonly Position EndSeparator;


		public ImmutableListPos(Position pos)
			: this(Position.Unknown, Position.Unknown, pos) {
		}

		public ImmutableListPos(Position beginSep, Position endSep, Position pos)
			: base(ImmutableList<T>.Empty) {

			BeginSeparator = beginSep;
			EndSeparator = endSep;
			Position = pos;
		}

		public ImmutableListPos(IEnumerable<T> vals, Position pos)
			: this(vals, Position.Unknown, Position.Unknown, pos) {
		}

		public ImmutableListPos(IEnumerable<T> vals, Position beginSep, Position endSep, Position pos)
			: base(vals) {

			BeginSeparator = beginSep;
			EndSeparator = endSep;
			Position = pos;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion
	}
}
