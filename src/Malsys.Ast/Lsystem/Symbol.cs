using System.Collections.Generic;

namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public abstract class Symbol<T> : IToken where T : IToken {

		public readonly string Name;
		public readonly ImmutableListPos<T> Arguments;

		public Symbol(string name, Position pos){
			Name = name;
			Arguments = new ImmutableListPos<T>(pos.GetEndPos());

			Position = pos;
		}

		public Symbol(string name, ImmutableListPos<T> args, Position pos) {
			Name = name;
			Arguments = args;

			Position = pos;	
		}

		#region IToken Members

		public Position Position { get; private set; }

		#endregion

		#region IAstVisitable Members

		public void Accept(IAstVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}
