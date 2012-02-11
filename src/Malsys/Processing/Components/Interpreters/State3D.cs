using System.Windows.Media.Media3D;
using Malsys.Media;

namespace Malsys.Processing.Components.Interpreters {
	public class State3D {

		public Point3D Position;

		public Quaternion Orientation;

		public double Width;

		public ColorF Color;


		public State3D Clone() {
			return (State3D)MemberwiseClone();
		}

	}
}
