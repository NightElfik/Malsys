
namespace Malsys.Compilers {
	public class CompilerResult<TData> where TData : class {

		public static readonly CompilerResult<TData> Error = new CompilerResult<TData>(null, false);


		public bool ErrorOcured { get; private set; }

		public TData Result { get; private set; }


		internal CompilerResult(TData rslt) {
			Result = rslt;
			ErrorOcured = false;
		}

		internal CompilerResult(TData rslt, bool err) {
			Result = rslt;
			ErrorOcured = err;
		}

		/// <summary>
		/// Returns true if no error ocured.
		/// </summary>
		public static implicit operator bool(CompilerResult<TData> cr) {
			return !cr.ErrorOcured;
		}

		/// <summary>
		/// Raturns data (result).
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
