using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Malsys {
	public class MessageLogger : IMessageLogger, IEnumerable<Message> {

		private List<Message> messages = new List<Message>();


		public MessageLogger() {
			ErrorOcured = false;
		}


		public bool ErrorOcured { get; private set; }

		public int Count { get { return messages.Count; } }


		public void AddMessage(Message msg) {
			if (msg.Type == MessageType.Error) {
				ErrorOcured = true;
			}

			messages.Add(msg);
		}

		public void LogMessage(string msgId, MessageType type, Position pos, string message) {
			AddMessage(new Message(msgId, type, message, pos, DateTime.Now));
		}

		public string AllMessagesToFullString() {
			var sb = new StringBuilder();
			foreach (var msg in messages) {
				sb.AppendLine(msg.GetFullMessage());
			}
			return sb.ToString();
		}


		public override string ToString() {
			return AllMessagesToFullString();
		}



		IEnumerator IEnumerable.GetEnumerator() {
			return messages.GetEnumerator();
		}

		public IEnumerator<Message> GetEnumerator() {
			return messages.GetEnumerator();
		}

	}
}
