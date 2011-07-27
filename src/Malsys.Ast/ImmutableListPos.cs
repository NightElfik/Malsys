using System.Collections.Generic;

namespace Malsys.Ast {
	public class ImmutableListPos<T> : ImmutableList<T>, IToken {

		public ImmutableListPos(Position pos)
			:base(ImmutableList<T>.Empty) {

			Position = pos;
		}

		public ImmutableListPos(IEnumerable<T> vals, Position pos)
			: base(vals) {

			Position = pos;
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion
	}
}
