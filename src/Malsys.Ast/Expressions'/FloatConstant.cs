/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class FloatConstant : IExpressionMember {

		public readonly double Value;
		public readonly ConstantFormat Format;


		public FloatConstant(double value, ConstantFormat cf, Position pos) {
			Value = value;
			Format = cf;
			Position = pos;
		}


		public override string ToString() {

			switch (Format) {
				case ConstantFormat.Binary:
					return "0b" + Convert.ToString((long)Math.Round(Value), 2);
				case ConstantFormat.Octal:
					return "0o" + Convert.ToString((long)Math.Round(Value), 8);
				case ConstantFormat.Hexadecimal:
					return "0x" + Convert.ToString((long)Math.Round(Value), 16).ToUpperInvariant();
				case ConstantFormat.HashHexadecimal:
					return "#" + Convert.ToString((long)Math.Round(Value), 16).ToUpperInvariant();
				default:
					return Value.ToStringInvariant();
			}
		}



		public Position Position { get; private set; }


		public ExpressionMemberType MemberType {
			get { return ExpressionMemberType.FloatConstant; }
		}

	}
}
