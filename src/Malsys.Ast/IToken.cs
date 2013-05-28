// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Diagnostics.Contracts;

namespace Malsys.Ast {
	public interface IAstNode {

		PositionRange Position { get; }

	}
}

namespace Malsys {

	/// <summary>
	/// This class is in Malsys namespace to be callable without including Malsys.Ast namespace.
	/// </summary>
	public static class IAstNodeExtensions {

		/// <summary>
		/// Tries to get position from the AstNode.
		/// Because this is extension method it is safe to call it on potentially null object.
		/// Returns unknown position even if instance is null or if position in instance is null.
		/// </summary>
		public static PositionRange TryGetPosition(this Ast.IAstNode instance) {

			Contract.Ensures(Contract.Result<PositionRange>() != null);

			if (instance != null) {
				return instance.Position ?? PositionRange.Unknown;
			}
			else {
				return PositionRange.Unknown;
			}
		}

	}

}
