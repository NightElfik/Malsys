using System;
using System.Linq;
using Malsys.Expressions;
using Malsys.Processing.Components.Renderers;
using Malsys.Media;

namespace Malsys.Processing.Components.Interpreters.TwoD {
	public class Interpreter2D : IInterpreter {

		private IRenderer2D renderer;

		private State2D currState;



		#region IInterpreter Members

		public IRenderer Renderer {
			set { renderer = (IRenderer2D)value; }
		}

		public bool IsRendererCompatible(IRenderer renderer) {
			return renderer.GetType().GetInterfaces().Contains(typeof(IRenderer2D));
		}

		public void BeginInterpreting() {
			renderer.BeginRendering();
		}

		public void EndInterpreting() {
			renderer.EndRendering();
		}

		#endregion

		#region Symbols interpretation methods


		[SymbolInterpretation]
		public void Nothing(ImmutableList<IValue> prms) {

		}

		[SymbolInterpretation]
		public void MoveForward(ImmutableList<IValue> prms) {

			double param = getParamAsDouble(prms, 0);

			currState.X += param * Math.Cos(currState.CurrentAngle * MathHelper.PiOver180);
			currState.Y += param * Math.Sin(currState.CurrentAngle * MathHelper.PiOver180);

			renderer.MoveTo(new PointF((float)currState.X, (float)currState.Y), ColorF.Black, 1);
		}

		[SymbolInterpretation]
		public void DrawLine(ImmutableList<IValue> prms) {

			double param = getParamAsDouble(prms, 0);

			currState.X += param * Math.Cos(currState.CurrentAngle * MathHelper.PiOver180);
			currState.Y += param * Math.Sin(currState.CurrentAngle * MathHelper.PiOver180);

			renderer.LineTo(new PointF((float)currState.X, (float)currState.Y), ColorF.Black, 1);
		}

		[SymbolInterpretation]
		public void TurnLeft(ImmutableList<IValue> prms) {

			double param = getParamAsDouble(prms, 0);

			currState.CurrentAngle += param;
		}

		[SymbolInterpretation]
		public void TurnRight(ImmutableList<IValue> prms) {

			double param = getParamAsDouble(prms, 0);

			currState.CurrentAngle -= param;
		}



		#endregion

		private double getParamAsDouble(ImmutableList<IValue> prms, int i) {
			if (prms.Length < i && prms[i].IsConstant) {
				return ((Constant)prms[i]).Value;
			}
			else {
				return 0.0;
			}
		}

	}
}
