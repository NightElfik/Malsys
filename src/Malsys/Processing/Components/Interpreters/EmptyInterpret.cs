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

		#endregion

		#region IComponent Members

		public bool RequiresMeasure {
			get { throw new InvalidOperationException("Empty interpreter."); }
		}

		public void Initialize(ProcessContext context) {
			throw new InvalidOperationException("Empty interpreter.");
		}

		public void Cleanup() {
			throw new InvalidOperationException("Empty interpreter.");
		}

		public void BeginProcessing(bool measuring) {
			throw new InvalidOperationException("Empty interpreter.");
		}

		public void EndProcessing() {
			throw new InvalidOperationException("Empty interpreter.");
		}

		#endregion
	}
}
