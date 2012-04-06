
namespace Malsys {
	public class DullMessageLogger : IMessageLogger {

		private DullMessageLoggerBlock block = new DullMessageLoggerBlock();



		public bool ErrorOccurred { get { return false; } }

		public void LogMessage(string msgId, MessageType type, Position pos, string message) { }

		public IMessageLoggerBlock StartErrorLoggingBlock() {
			return block;
		}


		private class DullMessageLoggerBlock : IMessageLoggerBlock {

			public bool ErrorOccurred { get { return false; } }

			public void Dispose() { }

		}
	}


}
