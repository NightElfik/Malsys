/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.Diagnostics.Contracts;

namespace Malsys.Ast {
	public interface IToken {

		Position Position { get; }

	}
}

namespace Malsys {

	/// <summary>
	/// This class is in Malsys namespace to be callable without including Malsys.Ast namespace.
	/// </summary>
	public static class ITokenExtensions {

		/// <summary>
		/// Tries to get position from instance.
		/// Returns unknown position even if instance is null or position in instance is null.
		/// </summary>
		public static Position TryGetPosition(this Ast.IToken instance) {

			Contract.Ensures(Contract.Result<Position>() != null);

			if (instance != null) {
				return instance.Position ?? Position.Unknown;
			}
			else {
				return Position.Unknown;
			}
		}

	}

}
