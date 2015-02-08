using System.Windows.Media.Media3D;
using Malsys.Media;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;

namespace Malsys.Processing.Components.Interpreters {
	public class State3D {

		public Point3D Position;

		public Quaternion Rotation;

		public FSharpMap<string, IValue> Variables;


		public State3D Clone() {
			return (State3D)MemberwiseClone();
		}

	}
}
