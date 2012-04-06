using System.Collections.Generic;
using System.Windows.Media.Media3D;
using Malsys.Evaluators;
using Malsys.Media;
using Malsys.Processing.Components.Renderers;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;

namespace Malsys.Processing.Components.Interpreters {
	[Component("Turtle graphics interpreter (2D/3D)", ComponentGroupNames.Interpreters)]
	public class TurtleInterpreter : IInterpreter2D, IInterpreter3D {

		private IRenderer renderer;
		private IRenderer2D renderer2D;
		private IRenderer3D renderer3D;
		/// <summary>
		/// True if 2D renderer is attached false if 3D.
		/// </summary>
		private bool mode2D;

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

		private ColorParser colorParser;

		private Vector3D fwdVector = new Vector3D(1, 0, 0);
		private Vector3D upVector = new Vector3D(0, 1, 0);
		private Vector3D rightVector;  // counted from forward and up vectors by cross product

		private Point3D initPosition = new Point3D(0, 0, 0);
		private Quaternion initRotation = Quaternion.Identity;
		private double initWidth = 2;
		private ColorF initColor = ColorF.Black;

		private Stack<State3D> statesStack = new Stack<State3D>();

		// 2d and 3d polygons are separated because of performance
		private bool reversePolygonOrder = false;  // only for 2D
		private Polygon2D currPolygon2d;
		private Stack<Polygon2D> polygonStack2d = new Stack<Polygon2D>();
		private List<Polygon2D> polygonReverseHistory = new List<Polygon2D>();

		private Polygon3D currPolygon3d;
		private Stack<Polygon3D> polygonStack3d = new Stack<Polygon3D>();


		public TurtleInterpreter() {

			ReversePolygonOrder = Constant.False;

			quatRotation = new QuaternionRotation3D();
			tranform = new RotateTransform3D(quatRotation, 0, 0, 0);
		}


		#region User settable variables

		/// <summary>
		/// Reverses order of drawn polygons from first-opened last-drawn to first-opened first-drawn.
		/// This in only valid when 2D renderer is attached (in 3D is order insignificant).
		/// </summary>
		/// <expected>Boolean (true/false).</expected>
		/// <default>false</default>
		[AccessName("reversePolygonOrder")]
		[UserSettable]
		public Constant ReversePolygonOrder { get; set; }

		/// <summary>
		/// Origin (start position) of "turtle".
		/// </summary>
		/// <expected>Array of 2 or 3 constants representing x, y and optionally z coordinate.</expected>
		/// <default>{0, 0, 0}</default>
		[AccessName("origin")]
		[UserSettable]
		public ValuesArray Origin {
			set {
				if (!value.IsConstArray() || (value.Length != 2 && value.Length != 3)) {
					throw new InvalidUserValueException("Origin have to be array of 2 or 3 constants representing x, y and optionally z coordinate.");
				}

				initPosition.X = ((Constant)value[0]).Value;
				initPosition.Y = ((Constant)value[1]).Value;
				initPosition.Z = value.Length == 3 ? ((Constant)value[2]).Value : 0.0;
			}
		}

		/// <summary>
		/// Forward vector of "turtle".
		/// </summary>
		/// <expected>Array of 3 constants representing x, y and z coordinate.</expected>
		/// <default>{1, 0, 0}</default>
		[AccessName("forwardVector")]
		[UserSettable]
		public ValuesArray ForwardVector {
			set {
				if (!value.IsConstArrayOfLength(3)) {
					throw new InvalidUserValueException("Forward vector value must be array of 3 constants representing x, y and z coordinate.");
				}

				fwdVector.X = ((Constant)value[0]).Value;
				fwdVector.Y = ((Constant)value[1]).Value;
				fwdVector.Z = ((Constant)value[2]).Value;
			}
		}

		/// <summary>
		/// Up vector of "turtle".
		/// </summary>
		/// <expected>Array of 3 constants representing x, y and z coordinate.</expected>
		/// <default>{0, 1, 0}</default>
		[AccessName("upVector")]
		[UserSettable]
		public ValuesArray UpVector {
			set {
				if (!value.IsConstArrayOfLength(3)) {
					throw new InvalidUserValueException("Up vector value must be array of 3 constants representing x, y and z coordinate.");
				}

				upVector.X = ((Constant)value[0]).Value;
				upVector.Y = ((Constant)value[1]).Value;
				upVector.Z = ((Constant)value[2]).Value;
			}
		}



		[AccessName("rotationQuaternion")]
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

		/// <summary>
		/// Initial angle (in degrees) in 2D mode (angle in plane given by forward and up vectors).
		/// </summary>
		/// <expected>Constant representing angle in degrees.</expected>
		/// <default>0</default>
		[AccessName("initialAngle")]
		[UserSettable]
		public Constant InitialAngleZ { private get; set; }

		/// <summary>
		/// Initial width of drawn line.
		/// </summary>
		/// <expected>Constant representing width. Unit of value depends on used renderer.</expected>
		/// <default>2</default>
		[AccessName("initialLineWidth")]
		[UserSettable]
		public Constant InitialLineWidth {
			set { initWidth = value.Value; }
		}

		/// <summary>
		/// Initial color of drawn line.
		/// </summary>
		/// <expected>Constant representing ARGB color (in range from 0 to 2^32 - 1) or array of constants (in range from 0.0 to 1.0) with length of 3 (RGB) or 4 (ARGB).</expected>
		/// <default>0 (black)</default>
		[AccessName("initialColor")]
		[UserSettable]
		public ValuesArray InitialColor { get; set; }

		/// <summary>
		/// Continuous coloring gradient.
		/// If enabled all colors will be ignored and given gradient (or default gradient of rainbow) will be used to continuously color all objects.
		/// </summary>
		/// <expected>Boolean (true) to use default rainbow gradient or array representing color gradient (see documentation or examples for syntax).</expected>
		/// <default>false</default>
		[AccessName("continousColoring")]
		[UserSettable]
		public IValue ContinousColoring { get; set; }

		#endregion


		#region IInterpreter Members

		[UserConnectable]
		public IRenderer Renderer {
			set {
				var rendererType = value.GetType();
				// if renderer implements both 2D and 3D interfaces the 3D will be chosen
				if (typeof(IRenderer3D).IsAssignableFrom(rendererType)) {
					renderer2D = null;
					renderer3D = (IRenderer3D)value;
					renderer = renderer3D;
					mode2D = false;
				}
				else if (typeof(IRenderer2D).IsAssignableFrom(rendererType)) {
					renderer2D = (IRenderer2D)value;
					renderer3D = null;
					renderer = renderer2D;
					mode2D = true;
				}
				else {
					throw new InvalidConnectedComponentException("Renderer is not valid. Expected object implementing `{0}` or `{1}`."
						.Fmt(typeof(IRenderer3D).FullName, typeof(IRenderer2D).FullName));
				}
			}
		}


		public bool RequiresMeasure { get { return continousColoring; } }

		public void Initialize(ProcessContext ctxt) {

			logger = ctxt.Logger;
			colorParser = new ColorParser(ctxt.Logger);

			rightVector = Vector3D.CrossProduct(fwdVector, upVector);

			fwdVector.Normalize();
			upVector.Normalize();
			rightVector.Normalize();

			if (fwdVector.Length.EpsilonCompareTo(0) == 0) {
				throw new ComponentException("Forward vector must be non-zero.");
			}
			if (upVector.Length.EpsilonCompareTo(0) == 0) {
				throw new ComponentException("Up vector must be non-zero.");
			}
			if (rightVector.Length.EpsilonCompareTo(0) == 0) {
				throw new ComponentException("Forward and up vectors must be linearly independent.");
			}

			if (InitialAngleZ != null) {
				initRotation = new Quaternion(rightVector, InitialAngleZ.Value);
			}


			if (ContinousColoring != null) {
				if (ContinousColoring.IsConstant) {
					if (!((Constant)ContinousColoring).IsZero) {
						continousColoring = true;
						colorGradient = ColorGradients.Rainbow;
					}
				}
				else if (ContinousColoring.IsArray) {
					continousColoring = true;
					colorGradient = new ColorGradientFactory().CreateFromValuesArray((ValuesArray)ContinousColoring, logger);
				}
			}

			if (InitialColor != null) {
				colorParser.TryParseColor(InitialColor, out initColor);
			}

			bool reversePoly = !ReversePolygonOrder.IsZero;
			if (reversePoly && !mode2D) {
				logger.LogMessage(Message.ReversePolyIn3d);
				reversePolygonOrder = false;
			}
			else {
				reversePolygonOrder = reversePoly;
			}

		}

		public void Cleanup() { }

		public void BeginProcessing(bool measuring) {

			this.measuring = measuring;

			statesStack.Clear();
			if (mode2D) {
				currPolygon2d = null;
				polygonStack2d.Clear();
				polygonReverseHistory.Clear();
			}
			else {
				currPolygon3d = null;
				polygonStack3d.Clear();
			}

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
				Color = initColor,
				Variables = MapModule.Empty<string, IValue>()
			};

			renderer.BeginProcessing(measuring);
			if (mode2D) {
				renderer2D.InitializeState(currState.Position.ToPoint2D(), currState.Width, currState.Color);
			}
			else {
				renderer3D.InitializeState(currState.Position, currState.Rotation, currState.Width, currState.Color);
			}
		}

		public void EndProcessing() {

			renderer.EndProcessing();

			currState = null;

			if (measuring) {
				colorEventsMeasured = colorEvents;
			}
			else {
				if (statesStack.Count > 0) {
					logger.LogMessage(Message.BranchNotClosedAtEnd);
					statesStack.Clear();
				}

				if (mode2D) {
					if (polygonStack2d.Count > 0) {
						logger.LogMessage(Message.PolygonNotClosedAtEnd);
						if (reversePolygonOrder) {
							// if polygon order reverse is true polygons are not drawn until all are closed
							// draw at least all closed polygons
							foreach (var p in polygonReverseHistory) {
								renderer2D.DrawPolygon(p);
							}
						}
						currPolygon2d = null;
						polygonStack2d.Clear();
						polygonReverseHistory.Clear();
					}
				}
				else {
					if (polygonStack3d.Count > 0) {
						logger.LogMessage(Message.PolygonNotClosedAtEnd);
						currPolygon3d = null;
						polygonStack3d.Clear();
					}
				}
			}

		}

		#endregion


		#region IInterpreter2D & IInterpreter3D Members


		/// <summary>
		/// Symbol is ignored.
		/// </summary>
		[SymbolInterpretation]
		public void Nothing(ArgsStorage args) { }

		/// <summary>
		/// Moves forward in current direction (without drawing) by distance equal to value of the first parameter.
		/// </summary>
		/// <parameters>
		/// Moved distance.
		/// </parameters>
		[SymbolInterpretation(1, 1)]
		public void MoveForward(ArgsStorage args) {

			double length = getArgumentAsDouble(args, 0);

			quatRotation.Quaternion = currState.Rotation;
			currState.Position += tranform.Transform(fwdVector * length);

			if (mode2D) {
				renderer2D.MoveTo(currState.Position.ToPoint2D(), currState.Width, currState.Color);
			}
			else {
				renderer3D.MoveTo(currState.Position, currState.Rotation, currState.Width, currState.Color);
			}
		}

		/// <summary>
		/// Draws line in current direction with length equal to value of first parameter.
		/// </summary>
		/// <parameters>
		/// Length of line.
		/// Width of line.
		/// Color of line. Can be ARGB number in range from 0 to 2^32 - 1 or array with 3 (RGB) or 4 (ARGB) items in range from 0.0 to 1.0.
		/// </parameters>
		[SymbolInterpretation(3, 1)]
		public void DrawForward(ArgsStorage args) {

			double length = getArgumentAsDouble(args, 0);
			double width = getArgumentAsDouble(args, 1, currState.Width);
			ColorF color = getArgumentAsColor(args, 2);

			quatRotation.Quaternion = currState.Rotation;
			currState.Position += tranform.Transform(fwdVector * length);

			colorEvents++;
			if (mode2D) {
				renderer2D.DrawTo(currState.Position.ToPoint2D(), width, color);
			}
			else {
				renderer3D.DrawTo(currState.Position, currState.Rotation, width, color);
			}
		}

		/// <summary>
		/// Draws circle with center in current position and radius equal to value of the first parameter.
		/// </summary>
		[SymbolInterpretation(2, 1)]
		public void DrawCircle(ArgsStorage args) {
			DrawSphere(args);
		}

		/// <summary>
		/// Draws sphere with center in current position and radius equal to value of the first parameter.
		/// </summary>
		[SymbolInterpretation(2, 1)]
		public void DrawSphere(ArgsStorage args) {

			double radius = getArgumentAsDouble(args, 0);
			ColorF color = getArgumentAsColor(args, 1);

			colorEvents++;
			if (mode2D) {
				renderer2D.DrawCircle(currState.Position.ToPoint2D(), radius, color);
			}
			else {
				renderer3D.DrawSphere(currState.Position, radius, color);
			}
		}


		/// <summary>
		/// Turns left by value of the first parameter (in degrees) (in X-Y plane, around axis Z).
		/// </summary>
		[SymbolInterpretation(1)]
		public void TurnLeft(ArgsStorage args) {
			Pitch(args);
		}

		/// <summary>
		/// Turns left around up vector axis (default Y axis [0, 1, 0]) by value given in the first parameter (in degrees).
		/// </summary>
		[SymbolInterpretation(1)]
		public void Yaw(ArgsStorage args) {

			double angle = getArgumentAsDouble(args, 0);

			currState.Rotation *= new Quaternion(upVector, angle);
		}

		/// <summary>
		/// Turns up around right-hand vector axis (default Z axis [0, 0, 1]) by value given in the first parameter (in degrees).
		/// </summary>
		[SymbolInterpretation(1)]
		public void Pitch(ArgsStorage args) {

			double angle = getArgumentAsDouble(args, 0);

			currState.Rotation *= new Quaternion(rightVector, angle);
		}

		/// <summary>
		/// Rolls clock-wise around forward vector axis (default X axis [1, 0, 0]) by value given in the first parameter (in degrees).
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
			if (statesStack.Count == 0) {
				throw new InterpretationException("Failed to complete branch. No branch is opened.");
			}

			currState = statesStack.Pop();
			if (mode2D) {
				renderer2D.MoveTo(currState.Position.ToPoint2D(), currState.Width, currState.Color);
			}
			else {
				renderer3D.MoveTo(currState.Position, currState.Rotation, currState.Width, currState.Color);
			}
		}

		/// <summary>
		/// Starts to record polygon vertices (do not saves current position as first vertex).
		/// If another polygon is opened, its state is saved and will be restored after closing of current polygon.
		/// </summary>
		[SymbolInterpretation(3)]
		public void StartPolygon(ArgsStorage args) {

			ColorF color = getArgumentAsColor(args, 0);
			double strokeWidth = getArgumentAsDouble(args, 1, currState.Width);
			ColorF strokeColor = getArgumentAsColor(args, 2);

			colorEvents++;

			if (mode2D) {
				if (currPolygon2d != null) {
					polygonStack2d.Push(currPolygon2d);
				}
				currPolygon2d = new Polygon2D(color, strokeWidth, strokeColor);
				if (reversePolygonOrder) {
					polygonReverseHistory.Add(currPolygon2d);
				}
			}
			else {
				if (currPolygon3d != null) {
					polygonStack3d.Push(currPolygon3d);
				}
				currPolygon3d = new Polygon3D(color, strokeWidth, strokeColor);
			}

		}

		/// <summary>
		/// Records current position to opened polygon.
		/// </summary>
		[SymbolInterpretation]
		public void RecordPolygonVertex(ArgsStorage args) {

			if (mode2D) {
				if (currPolygon2d == null) {
					throw new InterpretationException("Failed to record polygon vertex. No polygon is opened.");
				}

				currPolygon2d.Ponits.Add(currState.Position.ToPoint2D());
			}
			else {
				if (currPolygon3d == null) {
					throw new InterpretationException("Failed to record polygon vertex. No polygon is opened.");
				}

				currPolygon3d.Ponits.Add(currState.Position);
			}
		}

		/// <summary>
		/// Ends current polygon (do not saves current position as last vertex).
		/// </summary>
		[SymbolInterpretation]
		public void EndPolygon(ArgsStorage args) {

			if (mode2D) {
				if (currPolygon2d == null) {
					throw new InterpretationException("Failed to end polygon. No polygon is opened.");
				}

				if (reversePolygonOrder) {
					if (polygonStack2d.Count == 0) {
						foreach (var p in polygonReverseHistory) {
							renderer2D.DrawPolygon(p);
						}
						polygonReverseHistory.Clear();
					}
				}
				else {
					renderer2D.DrawPolygon(currPolygon2d);
				}
				currPolygon2d = polygonStack2d.Count > 0 ? polygonStack2d.Pop() : null;
			}
			else {
				renderer3D.DrawPolygon(currPolygon3d);
				currPolygon3d = polygonStack3d.Count > 0 ? polygonStack3d.Pop() : null;
			}

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

		private ColorF getArgumentAsColor(ArgsStorage args, int index) {

			if (colorContinously) {
				return colorGradient[(float)colorEvents / colorEventsMeasured];
			}
			else if (index < args.ArgsCount) {
				ColorF color;
				if (colorParser.TryParseColor(args[index], out color)) {
					return color;
				}
			}

			return currState.Color;
		}


		public enum Message {

			[Message(MessageType.Warning, "Some branches were not closed while interpretation ended.")]
			BranchNotClosedAtEnd,
			[Message(MessageType.Warning, "Some polygons were not closed while interpretation ended.")]
			PolygonNotClosedAtEnd,
			[Message(MessageType.Warning, "Reversing polygon order has no sense in 3D, ignoring value.")]
			ReversePolyIn3d,

		}
	}
}
