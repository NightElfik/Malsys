using System;
using Malsys.Media;

namespace Malsys.Processing.Components.Interpreters {
	public class State2D {

		public double X;
		public double Y;

		public double CurrentAngle;
		public double LineWidth;

		public ColorF Color;


		public State2D Clone() {
			return (State2D)MemberwiseClone();
		}

	}
}
