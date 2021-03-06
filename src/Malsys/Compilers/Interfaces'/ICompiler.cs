﻿using System.Collections.Generic;
using System.Linq;

namespace Malsys.Compilers {

	/// <summary>
	/// Compiler which is capable compiling <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.
	/// </summary>
	/// <typeparam name="TSource">Source type.</typeparam>
	/// <typeparam name="TResult">Result type.</typeparam>
	public interface ICompiler<TSource, TResult> {

		TResult Compile(TSource obj, IMessageLogger logger);

	}


	public static class ICompilerExtensions {

		/// <summary>
		/// Compiles given list with this compiler to a list.
		/// </summary>
		/// <remarks>
		/// All compilers which are capable to compile single entry can be extended to compile list.
		/// If any compiler needs to compile list in the other way it should define its own CompileList method.
		/// </remarks>
		public static List<TResult> CompileList<TSource, TResult>(this ICompiler<TSource, TResult> compiler, IEnumerable<TSource> list, IMessageLogger logger) {
			return list.Select(x => compiler.Compile(x, logger)).ToList();
		}

	}
}
