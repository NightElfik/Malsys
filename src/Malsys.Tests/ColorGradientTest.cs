using System;
using Malsys.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UintColorF = System.Tuple<uint, Malsys.Media.ColorF>;

namespace Malsys.Tests {
	[TestClass]
	public class ColorGradientTest {

		[TestMethod]
		public void ZeroDistances() {

			doTest(
				new ColorF[] { ColorF.Black },
				new uint[] { },
				new UintColorF(0, ColorF.Black),
				new UintColorF(1, ColorF.Black),
				new UintColorF(2, ColorF.Black));

			doTest(
				new ColorF[] { ColorF.Black, ColorF.White },
				new uint[] { 0 },
				new UintColorF(0, ColorF.Black),
				new UintColorF(1, ColorF.White),
				new UintColorF(2, ColorF.Black),
				new UintColorF(3, ColorF.White),
				new UintColorF(4, ColorF.Black));

			doTest(
				new ColorF[] { ColorF.Red, ColorF.Green, ColorF.Blue },
				new uint[] { 0, 0 },
				new UintColorF(0, ColorF.Red),
				new UintColorF(1, ColorF.Green),
				new UintColorF(2, ColorF.Blue),
				new UintColorF(3, ColorF.Red),
				new UintColorF(4, ColorF.Green));

		}

		[TestMethod]
		public void TwoColorsGradient() {
			doTest(
				new ColorF[] { ColorF.Black, ColorF.White },
				new uint[] { 7 },
				new UintColorF(0, ColorF.Black),
				new UintColorF(1, new ColorF(0.125f, 0.125f, 0.125f)),
				new UintColorF(2, new ColorF(0.25f, 0.25f, 0.25f)),
				new UintColorF(3, new ColorF(0.375f, 0.375f, 0.375f)),
				new UintColorF(4, new ColorF(0.5f, 0.5f, 0.5f)),
				new UintColorF(5, new ColorF(0.625f, 0.625f, 0.625f)),
				new UintColorF(6, new ColorF(0.75f, 0.75f, 0.75f)),
				new UintColorF(7, new ColorF(0.875f, 0.875f, 0.875f)),
				new UintColorF(8, ColorF.White));
		}

		[TestMethod]
		public void ThreeColorsGradient() {
			doTest(
				new ColorF[] { ColorF.TransparentBlack, ColorF.White, ColorF.TransparentBlack },
				new uint[] { 3, 3 },
				new UintColorF(0, ColorF.TransparentBlack),
				new UintColorF(1, new ColorF(0.75f, 0.25f, 0.25f, 0.25f)),
				new UintColorF(2, new ColorF(0.5f, 0.5f, 0.5f, 0.5f)),
				new UintColorF(4, ColorF.White),
				new UintColorF(5, new ColorF(0.25f, 0.75f, 0.75f, 0.75f)),
				new UintColorF(6, new ColorF(0.5f, 0.5f, 0.5f, 0.5f)),
				new UintColorF(8, ColorF.TransparentBlack));
		}


		private void doTest(ColorF[] colors, uint[] distances, params Tuple<uint, ColorF>[] expectedColors) {

			var target = new ColorGradient(colors, distances);

			foreach (var distColor in expectedColors) {
				var clr = target.GetColor(distColor.Item1);
				Assert.AreEqual(distColor.Item2, clr);
			}
		}
	}
}
