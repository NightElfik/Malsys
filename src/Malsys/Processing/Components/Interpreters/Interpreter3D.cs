using System.Collections.Generic;
using System.Windows.Media.Media3D;
using Malsys.Evaluators;
using Malsys.Media;
using Malsys.Processing.Components.Renderers;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components.Interpreters {
	[Component("3D interpreter", ComponentGroupNames.Interpreters)]
	public class Interpreter3D : IInterpreter3D {


		private IRenderer3D renderer;
		private IMessageLogger logger;

		private State3D currState;

		private QuaternionRotation3D quatRotation;
		private RotateTransform3D tranform;

		private bool measuring;

		private uint colorEvents;
		private uint colorEventsMeasured;

		private bool continousColoring = false;
		private bool colorContinously = false;
		private ColorGradient colorGradient;

		private bool interpretLines = true;
		private bool interpretPloygons = true;

		private Vector3D fwdVector = new Vector3D(1, 0, 0);
		private Vector3D upVector = new Vector3D(0, 1, 0);
		private Vector3D rightVector;  // counted from forward and up vectors by cross product

		private Point3D initPosition = new Point3D(0, 0, 0);
		private Quaternion initRotation = Quaternion.Identity;
		private double initWidth = 2;
		private ColorF initColor = ColorF.Black;

		private Stack<State3D> statesStack = new Stack<State3D>();

		private Polygon3D currPolygon;
		private Stack<Polygon3D> polygonStack = new Stack<Polygon3D>();


		public Interpreter3D() {

			quatRotation = new QuaternionRotation3D();
			tranform = new RotateTransform3D(quatRotation, 0, 0, 0);
		}


		#region User settable variables

		[Alias("interpretLines")]
		[UserSettable]
		public Constant InterpretLines {
			set { interpretLines = !value.IsZero; }
		}

		[Alias("interpretPolygons")]
		[UserSettable]
		public Constant InterpretPolygons {
			set { interpretPloygons = !value.IsZero; }
		}

		[Alias("origin")]
		[UserSettable]
		public ValuesArray Origin {
			set {
				if (!value.IsConstArrayOfLength(3)) {
					throw new InvalidUserValueException("Origin have to be array of 3 constants representing x, y and z coordinate.");
				}

				initPosition.X = ((Constant)value[0]).Value;
				initPosition.Y = ((Constant)value[1]).Value;
				initPosition.Z = ((Constant)value[2]).Value;
			}
		}

		[Alias("rotationQuaternion")]
		[UserSettable]
		public ValuesArray RotationQuaternion {
			set {
				if (!value.IsConstArrayOfLength(4)) {
					throw new InvalidUserValueException("Rotation quaternion have to be array of 4 constants representing direction quaternion.");
				}

				initRotation.W = ((Constant)value[0]).Value;
				initRotation.X = ((Constant)value[1]).Value;
				initRotation.Y = ((Constant)value[2]).Value;
				initRotation.Z = ((Constant)value[3]).Value;
			}
		}

		[Alias("initialLineWidth")]
		[UserSettable]
		public Constant InitialLineWidth {
			set { initWidth = value.Value; }
		}

		[Alias("initialColor")]
		[UserSettable]
		public ValuesArray InitialColor { get; set; }


		[Alias("continousColoring")]
		[UserSettable]
		public IValue ContinousColoring { get; set; }

		#endregion


		#region IInterpreter Members

		[UserConnectable]
		public IRenderer Renderer {
			set {
				if (!typeof(IRenderer3D).IsAssignableFrom(value.GetType())) {
					throw new InvalidConnectedComponentException("Renderer do not implement `{0}`.".Fmt(typeof(IRenderer3D).FullName));
				}
				renderer = (IRenderer3D)value;
			}
		}


		public bool RequiresMeasure { get { return continousColoring; } }

		public void Initialize(ProcessContext ctxt) {

			logger = ctxt.Logger;

			rightVector = Vector3D.CrossProduct(fwdVector, upVector);

			//var test = new Vector3D(0, 0, 0);
			//test.Normalize();

			fwdVector.Normalize();
			upVector.Normalize();
			rightVector.Normalize();

			if (ContinousColoring != null) {
				if (ContinousColoring.IsConstant) {
					if (!((Constant)ContinousColoring).IsZero) {
						continousColoring = true;
						colorGradient = ColorGradients.Rainbow;
					}
				}
				else {
					continousColoring = true;
					colorGradient = new ColorGradientFactory().CreateFromValuesArray((ValuesArray)ContinousColoring, logger);
				}
			}

			if (InitialColor != null) {
				initColor = ColorHelper.FromIValue(InitialColor, logger);
			}

		}

		public void Cleanup() { }

		public void BeginProcessing(bool measuring) {

			this.measuring = measuring;

			statesStack.Clear();
			currPolygon = null;
			polygonStack.Clear();

			if (measuring) {
				colorEventsMeasured = 0;
				colorContinously = false;
			}
			else {
				colorContinously = continousColoring;
			}

			colorEvents = 0;
			currState = new State3D() {
				Position = initPosition,
				Rotation = initRotation,
				Width = initWidth,
				Color = initColor
			};

			renderer.BeginProcessing(measuring);
			renderer.InitializeState(currState.Position, currState.Rotation, currState.Width, currState.Color);
		}

		public void EndProcessing() {

			if (measuring) {
				colorEventsMeasured = colorEvents;
			}

			if (statesStack.Count > 0) {
				logger.LogMessage(Message.BranchNotClosedAtEnd);
				statesStack.Clear();
			}

			if (polygonStack.Count > 0) {
				logger.LogMessage(Message.PolygonNotClosedAtEnd);
				currPolygon = null;
				polygonStack.Clear();
			}

			renderer.EndProcessing();
		}

		#endregion


		#region IInterpreter3D Members


		/// <summary>
		/// Symbol is ignored.
		/// </summary>
		[SymbolInterpretation]
		public void Nothing(ArgsStorage args) {

		}

		/// <summary>
		/// Moves pen forward (without drawing) by distance equal to value of the first parameter.
		/// </summary>
		[SymbolInterpretation(1)]
		public void MoveForward(ArgsStorage args) {

			double length = getArgumentAsDouble(args, 0);

			quatRotation.Quaternion = currState.Rotation;
			currState.Position += tranform.Transform(fwdVector * length);

			if (interpretLines) {
				ColorF color = colorContinously ? colorGradient[(float)colorEvents / colorEventsMeasured] : currState.Color;
				renderer.MoveTo(currState.Position, currState.Rotation, currState.Width, color);
			}
		}

		/// <summary>
		/// Draws line in current direction with length equal to value of first parameter.
		/// </summary>
		[SymbolInterpretation(1)]
		public void DrawForward(ArgsStorage args) {

			double length = getArgumentAsDouble(args, 0);

			quatRotation.Quaternion = currState.Rotation;
			currState.Position += tranform.Transform(fwdVector * length);

			if (interpretLines) {
				double width = getArgumentAsDouble(args, 1, currState.Width);

				ColorF color = currState.Color;

				if (colorContinously) {
					color = colorGradient[(float)colorEvents / colorEventsMeasured];
				}
				else if (args.ArgsCount >= 3) {
					ColorHelper.ParseColor(args[2], ref color);
				}

				colorEvents++;
				renderer.DrawTo(currState.Position, currState.Rotation, width, color);
			}
		}

		/// <summary>
		/// Draws sphere in current position with radius equal to value of first parameter.
		/// </summary>
		[SymbolInterpretation(1)]
		public void DrawSphere(ArgsStorage args) {

			double radius = getArgumentAsDouble(args, 0);

			ColorF color = currState.Color;

			if (colorContinously) {
				color = colorGradient[(float)colorEvents / colorEventsMeasured];
			}
			else if (args.ArgsCount >= 2) {
				ColorHelper.ParseColor(args[1], ref color);
			}

			colorEvents++;
			renderer.DrawSphere(currState.Position, radius, color);
		}

		/// <summary>
		/// Turns left around up vector axis by value given in the first parameter (in degrees).
		/// </summary>
		[SymbolInterpretation(1)]
		public void Yaw(ArgsStorage args) {

			double angle = getArgumentAsDouble(args, 0);

			currState.Rotation *= new Quaternion(upVector, angle);
		}

		/// <summary>
		/// Turns up around right vector axis by value given in the first parameter (in degrees).
		/// </summary>
		[SymbolInterpretation(1)]
		public void Pitch(ArgsStorage args) {

			double angle = getArgumentAsDouble(args, 0);

			currState.Rotation *= new Quaternion(rightVector, angle);
		}

		/// <summary>
		/// Turns left around forward vector axis by value given in the first parameter (in degrees).
		/// </summary>
		[SymbolInterpretation(1)]
		public void Roll(ArgsStorage args) {

			double angle = getArgumentAsDouble(args, 0);

			currState.Rotation *= new Quaternion(fwdVector, angle);
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
				renderer.MoveTo(currState.Position, currState.Rotation, currState.Width, currState.Color);
			}
			else {
				throw new InterpretationException("Failed to complete branch. No branch is opened.");
			}
		}

		/// <summary>
		/// Starts to record polygon vertices (do not saves current position as first vertex).
		/// If another polygon is opened, its state is saved and will be restored after closing of current polygon.
		/// </summary>
		[SymbolInterpretation]
		public void StartPolygon(ArgsStorage args) {

			if (!interpretPloygons) {
				return;
			}

			if (currPolygon != null) {
				polygonStack.Push(currPolygon);
			}

			ColorF color = currState.Color,
				strokeColor = currState.Color;
			float strokeWidth = (float)currState.Width;



			if (colorContinously) {
				color = strokeColor = colorGradient[(float)colorEvents / colorEventsMeasured];
			}
			else {
				if (args.ArgsCount >= 1) {
					ColorHelper.ParseColor(args[0], ref color);
				}
				if (args.ArgsCount >= 3) {
					ColorHelper.ParseColor(args[2], ref strokeColor);
				}
			}

			colorEvents++;

			if (args.ArgsCount >= 2 && args[1].IsConstant) {
				strokeWidth = (float)((Constant)args[1]).Value;
			}


			currPolygon = new Polygon3D(color, strokeWidth, strokeColor);

		}

		/// <summary>
		/// Records current position to opened polygon.
		/// </summary>
		[SymbolInterpretation]
		public void RecordPolygonVertex(ArgsStorage args) {

			if (!interpretPloygons) {
				return;
			}

			if (currPolygon == null) {
				throw new InterpretationException("Failed to record polygon vertex. No polygon is opened.");
			}

			currPolygon.Ponits.Add(currState.Position);
		}

		/// <summary>
		/// Ends current polygon (do not saves current position as last vertex).
		/// </summary>
		[SymbolInterpretation]
		public void EndPolygon(ArgsStorage args) {

			if (!interpretPloygons) {
				return;
			}

			if (currPolygon == null) {
				throw new InterpretationException("Failed to end polygon. No polygon is opened.");
			}

			renderer.DrawPolygon(currPolygon);

			currPolygon = polygonStack.Count > 0 ? polygonStack.Pop() : null;
		}


		#endregion


		private double getArgumentAsDouble(ArgsStorage args, int index, double defaultValue = 0d) {
			if (index < args.ArgsCount && args[index].IsConstant) {
				return ((Constant)args[index]).Value;
			}
			else {
				return defaultValue;
			}
		}


		public enum Message {

			[Message(MessageType.Warning, "Some branches were not closed while interpretation ended.")]
			BranchNotClosedAtEnd,
			[Message(MessageType.Warning, "Some polygons were not closed while interpretation ended.")]
			PolygonNotClosedAtEnd,

		}

	}
}
