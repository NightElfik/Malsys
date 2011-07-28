
namespace Malsys.Compilers {
	public class CompilerResult {

		public bool ErrorOcured { get { return Messages.ErrorOcured; } }

		public MessagesCollection Messages { get; private set; }

		public InputBlock Result { get; private set; }


		internal CompilerResult(MessagesCollection msgs, InputBlock result) {
			Messages = msgs;
			Result = result;
		}
	}
}
