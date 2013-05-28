// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Media {
	public class ColorParser {

		private IMessageLogger logger;

		public ColorParser() { }

		public ColorParser(IMessageLogger logger) {
			this.logger = logger;
		}

		public bool TryParseColor(IValue value, out ColorF color) {
			return TryParseColor(value, out color, logger);
		}

		public ColorF ParseColor(IValue value, IMessageLogger logger) {
			ColorF color;
			TryParseColor(value, out color, logger);
			return color;
		}

		public bool TryParseColor(IValue value, out ColorF color, IMessageLogger logger) {

			if (value.IsConstant) {

				long val = ((Constant)value).RoundedLongValue;
				if (val < 0L || val > 0xFFFFFFFFL) {
					logger.LogMessage(Message.ColorConnstantOutOfRrange, value.AstPosition);
					color = ColorF.Black;
					return false;
				}

				color = new ColorF((uint)((Constant)value).RoundedLongValue);
				return true;
			}
			else if (value.IsArray) {

				var arr = (ValuesArray)value;

				if (arr.Length != 3 && arr.Length != 4) {
					logger.LogMessage(Message.ExpectedColorAsArrayLen34, arr.AstPosition);
					color = ColorF.Black;
					return false;
				}

				for (int i = 0; i < arr.Length; i++) {
					if (!arr[i].IsConstant) {
						logger.LogMessage(Message.ExpectedConstAtIndexI, value.AstPosition, i);
						color = ColorF.Black;
						return false;
					}
				}

				if (arr.Length == 3) {
					color = new ColorF(((Constant)arr[0]).Value, ((Constant)arr[1]).Value, ((Constant)arr[2]).Value);
				}
				else {
					color = new ColorF(((Constant)arr[0]).Value, ((Constant)arr[1]).Value, ((Constant)arr[2]).Value, ((Constant)arr[3]).Value);
				}
				return true;
			}
			else {
				logger.LogMessage(Message.UnknownValue, value.AstPosition);
				color = ColorF.Black;
				return false;
			}
		}

		public enum Message {

			[Message(MessageType.Warning, "Color as constant have to be between 0 and 0xFFFFFFFF (2^32 - 1).")]
			ColorConnstantOutOfRrange,
			[Message(MessageType.Warning, "Failed to parse a color from array. Expected constant at index {0}.")]
			ExpectedConstAtIndexI,
			[Message(MessageType.Warning, "Color as array must have length of 3 (RGB) or 4 (ARGB).")]
			ExpectedColorAsArrayLen34,
			[Message(MessageType.Warning, "Failed to parse color. Unknown value type.")]
			UnknownValue,

		}

	}
}
