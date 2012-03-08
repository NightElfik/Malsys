
namespace Malsys.Compilers {
	/// <summary>
	/// Class holding result and information about success.
	/// </summary>
	internal class CompilerResult<TData> where TData : class {

		public static readonly CompilerResult<TData> Error = new CompilerResult<TData>(null, true);


		public bool ErrorOccurred { get; private set; }

		public TData Result { get; private set; }


		public CompilerResult(TData rslt) {
			Result = rslt;
			ErrorOccurred = false;
		}

		public CompilerResult(TData rslt, bool err) {
			Result = rslt;
			ErrorOccurred = err;
		}


		/// <summary>
		/// Returns true if no error occurred.
		/// </summary>
		public static implicit operator bool(CompilerResult<TData> cr) {
			return !cr.ErrorOccurred;
		}

		/// <summary>
		/// Returns data (result).
		/// </summary>
		public static implicit operator TData(CompilerResult<TData> cr) {
			return cr.Result;
		}

		/// <summary>
		/// Returns new CompilerResult with given data and no error.
		/// </summary>
		public static implicit operator CompilerResult<TData>(TData data) {
			return new CompilerResult<TData>(data);
		}
	}
}
