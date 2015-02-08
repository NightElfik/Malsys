using System;
using System.Collections.Generic;
using Malsys.Evaluators;
using Malsys.Processing.Components.Renderers;
using Malsys.SemanticModel;

namespace Malsys.Processing.Components.Interpreters {
	/// <summary>
	/// Hexagonal ASCII interpreter interprets symbols as lines on hexagonal grid rendering them as text (ASCII art).
	/// </summary>
	/// <name>Hexagonal ASCII interpreter</name>
	/// <group>Interpreters</group>
	public class HexaAsciiInterpreter : IInterpreter {

		// pre-draw delta x, y; post-draw delta x, y
		private static readonly int[,] angleToXyDeltaHexa = {
			{ 0, 0, 1, 0 },  // _ (left)
			{ 0, 0, 1, 1 }, // / (up)
			{ -1, 0, 0, 1 },  // \ (up)
			{ -1, 0, 0, 0 },  // _ (right)
			{ -1, -1, 0, 0 },  // / (down)
			{ 0, -1, 1, 0 } };  // \ (down)

		private static readonly char[] angleToCharHexa = { '_', '/', '\\', '_', '/', '\\' };

		private static readonly int[,] angleToXyDeltaRightAngleSlash = {
			{ 0, 0, 1, 1 }, // / (up)
			{ -1, 0, 0, 1 },  // \ (up)
			{ -1, -1, 0, 0 },  // / (down)
			{ 0, -1, 1, 0 } };  // \ (down)

		private static readonly char[] angleToCharRightAngleSlash = { '/', '\\', '/', '\\' };

		private int angleStates;
		private int[,] angleToXyDelta;
		private char[] angleToChar;


		private ITextRenderer renderer;

		private Stack<HexaState> statesStack = new Stack<HexaState>();
		private HexaState currState;

		private int scale;
		private float horizontalScaleMult;
		private bool rightAngleSlashMode;


		#region User gettable and settable properties

		/// <summary>
		/// Scale of result ASCII art.
		/// Value representing number of characters to draw per line.
		/// </summary>
		/// <expected>Positive number.</expected>
		/// <default>1</default>
		[AccessName("scale")]
		[UserSettable]
		public Constant Scale { get; set; }

		/// <summary>
		/// Horizontal scale multiplier is used to multiply number of characters per horizontal line.
		/// Default value is 2 because ordinary characters are 2 times taller than wider.
		/// </summary>
		/// <expected>Positive number.</expected>
		/// <default>2</default>
		[AccessName("horizontalScaleMultiplier")]
		[UserSettable]
		public Constant HorizontalScaleMultiplier { get; set; }


		/// <summary>
		/// If set to true, renderer will draw right angles using slashes only.
		/// </summary>
		/// <expected>true or false</expected>
		/// <default>false</default>
		[AccessName("rightAngleSlashMode")]
		[UserSettable]
		public Constant RightAngleSlashMode { get; set; }


		#endregion User gettable and settable properties

		/// <summary>
		/// Render for rendering of ASCII art.
		/// Connected renderer must implement ITextRenderer interface.
		/// </summary>
		[UserConnectable]
		public IRenderer Renderer {
			set {
				if (!typeof(ITextRenderer).IsAssignableFrom(value.GetType())) {
					throw new InvalidConnectedComponentException("Renderer do not implement `{0}`.".Fmt(typeof(ITextRenderer).FullName));
				}
				renderer = (ITextRenderer)value;
			}
		}


		public IMessageLogger Logger { get; set; }


		public void Reset() {
			Scale = Constant.One;
			HorizontalScaleMultiplier = Constant.Two;
			RightAngleSlashMode = Constant.False;
		}

		public void Initialize(ProcessContext ctxt) {
			scale = Scale.RoundedIntValue;
			horizontalScaleMult = (float)HorizontalScaleMultiplier.Value;
			renderer.AddGlobalOutputData(OutputMetadataKeyHelper.OutputIsAsciiArt, true);
			rightAngleSlashMode = RightAngleSlashMode.IsTrue;

			if (rightAngleSlashMode) {
				angleStates = angleToCharRightAngleSlash.Length;
				angleToXyDelta = angleToXyDeltaRightAngleSlash;
				angleToChar = angleToCharRightAngleSlash;
			}
			else {
				angleStates = angleToCharHexa.Length;
				angleToXyDelta = angleToXyDeltaHexa;
				angleToChar = angleToCharHexa;
			}
		}

		public void Cleanup() { }

		public void Dispose() { }



		public bool RequiresMeasure { get { return false; } }


		public void BeginProcessing(bool measuring) {
			statesStack.Clear();
			currState = new HexaState();
			renderer.BeginProcessing(measuring);
		}

		public void EndProcessing() {
			renderer.EndProcessing();
		}



		#region Symbols interpretation methods

		/// <summary>
		/// Symbol is ignored.
		/// </summary>
		[SymbolInterpretation]
		public void Nothing(ArgsStorage args) { }

		/// <summary>
		/// Moves forward (without drawing) by one tile in current direction.
		/// </summary>
		[SymbolInterpretation]
		public void MoveForward(ArgsStorage args) {
			currState.X += angleToXyDelta[currState.Angle, 0];
			currState.Y += angleToXyDelta[currState.Angle, 1];
		}

		/// <summary>
		/// Draws line (from characters) in current direction.
		/// </summary>
		[SymbolInterpretation]
		public void DrawLine(ArgsStorage args) {
			int repeat = scale;

			if (!rightAngleSlashMode && currState.Angle % 3 == 0) {
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
			currState.Angle = (currState.Angle + angleStates / 2) % angleStates;
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

		#endregion Symbols interpretation methods



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
