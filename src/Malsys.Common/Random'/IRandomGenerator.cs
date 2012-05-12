/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys {
	public interface IRandomGenerator {

		/// <summary>
		/// Fills the elements of a specified array of bytes with random numbers.
		/// </summary>
		void NextBytes(byte[] buffer);

		/// <summary>
		/// Returns a double-precision floating point number greater than or equal to 0.0, and less than 1.0.
		/// </summary>
		double NextDouble();

	}

	public static class IRandomGeneratorExtensions {

		/// <summary>
		/// Returns a nonnegative random number.
		/// </summary>
		public static int Next(this IRandomGenerator randomProvider) {
			return (int)(randomProvider.NextDouble() * int.MaxValue);
		}

		/// <summary>
		/// Returns a nonnegative random number less than the specified maximum.
		/// </summary>
		public static int Next(this IRandomGenerator randomProvider, int maxValue) {
			return (int)(randomProvider.NextDouble() * maxValue);
		}

		/// <summary>
		/// Returns a random number within a specified range.
		/// </summary>
		/// <param name="randomProvider">The random provider instance.</param>
		/// <param name="minValue">The inclusive lower bound of the random number returned.</param>
		/// <param name="maxValue">The exclusive upper bound of the random number returned.</param>
		public static int Next(this IRandomGenerator randomProvider, int minValue, int maxValue) {
			return minValue + (int)(randomProvider.NextDouble() * (maxValue - minValue));
		}

	}
}
