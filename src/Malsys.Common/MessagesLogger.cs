
namespace Malsys {
	public abstract class MessagesLogger<T> {

		protected MessagesCollection messages;


		public MessagesLogger(MessagesCollection msgs) {
			messages = msgs;
		}

		public abstract string GetMessageTypeId(T msgType);


		protected void logMessage(T msgType, Position pos, params object[] args) {

			MessageType type;
			var msgStr = resolveMessage(msgType, out type, args);
			string id = GetMessageTypeId(msgType);

			messages.LogMessage<T>(id, type, pos, msgStr);
		}

		protected void logMessage(string msgId, MessageType msgType, string message) {
			messages.LogMessage<T>(msgId, msgType, message);
		}

		protected void logMessage(string msgId, MessageType msgType, Position pos, string message) {
			messages.LogMessage<T>(msgId, msgType, pos, message);
		}


		protected abstract string resolveMessage(T msgType, out MessageType type, params object[] args);

	}
}
