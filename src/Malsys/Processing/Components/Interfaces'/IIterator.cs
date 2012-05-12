/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using Malsys.SemanticModel;

namespace Malsys.Processing.Components {
	/// <summary>
	///	Iterators are responsible for iterating L-system to desired number of iterations.
	/// </summary>
	/// <name>Iterator interface</name>
	/// <group>Iterators</group>
	public interface IIterator : ISymbolProvider, IProcessStarter {

		/// <summary>
		/// Iterator iterates symbols by reading all symbols from SymbolProvider every iteration.
		/// Usually Rewriter is connected as SymbolProvider and rewriters's SymbolProvider is Iterator.
		/// This setup creates loop and iterator rewrites string of symbols every iteration.
		/// </summary>
		[UserConnectable]
		ISymbolProvider SymbolProvider { set; }

		/// <summary>
		/// Axiom provider component provides initial string of symbols.
		/// </summary>
		[UserConnectable]
		ISymbolProvider AxiomProvider { set; }

		/// <summary>
		/// Result string of symbols is sent to connected output processor.
		/// Usually it is InterpretrCaller who calls Interpreter and interprets symbols.
		/// </summary>
		[UserConnectable]
		ISymbolProcessor OutputProcessor { set; }

		/// <summary>
		/// Number of iterations to do with current L-system.
		/// </summary>
		[UserSettable]
		Constant Iterations { set; }
	}
}
