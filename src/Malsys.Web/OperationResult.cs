
namespace Malsys.Web {
	public class OperationResult<T> where T : class {

		public bool Error { get { return ErrorMessage != null; } }

		public string ErrorMessage { get; private set; }

		public T Data { get; private set; }


		public static implicit operator bool(OperationResult<T> opResult) {
			return !opResult.Error;
		}


		public OperationResult(T data) {
			Data = data;
			ErrorMessage = null;
		}

		public OperationResult(string errorMessage) {
			Data = null;
			ErrorMessage = errorMessage;
		}

	}
}