using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Media {
	public class ColorFactory {

		public bool IsColor(IValue value) {

			if (value.IsConstant) {
				var c = ((Constant)value);
				if (c.IsNaN) {
					return false;
				}
				long val = c.RoundedLongValue;
				if (val < 0 || val > 0xFFFFFF) {
					return false;
				}
			}
			else if (value.IsArray) {
				var arr = (ValuesArray)value;

				if (arr.Length != 3 && arr.Length != 4) {
					return false;
				}

				if (!arr.IsConstArray()) {
					return false;
				}
			}

			return true;
		}

		public bool TryParseColor(IValue value, out ColorF color) {

			if (IsColor(value)) {
				color = toColor(value);
				return true;
			}
			else {
				color = ColorF.Black;
				return false;
			}
		}

		public void ParseColor(IValue value, ref ColorF color) {

			if (IsColor(value)) {
				color = toColor(value);
			}

		}

		public ColorF FromIValue(IValue value, IMessageLogger logger) {

			if (value.IsConstant) {
				long val = ((Constant)value).RoundedLongValue;
				if (val < 0 || val > 0xFFFFFF) {
					logger.LogMessage(Message.ColorConnstantOutOfRrange, value.AstPosition);
				}
			}
			else if (value.IsArray) {

				var arr = (ValuesArray)value;

				if (arr.Length != 3 && arr.Length != 4) {
					logger.LogMessage(Message.ExpectedColorAsArrayLen34, arr.AstPosition);
					return ColorF.Black;
				}

				for (int i = 0; i < arr.Length; i++) {
					if (!arr[i].IsConstant) {
						logger.LogMessage(Message.ExpectedConstAtIndexI, value.AstPosition, i);
						return ColorF.Black;
					}
				}
			}
			else {
				logger.LogMessage(Message.UnknownValue, value.AstPosition);
				return ColorF.Black;
			}

			return toColor(value);
		}


		private ColorF toColor(IValue value) {

			if (value.IsConstant) {
				return new ColorF((uint)((Constant)value).RoundedLongValue);
			}
			else {
				ValuesArray arr = (ValuesArray)value;
				if (arr.Length == 3) {
					return new ColorF(((Constant)arr[0]).Value, ((Constant)arr[1]).Value, ((Constant)arr[2]).Value);
				}
				else {
					return new ColorF(((Constant)arr[0]).Value, ((Constant)arr[1]).Value, ((Constant)arr[2]).Value, ((Constant)arr[3]).Value);
				}
			}
		}


		public enum Message {

			[Message(MessageType.Warning, "Color as constant have to be between #0 and #FFFFFF.")]
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
