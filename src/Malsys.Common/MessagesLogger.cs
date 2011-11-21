
namespace Malsys {
	public abstract class MessagesLogger<T> {

		protected MessagesCollection messages;

		protected virtual string reporterName { get { return this.GetType().Name; } }


		public MessagesLogger(MessagesCollection msgs) {
			messages = msgs;
		}

		public abstract string GetMessageTypeId(T msgType);


		protected void logMessage(T msgType, Position pos, params object[] args) {

			MessageType type;
			var msgStr = resolveMessage(msgType, out type, args);
			string id = reporterName + "." + GetMessageTypeId(msgType);
			var msg = new Message(id, type, msgStr, messages.DefaultSourceName, pos);
			messages.AddMessage(msg);
		}


		protected abstract string resolveMessage(T msgType, out MessageType type, params object[] args);

	}
}
