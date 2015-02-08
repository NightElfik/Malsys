using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Malsys {
	/// <summary>
	/// Standard implementation of the IMessageLogger interface.
	/// Logs messages to the Message class.
	/// </summary>
	public class MessageLogger : IMessageLogger, IEnumerable<Message> {

		private bool errorOccurred = false;

		private List<Message> messages = new List<Message>();
		private List<LoggingBlock> openedBlocks = new List<LoggingBlock>();



		#region IMessageLogger Members

		public bool ErrorOccurred { get { return errorOccurred; } }

		public void LogMessage(string msgId, MessageType type, PositionRange pos, string message) {
			AddMessage(new Message(msgId, type, message, pos, DateTime.Now));
		}

		public IMessageLoggerBlock StartErrorLoggingBlock() {
			var block = new LoggingBlock(this);
			openedBlocks.Add(block);
			return block;
		}

		#endregion


		public int Count { get { return messages.Count; } }


		public void AddMessage(Message msg) {
			if (msg.Type == MessageType.Error) {
				setError();
			}

			messages.Add(msg);
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


		private void setError() {

			errorOccurred = true;

			foreach (var block in openedBlocks) {
				block.SetError();
			}

		}


		private class LoggingBlock : IMessageLoggerBlock {

			private bool errorOccurred = false;
			private MessageLogger parent;


			public LoggingBlock(MessageLogger parentLogger) {
				parent = parentLogger;
			}


			public bool ErrorOccurred { get { return errorOccurred; } }


			public void SetError() {
				errorOccurred = true;
			}

			public void Dispose() {
				parent.openedBlocks.Remove(this);
			}

		}


	}
}
