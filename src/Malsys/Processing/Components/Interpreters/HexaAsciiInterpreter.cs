using System;
using Malsys.Evaluators;
using System.Collections.Generic;
using Malsys.Processing.Components.Renderers;
using Malsys.SemanticModel;

namespace Malsys.Processing.Components.Interpreters {
	[Component("Hexagonal ASCII interpreter", ComponentGroupNames.Interpreters)]
	public class HexaAsciiInterpreter : IInterpreter {

		const int angleStates = 6;

		// pre-draw delta x, y; post-draw delta x, y
		private static readonly int[,] angleToXyDelta = {
			{ 0, 0, 1, 0 },  // _ (left)
			{ 0, 0, 1, 1 }, // / (up)
			{ -1, 0, 0, 1 },  // \ (up)
			{ -1, 0, 0, 0 },  // _ (right)
			{ -1, -1, 0, 0 },  // / (down)
			{ 0, -1, 1, 0 } };  // \ (down)

		private static readonly char[] angleToChar = { '_', '/', '\\', '_', '/', '\\' };

		private ITextRenderer renderer;
		private IMessageLogger logger;

		private Stack<HexaState> statesStack = new Stack<HexaState>();
		private HexaState currState;

		private int scale = 1;
		private float horizontalScaleMult = 2;


		[UserSettable]
		public Constant Scale {
			set { scale = value.RoundedIntValue; }
		}

		[UserSettable]
		public Constant HorizontalScaleMultiplier {
			set { horizontalScaleMult = (float)value.Value; }
		}

		#region IInterpreter Members

		[UserConnectable]
		public IRenderer Renderer {
			set {
				if (!IsRendererCompatible(value)) {
					throw new InvalidUserValueException("Renderer do not implement `{0}`.".Fmt(typeof(ITextRenderer).FullName));
				}
				renderer = (ITextRenderer)value;
			}
		}

		public bool IsRendererCompatible(IRenderer renderer) {
			return typeof(ITextRenderer).IsAssignableFrom(renderer.GetType());
		}

		public bool RequiresMeasure { get { return false; } }

		public void Initialize(ProcessContext ctxt) {
			logger = ctxt.Logger;
			renderer.AddGlobalOutputData(CommonAdditionalDataKeys.OutputIsAsciiArt, true);
		}

		public void Cleanup() { }

		public void BeginProcessing(bool measuring) {
			statesStack.Clear();
			currState = new HexaState();
			renderer.BeginProcessing(measuring);
		}

		public void EndProcessing() {
			renderer.EndProcessing();
		}

		#endregion


		#region Symbols interpretation methods

		/// <summary>
		/// Symbol is ignored.
		/// </summary>
		[SymbolInterpretation]
		public void Nothing(ArgsStorage args) {

		}

		/// <summary>
		/// Moves forward (without drawing) by one tile in current direction.
		/// </summary>
		[SymbolInterpretation]
		public void MoveForward(ArgsStorage args) {
			currState.X += angleToXyDelta[currState.Angle, 0];
			currState.Y += angleToXyDelta[currState.Angle, 1];
		}

		/// <summary>
		/// Draws line in current direction.
		/// </summary>
		[SymbolInterpretation]
		public void DrawLine(ArgsStorage args) {

			int repeat = scale;

			if (currState.Angle % 3 == 0) {
				repeat = (int)Math.Round(repeat * horizontalScaleMult);
			}

			while (repeat-- > 0) {
				currState.X += angleToXyDelta[currState.Angle, 0];
				currState.Y += angleToXyDelta[currState.Angle, 1];
				renderer.PutCharAt(angleToChar[currState.Angle], currState.X, currState.Y);
				currState.X += angleToXyDelta[currState.Angle, 2];
				currState.Y += angleToXyDelta[currState.Angle, 3];
			}
		}

		/// <summary>
		/// Turns left by 60 degrees.
		/// </summary>
		[SymbolInterpretation]
		public void TurnLeft(ArgsStorage args) {
			currState.Angle = (currState.Angle + 1) % angleStates;
		}

		/// <summary>
		/// Turns right by 60 degrees.
		/// </summary>
		[SymbolInterpretation]
		public void TurnRight(ArgsStorage args) {
			currState.Angle = (currState.Angle - 1 + angleStates) % angleStates;
		}

		/// <summary>
		/// Turns by 180 degrees.
		/// </summary>
		[SymbolInterpretation]
		public void TurnAround(ArgsStorage args) {
			currState.Angle = (currState.Angle + 3) % angleStates;
		}

		/// <summary>
		/// Saves current state (on stack).
		/// </summary>
		[SymbolInterpretation]
		public void StartBranch(ArgsStorage args) {
			statesStack.Push(currState.Clone());
		}

		/// <summary>
		/// Loads previously saved state (returns to last saved position).
		/// </summary>
		[SymbolInterpretation]
		public void EndBranch(ArgsStorage args) {
			if (statesStack.Count > 0) {
				currState = statesStack.Pop();
			}
			else {
				throw new InterpretationException("Failed to complete branch. No branch is opened.");
			}
		}

		#endregion



		private class HexaState {

			public int X;
			public int Y;
			public int Angle;

			public HexaState Clone() {
				return (HexaState)MemberwiseClone();
			}


		}

	}
}
