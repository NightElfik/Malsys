using System;

namespace Malsys.Interpreters {
	class EmptyInterpret : IInterpreter {

		public static readonly EmptyInterpret Instance = new EmptyInterpret();


		#region IInterpreter Members

		public Renderers.IRenderer Renderer {
			set { throw new InvalidOperationException("Empty interpreter."); }
		}

		public bool IsRendererCompatible(Renderers.IRenderer renderer) {
			throw new InvalidOperationException("Empty interpreter.");
		}

		public void BeginInterpreting() {
			throw new InvalidOperationException("Empty interpreter.");
		}

		public void EndInterpreting() {
			throw new InvalidOperationException("Empty interpreter.");
		}

		#endregion
	}
}
