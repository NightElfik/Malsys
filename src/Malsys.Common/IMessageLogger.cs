/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using System.Diagnostics.Contracts;

namespace Malsys {
	/// <summary>
	/// General interface for logging of messages.
	/// </summary>
	public interface IMessageLogger {

		bool ErrorOccurred { get; }

		void LogMessage(string msgId, MessageType type, PositionRange pos, string message);


		IMessageLoggerBlock StartErrorLoggingBlock();

	}

	public interface IMessageLoggerBlock : IDisposable {

		bool ErrorOccurred { get; }

	}


	public static class IMessageLoggerExtensions {

		public static void LogInfo(this IMessageLogger logger, string msgId, string message, params object[] args) {
			logger.LogMessage(msgId, MessageType.Info, PositionRange.Unknown, message.Fmt(args));
		}

		public static void LogError(this IMessageLogger logger, string msgId, string message, params object[] args) {
			logger.LogMessage(msgId, MessageType.Error, PositionRange.Unknown, message.Fmt(args));
		}

		public static void LogError(this IMessageLogger logger, string msgId, PositionRange pos, string message, params object[] args) {
			logger.LogMessage(msgId, MessageType.Error, pos, message.Fmt(args));
		}

		/// <summary>
		/// Logs message with ID given by enumeration full name and data are taken from its Message attribute.
		/// </summary>
		/// <typeparam name="TEnum">Enum type. Its values should be marked with Message attribute.</typeparam>
		/// <param name="logger">The logger instance to log the message.</param>
		/// <param name="msgIdEnum">Enum value marked with Message attribute. All the information about logged message are taken from it.</param>
		/// <param name="args">Arguments that will be replaced in the message text (taken from the Message attribute).</param>
		public static void LogMessage<TEnum>(this IMessageLogger logger, TEnum msgIdEnum, params object[] args) where TEnum : struct {
			LogMessage(logger, msgIdEnum, PositionRange.Unknown, args);
		}

		/// <summary>
		/// Logs message with ID given by enumeration full name and data are taken from its Message attribute.
		/// </summary>
		/// <typeparam name="TEnum">Enum type. Its values should be marked with Message attribute.</typeparam>
		/// <param name="logger">The logger instance to log the message.</param>
		/// <param name="msgIdEnum">Enum value marked with Message attribute. All the information about logged message are taken from it.</param>
		/// <param name="pos">Position</param>
		/// <param name="args">Arguments that will be replaced in the message text (taken from the Message attribute).</param>
		public static void LogMessage<TEnum>(this IMessageLogger logger, TEnum msgIdEnum, PositionRange pos, params object[] args) where TEnum : struct {

			Contract.Requires<ArgumentException>(typeof(TEnum).IsEnum, "T must be an enumerated type.");

			string msgId = EnumToMessageId(msgIdEnum);
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

		public static string EnumToMessageId<TEnum>(TEnum msgIdEnum) {
			return typeof(TEnum).FullName + "#" + msgIdEnum.ToString();
		}
	}
}
