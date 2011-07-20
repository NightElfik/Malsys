﻿using System;

namespace Malsys.Expressions {
	static class FloatArithmeticHelper {
		public static readonly double Epsilon = 1e-8;

		public static bool IsZero(double value) {
			return Math.Abs(value) < Epsilon;
		}
	}
}
