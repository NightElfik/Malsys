using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Malsys {
	public class MessagesCollection : IEnumerable<Message> {

		public bool ErrorOcured { get; private set; }

		public string DefaultSourceName { get; set; }

		private List<Message> messages = new List<Message>();


		public MessagesCollection() {
			ErrorOcured = false;
		}

		public void AddMessage(Message msg) {
			if (msg.Type == MessageType.Error) {
				ErrorOcured = true;
			}

			messages.Add(msg);
		}

		public void AddMessage(string message, MessageType type, Position pos) {
			if (type == MessageType.Error) {
				AddError(message, pos);
				return;
			}

			messages.Add(new Message(message, type, DefaultSourceName, pos));
		}

		public void AddError(string message, Position pos) {
			ErrorOcured = true;

			messages.Add(new Message(message, MessageType.Error, DefaultSourceName, pos));
		}

		public void AddError(string message, Position pos, params Position[] otherPos) {
			ErrorOcured = true;

			messages.Add(new Message(message, MessageType.Error, DefaultSourceName, pos, otherPos));
		}


		public override string ToString() {

			var sb = new StringBuilder();
			foreach (var msg in messages) {
				sb.AppendLine(msg.GetFullMessage());
			}
			return sb.ToString();
		}


		#region IEnumerable Members

		IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return messages.GetEnumerator();
		}

		#endregion

		#region IEnumerable<CompilerMessage> Members

		public IEnumerator<Message> GetEnumerator() {
			return messages.GetEnumerator();
		}

		#endregion
	}
}
