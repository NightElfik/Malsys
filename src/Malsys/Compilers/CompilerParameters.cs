
namespace Malsys.Compilers {
	public class CompilerParameters {

		public bool CaseSensitiveVarsNames { get; set; }
		public bool CaseSensitiveFunsNames { get; set; }
		public bool CaseSensitiveLsystemNames { get; set; }

		public MessagesCollection Messages { get; private set; }


		public CompilerParameters() {
			Messages = new MessagesCollection();
		}

		internal CompilerParameters(MessagesCollection msgs) {
			Messages = msgs;
		}
	}
}
