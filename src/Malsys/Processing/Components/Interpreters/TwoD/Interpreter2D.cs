using System;
using System.Linq;
using Malsys.Evaluators;
using Malsys.Media;
using Malsys.Processing.Components.Renderers;
using Malsys.SemanticModel;

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

		#endregion

		#region IComponent Members

		public ProcessContext Context { get; set; }

		public void BeginProcessing(bool measuring) {
			currState = new State2D();
			renderer.BeginProcessing(measuring);
		}

		public void EndProcessing() {
			renderer.EndProcessing();
		}

		#endregion

		#region Symbols interpretation methods

		[SymbolInterpretation]
		public void Nothing(ArgsStorage args) {

		}

		[SymbolInterpretation]
		public void MoveForward(ArgsStorage args) {

			double param = getParamAsDouble(args, 0);

			currState.X += param * Math.Cos(currState.CurrentAngle * MathHelper.PiOver180);
			currState.Y += param * Math.Sin(currState.CurrentAngle * MathHelper.PiOver180);

			renderer.MoveTo(new PointF((float)currState.X, (float)currState.Y), ColorF.Black, 1);
		}

		[SymbolInterpretation]
		public void DrawLine(ArgsStorage args) {

			double param = getParamAsDouble(args, 0);

			currState.X += param * Math.Cos(currState.CurrentAngle * MathHelper.PiOver180);
			currState.Y += param * Math.Sin(currState.CurrentAngle * MathHelper.PiOver180);

			renderer.LineTo(new PointF((float)currState.X, (float)currState.Y), ColorF.Black, 1);
		}

		[SymbolInterpretation]
		public void TurnLeft(ArgsStorage args) {

			double param = getParamAsDouble(args, 0);

			currState.CurrentAngle += param;
		}

		[SymbolInterpretation]
		public void TurnRight(ArgsStorage args) {

			double param = getParamAsDouble(args, 0);

			currState.CurrentAngle -= param;
		}

		#endregion


		private double getParamAsDouble(ArgsStorage args, int i) {
			if (args.ArgsCount < i && args[i].IsConstant) {
				return ((Constant)args[i]).Value;
			}
			else {
				return 0.0;
			}
		}


	}
}
