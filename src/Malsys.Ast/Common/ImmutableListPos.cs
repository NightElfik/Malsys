using System.Collections.Generic;

namespace Malsys.Ast {
	public class ImmutableListPos<T> : ImmutableList<T>, IToken where T : IToken {

		new public static readonly ImmutableListPos<T> Empty = new ImmutableListPos<T>(Position.Unknown);


		public readonly Position BeginSeparator;
		public readonly Position EndSeparator;

		#region Constructors

		public ImmutableListPos(ImmutableListPos<T> immutableValuesPos)
			: base(immutableValuesPos) {

				BeginSeparator = immutableValuesPos.BeginSeparator;
				EndSeparator = immutableValuesPos.EndSeparator;
				Position = immutableValuesPos.Position;
		}

		public ImmutableListPos(ImmutableList<T> immutableValues, Position beginSep, Position endSep, Position pos)
			: base(immutableValues) {

			BeginSeparator = beginSep;
			EndSeparator = endSep;
			Position = pos;
		}

		public ImmutableListPos(ImmutableList<T> immutableValues, Position pos)
			: this(immutableValues, Position.Unknown, Position.Unknown, pos) {
		}

		public ImmutableListPos(Position pos)
			: this(ImmutableList<T>.Empty, Position.Unknown, Position.Unknown, pos) {
		}

		public ImmutableListPos(Position beginSep, Position endSep, Position pos)
			: this(ImmutableList<T>.Empty, beginSep, endSep, pos) {
		}

		public ImmutableListPos(List<T> vals, Position pos)
			: this(vals, Position.Unknown, Position.Unknown, pos) {
		}

		public ImmutableListPos(List<T> vals, Position beginSep, Position endSep, Position pos)
			: base(vals) {

			BeginSeparator = beginSep;
			EndSeparator = endSep;
			Position = pos;
		}

		#endregion



		public ImmutableListPos<T> AddSeparators(Position beginSep, Position endSep) {
			return new ImmutableListPos<T>(this, beginSep, endSep, this.Position);
		}

		public ImmutableListPos<T> AddSeparators(Position beginSep, Position endSep, Position pos) {
			return new ImmutableListPos<T>(this, beginSep, endSep, pos);
		}


		#region IToken Members

		public Position Position { get; private set; }

		#endregion
	}
}
