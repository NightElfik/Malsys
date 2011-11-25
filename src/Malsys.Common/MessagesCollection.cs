using System.Collections;
using System.Collections.Generic;
using System.Text;

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

		public void LogMessage<TReporter>(string msgId, MessageType msgType, Position pos, string message) {

			string id = typeof(TReporter).Name + "." + msgId;
			var msg = new Message(id, msgType, message, DefaultSourceName, pos);
			AddMessage(msg);
		}

		public void LogMessage<TReporter>(string msgId, MessageType msgType, string message) {
			LogMessage<TReporter>(msgId, msgType, Position.Unknown, message);
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
