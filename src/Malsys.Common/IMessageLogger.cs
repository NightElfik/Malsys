﻿/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using System.Diagnostics.Contracts;

namespace Malsys {
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

		public static void LogMessage<TEnum>(this IMessageLogger logger, TEnum msgIdEnum, params object[] args) where TEnum : struct {
			LogMessage(logger, msgIdEnum, PositionRange.Unknown, args);
		}

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
