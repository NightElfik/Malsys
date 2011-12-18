using System;
using System.Diagnostics.Contracts;

namespace Malsys {
	public interface IMessageLogger {

		bool ErrorOcured { get; }

		void LogMessage(string msgId, MessageType type, Position pos, string message);

	}


	public static class IMessageLoggerExtensions {

		public static void LogError(this IMessageLogger logger, string msgId, string message, params object[] args) {
			logger.LogMessage(msgId, MessageType.Error, Position.Unknown, message.Fmt(args));
		}

		public static void LogError(this IMessageLogger logger, string msgId, Position pos, string message, params object[] args) {
			logger.LogMessage(msgId, MessageType.Error, pos, message.Fmt(args));
		}

		public static void LogMessage<TEnum>(this IMessageLogger logger, TEnum msgIdEnum, params object[] args) where TEnum : struct {
			LogMessage(logger, msgIdEnum, Position.Unknown, args);
		}

		public static void LogMessage<TEnum>(this IMessageLogger logger, TEnum msgIdEnum, Position pos, params object[] args) where TEnum : struct {

			Contract.Requires<ArgumentException>(typeof(TEnum).IsEnum, "T must be an enumerated type.");

			string msgId = typeof(TEnum).FullName;
			string message;
			MessageType msgType;

			var msgAttr = EnumHelper.GetAttrFromEnumVal<TEnum, MessageAttribute>(msgIdEnum);
			if (msgAttr != null) {
				message = msgAttr.Message.Fmt(args);
				msgType = msgAttr.MessageType;
			}
			else {
				message = "Unknown message.";
				msgType = MessageType.Error;
			}

			logger.LogMessage(msgId, msgType, pos, message);
		}

	}
}
