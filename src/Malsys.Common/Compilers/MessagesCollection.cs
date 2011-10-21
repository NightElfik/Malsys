using System.Collections.Generic;
using System.Text;

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
				AddError(message, pos);
				return;
			}

			messages.Add(new CompilerMessage(message, type, DefaultSourceName, pos));
		}

		public void AddError(string message, Position pos) {
			ErrorOcured = true;

			messages.Add(new CompilerMessage(message, CompilerMessageType.Error, DefaultSourceName, pos));
		}

		public void AddError(string message, Position pos, params Position[] otherPos) {
			ErrorOcured = true;

			messages.Add(new CompilerMessage(message, CompilerMessageType.Error, DefaultSourceName, pos, otherPos));
		}


		public override string ToString() {
			var sb = new StringBuilder();
			foreach (var msg in messages) {
				sb.AppendLine(msg.GetFullMessage());
			}
			return sb.ToString();
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
