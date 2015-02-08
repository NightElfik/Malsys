using System.Diagnostics.Contracts;

namespace Malsys.Ast {
	public interface IAstNode {

		PositionRange Position { get; }

	}
}

namespace Malsys {

	/// <remarks>
	/// This extension class is in Malsys namespace to be callable without including Malsys.Ast namespace.
	/// </remarks>
	public static class IAstNodeExtensions {

		/// <summary>
		/// Tries to get position from the AstNode.
		/// This method is extension because it is safe to call it on potentially null object - I ♥ C# :).
		/// </summary>
		/// <returns>AST position even if instance is null or if position in instance is null.</returns>
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
