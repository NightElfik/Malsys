using System.Collections.Generic;

namespace Malsys.Compilers {

	public interface ICompiler<TSource, TResult> {

		TResult Compile(TSource obj, IMessageLogger logger);

	}

	public static class ICompilerExtensions {

		/// <summary>
		/// Compiles given list with this compiler to immutable list.
		/// </summary>
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
