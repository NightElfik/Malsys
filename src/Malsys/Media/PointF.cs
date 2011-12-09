﻿
namespace Malsys.Media {
	public struct PointF {

		public float X;
		public float Y;


		public PointF(float x, float y) {
			X = x;
			Y = y;
		}

		public override string ToString() {
			return "{" + X + "," + Y + "}";
		}
	}
}
