/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys {
	/// <summary>
	/// Implementation if IMessageLogger interface which do not log anything.
	/// </summary>
	public class DullMessageLogger : IMessageLogger {

		private DullMessageLoggerBlock block = new DullMessageLoggerBlock();



		public bool ErrorOccurred { get { return false; } }

		public void LogMessage(string msgId, MessageType type, PositionRange pos, string message) { }

		public IMessageLoggerBlock StartErrorLoggingBlock() {
			return block;
		}


		private class DullMessageLoggerBlock : IMessageLoggerBlock {

			public bool ErrorOccurred { get { return false; } }

			public void Dispose() { }

		}
	}


}
