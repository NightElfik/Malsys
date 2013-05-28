// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Collections.Generic;

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ImmutableListPos<T> : ImmutableList<T>, IAstNode where T : IAstNode {

		new public static readonly ImmutableListPos<T> Empty = new ImmutableListPos<T>(PositionRange.Unknown);


		public readonly PositionRange BeginSeparator;
		public readonly PositionRange EndSeparator;

		#region Constructors

		public ImmutableListPos(ImmutableListPos<T> immutableValuesPos)
			: base(immutableValuesPos) {

				BeginSeparator = immutableValuesPos.BeginSeparator;
				EndSeparator = immutableValuesPos.EndSeparator;
				Position = immutableValuesPos.Position;
		}

		public ImmutableListPos(ImmutableList<T> immutableValues, PositionRange beginSep, PositionRange endSep, PositionRange pos)
			: base(immutableValues) {

			BeginSeparator = beginSep;
			EndSeparator = endSep;
			Position = pos;
		}

		public ImmutableListPos(ImmutableList<T> immutableValues, PositionRange pos)
			: this(immutableValues, PositionRange.Unknown, PositionRange.Unknown, pos) {
		}

		public ImmutableListPos(PositionRange pos)
			: this(ImmutableList<T>.Empty, PositionRange.Unknown, PositionRange.Unknown, pos) {
		}

		public ImmutableListPos(PositionRange beginSep, PositionRange endSep, PositionRange pos)
			: this(ImmutableList<T>.Empty, beginSep, endSep, pos) {
		}

		public ImmutableListPos(List<T> vals, PositionRange pos)
			: this(vals, PositionRange.Unknown, PositionRange.Unknown, pos) {
		}

		public ImmutableListPos(List<T> vals, PositionRange beginSep, PositionRange endSep, PositionRange pos)
			: base(vals) {

			BeginSeparator = beginSep;
			EndSeparator = endSep;
			Position = pos;
		}

		#endregion



		public ImmutableListPos<T> AddSeparators(PositionRange beginSep, PositionRange endSep) {
			return new ImmutableListPos<T>(this, beginSep, endSep, this.Position);
		}

		public ImmutableListPos<T> AddSeparators(PositionRange beginSep, PositionRange endSep, PositionRange pos) {
			return new ImmutableListPos<T>(this, beginSep, endSep, pos);
		}


		public PositionRange Position { get; private set; }

	}
}
