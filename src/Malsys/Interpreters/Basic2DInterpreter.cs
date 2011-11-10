using System;
using Malsys.Expressions;
using Malsys.Renderers;

namespace Malsys.Interpreters {
	public class Basic2DInterpreter : IInterpreter {

		IBasic2DRenderer renderer;


		public Basic2DInterpreter(IBasic2DRenderer rend) {
			renderer = rend;
		}



		#region IInterpreter Members

		public IRenderer Renderer {
			set { throw new NotImplementedException(); }
		}

		public bool IsRendererCompatible(IRenderer renderer) {
			throw new NotImplementedException();
		}

		public void BeginInterpreting() {
			throw new NotImplementedException();
		}

		public void EndInterpreting() {
			throw new NotImplementedException();
		}

		#endregion
	}
}
