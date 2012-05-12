/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.Collections.Generic;

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
		/// Compiles given list with this compiler to immutable list.
		/// </summary>
		/// <remarks>
		/// All compilers which are capable to compile single entry can be extended to compile list.
		/// If any compiler needs to compile list in the other way it should define its own CompileList method.
		/// </remarks>
		public static ImmutableList<TResult> CompileList<TSource, TResult>(this ICompiler<TSource, TResult> compiler, IList<TSource> list, IMessageLogger logger) {

			int count = list.Count;
			TResult[] resultArr = new TResult[count];

			for (int i = 0; i < count; i++) {
				resultArr[i] = compiler.Compile(list[i], logger);
			}

			return new ImmutableList<TResult>(resultArr, true);
		}

	}
}
