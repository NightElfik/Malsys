using System;

namespace Malsys.Processing.Components.Interpreters {
	class EmptyInterpret : IInterpreter {

		public static readonly EmptyInterpret Instance = new EmptyInterpret();


		#region IInterpreter Members

		public IRenderer Renderer {
			set { throw new InvalidOperationException("Empty interpreter."); }
		}

		public bool IsRendererCompatible(IRenderer renderer) {
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
