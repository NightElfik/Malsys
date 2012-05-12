/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.Ast {

	/// <remarks>
	/// For easier implementation of parser.
	/// Rules returning various statement types can return this type.
	/// </remarks>
	public interface IStatement : IToken { }

}
