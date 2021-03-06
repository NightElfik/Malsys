﻿
namespace Malsys.Processing.Components {
	/// <summary>
	/// Rewriters are responsible for rewriting L-system symbols by defined rewrite rules.
	/// </summary>
	/// <name>Symbol rewriter interface</name>
	/// <group>Rewriters</group>
	public interface IRewriter : ISymbolProvider {

		ISymbolProvider SymbolProvider { set; }

	}
}
