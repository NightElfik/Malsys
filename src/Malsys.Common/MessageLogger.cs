using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace Malsys {
	public class MessageLogger : IEnumerable<Message> {

		public bool ErrorOcured { get; private set; }

		public string DefaultSourceName { get; set; }

		private List<Message> messages = new List<Message>();


		public MessageLogger() {
			ErrorOcured = false;
		}

		public void AddMessage(Message msg) {
			if (msg.Type == MessageType.Error) {
				ErrorOcured = true;
			}

			messages.Add(msg);
		}

		public void LogMessage<TReporter>(string msgId, MessageType msgType, string message) {
			LogMessage<TReporter>(msgId, msgType, Position.Unknown, message);
		}

		public void LogMessage<TEnum>(TEnum msgId, params object[] args) where TEnum : struct {
			LogMessage(msgId, Position.Unknown, args);
		}

		public void LogMessage<TEnum>(TEnum msgId, Position pos, params object[] args) where TEnum : struct {

			Contract.Requires<ArgumentException>(typeof(TEnum).IsEnum, "T must be an enumerated type.");

			string message;
			MessageType msgType;

			var msgAttr = AttributeHelper.GetAttribute<MessageAttribute>(msgId);
			if (msgAttr != null) {
				message = msgAttr.Message.Fmt(args);
				msgType = msgAttr.MessageType;
			}
			else {
				message = "Unknown message.";
				msgType = MessageType.Error;
			}

			string id = typeof(TEnum).FullName;
			var msg = new Message(id, msgType, message, DefaultSourceName, pos);
			AddMessage(msg);
		}

		public void LogMessage<TReporter>(string msgId, MessageType msgType, Position pos, string message) {

			string id = typeof(TReporter).Name + "." + msgId;
			var msg = new Message(id, msgType, message, DefaultSourceName, pos);
			AddMessage(msg);
		}


		public override string ToString() {

			var sb = new StringBuilder();
			foreach (var msg in messages) {
				sb.AppendLine(msg.GetFullMessage());
			}
			return sb.ToString();
		}



		IEnumerator IEnumerable.GetEnumerator() {
			return messages.GetEnumerator();
		}

		public IEnumerator<Message> GetEnumerator() {
			return messages.GetEnumerator();
		}

	}
}
