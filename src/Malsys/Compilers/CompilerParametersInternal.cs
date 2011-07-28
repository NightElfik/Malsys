
namespace Malsys.Compilers {
	public class CompilerParametersInternal {

		public CompilerParameters Params { get; private set; }

		public MessagesCollection Messages { get; private set; }


		public CompilerParametersInternal(CompilerParameters prms) {
			Params = prms;
			Messages = new MessagesCollection();
		}

		public CompilerParametersInternal(MessagesCollection msgs, CompilerParameters prms) {
			Params = prms;
			Messages = msgs;
		}
	}
}
