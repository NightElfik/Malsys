
namespace Malsys.Compilers {
	public class ExpressionCompilerParameters {
		public bool CaseSensitiveVarsNames { get; set; }
		public bool CaseSensitiveFunsNames { get; set; }

		public MessagesCollection Messages { get; private set; }

		public ExpressionCompilerParameters(MessagesCollection msgs) {
			Messages = msgs;
		}
	}
}
