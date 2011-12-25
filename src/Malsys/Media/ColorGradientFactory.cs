using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.SemanticModel.Evaluated;
using Malsys.SemanticModel;

namespace Malsys.Media {

	public class ColorGradientFactory {

		public ColorGradient CreateFromValuesArray(ValuesArray array, IMessageLogger logger) {

			var colors = new List<ColorF>();
			var dists = new List<uint>();

			bool nextColor = true;

			foreach (var item in array) {
				if (nextColor) {
					colors.Add(parseColor(item, logger));
					nextColor = false;
				}
				else {
					if (item.IsConstant) {
						var c = (Constant)item;
						if (c.Value < 0) {
							logger.LogMessage(Message.DistanceCantBeNegative, c.AstPosition);
							c = Constant.Zero;
						}
						dists.Add((uint)Math.Round(c.Value));
						nextColor = true;
					}
					else {
						dists.Add(0);
						colors.Add(parseColor(item, logger));
						// still nextColor = false;
					}
				}
			}

			return new ColorGradient(colors.ToArray(), dists.ToArray());
		}


		private ColorF parseColor(IValue value, IMessageLogger logger) {

			if (!value.IsArray) {
				logger.LogMessage(Message.ExpectedColorAsArray, value.AstPosition);
				return ColorF.Black;
			}

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

			if (arr.Length == 3) {
				return new ColorF(((Constant)arr[0]).Value, ((Constant)arr[1]).Value, ((Constant)arr[2]).Value);
			}
			else {
				return new ColorF(((Constant)arr[0]).Value, ((Constant)arr[1]).Value, ((Constant)arr[2]).Value, ((Constant)arr[3]).Value);
			}
		}

		public enum Message {

			[Message(MessageType.Warning, "Expected color as array of length 3 (RGB) or 4 (ARGB).")]
			DistanceCantBeNegative,
			[Message(MessageType.Warning, "Expected color as array of length 3 (RGB) or 4 (ARGB).")]
			ExpectedColorAsArray,
			[Message(MessageType.Warning, "Failed to parse a color. Expected constant at index {0}.")]
			ExpectedConstAtIndexI,
			[Message(MessageType.Warning, "Failed to parse a color. Expected array of length 3 (RGB) or 4 (ARGB).")]
			ExpectedColorAsArrayLen34,

		}

	}
}
