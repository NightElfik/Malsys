using System;
using System.Collections.Generic;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Media {

	public class ColorGradientFactory {

		private ColorParser clrParser = new ColorParser();

		public ColorGradient CreateFromValuesArray(ValuesArray array, IMessageLogger logger) {

			var colors = new List<ColorF>();
			var dists = new List<uint>();

			bool nextColor = true;

			foreach (var item in array) {
				if (nextColor) {
					colors.Add(clrParser.ParseColor(item, logger));
					nextColor = false;
				}
				else {
					if (item.IsConstant && ((Constant)item).ConstantFormat != Ast.ConstantFormat.HashHexadecimal) {
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
						colors.Add(clrParser.ParseColor(item, logger));
						// still nextColor = false;
					}
				}
			}

			return new ColorGradient(colors.ToArray(), dists.ToArray());
		}

		public ValuesArray ToValuesArray(ColorGradient gradient) {

			var arr = new IValue[gradient.Length];
			for (int i = 0; i < gradient.Length; i++) {
				arr[i] = new Constant(gradient[i].ToArgb());
			}
			return new ValuesArray(new ImmutableList<IValue>(arr, true));
		}


		public enum Message {

			[Message(MessageType.Warning, "Distance between colors can not be negative, using zero distance.")]
			DistanceCantBeNegative,

		}

	}
}
