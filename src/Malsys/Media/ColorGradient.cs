using System;

namespace Malsys.Media {
	public class ColorGradient {

		private ColorF[] colors;
		private uint[] distances;
		private uint[] prefixSumDist;
		private uint length;


		public ColorGradient(ColorF[] colors, uint[] distances) {

			if (colors.Length != distances.Length + 1) {
				throw new ArgumentException("Number of colors must be one more than number of distances (between each color is distance).");
			}

			this.colors = colors;
			this.distances = distances;

			prefixSumDist = new uint[distances.Length + 1];
			prefixSumDist[0] = 0;
			for (int i = 0; i < distances.Length; i++) {
				prefixSumDist[i + 1] = distances[i] + prefixSumDist[i] + 1;
			}
			length = prefixSumDist[prefixSumDist.Length - 1] + 1;  // +1 for first color
		}

		public uint Length { get { return length; } }

		public ColorF this[uint distance] { get { return GetColor(distance); } }
		public ColorF this[float percent] { get { return GetColor((uint)(percent * (length - 1))); } }


		public ColorF GetColor(uint distance) {

			distance = distance % length;
			if (length == 1 || distance == 0) {
				return colors[0];
			}

			int upperBoundIndex = Array.BinarySearch(prefixSumDist, distance);
			if (upperBoundIndex >= 0) {
				return colors[upperBoundIndex];
			}
			else {
				upperBoundIndex = ~upperBoundIndex;
			}

			int lowerBoundIndex = upperBoundIndex - 1;

			float lowerMultiplier = (float)(prefixSumDist[upperBoundIndex] - distance) / (distances[lowerBoundIndex] + 1);
			float upperMultiplier = (float)(distance - prefixSumDist[lowerBoundIndex]) / (distances[lowerBoundIndex] + 1);

			ColorF lowerColor = colors[lowerBoundIndex];
			ColorF upperColor = colors[upperBoundIndex];

			return new ColorF(
				lowerColor.A * lowerMultiplier + upperColor.A * upperMultiplier,
				lowerColor.R * lowerMultiplier + upperColor.R * upperMultiplier,
				lowerColor.G * lowerMultiplier + upperColor.G * upperMultiplier,
				lowerColor.B * lowerMultiplier + upperColor.B * upperMultiplier);
		}

	}
}
