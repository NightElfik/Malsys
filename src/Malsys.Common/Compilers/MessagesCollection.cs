using System.Collections.Generic;

namespace Malsys.Compilers {
	public class MessagesCollection : IEnumerable<CompilerMessage> {

		public bool ErrorOcured { get; private set; }

		public string DefaultSourceName { get; set; }

		private List<CompilerMessage> messages = new List<CompilerMessage>();


		public MessagesCollection() {
			ErrorOcured = false;
		}

		public void AddMessage(CompilerMessage msg) {
			if (msg.Type == CompilerMessageType.Error) {
				ErrorOcured = true;
			}

			messages.Add(msg);
		}

		public void AddMessage(string message, CompilerMessageType type, Position pos) {
			if (type == CompilerMessageType.Error) {
				ErrorOcured = true;
			}

			messages.Add(new CompilerMessage(message, type, DefaultSourceName, pos));
		}


		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return messages.GetEnumerator();
		}

		#endregion

		#region IEnumerable<CompilerMessage> Members

		public IEnumerator<CompilerMessage> GetEnumerator() {
			return messages.GetEnumerator();
		}

		#endregion
	}
}
