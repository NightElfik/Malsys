/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media.Media3D;
using Malsys.Evaluators;
using Malsys.Media;
using Malsys.Processing.Components.Renderers;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Collections;

namespace Malsys.Processing.Components.Interpreters {
	/// <summary>
	/// Turtle interpreter interprets symbols as basic 2D or 3D graphics primitives.
	/// Interpreting is always in 3D but if it is connected 2D renderer
	/// (component with interface IRenderer2D) the Z coordinate is omitted.
	/// </summary>
	/// <name>Turtle interpreter</name>
	/// <group>Interpreters</group>
	public class TurtleInterpreter : IInterpreter2D, IInterpreter3D {

		private IRenderer renderer;
		private IRenderer2D renderer2D;
		private IRenderer3D renderer3D;
		/// <summary>
		/// True if 2D renderer is attached false if 3D.
		/// </summary>
		private bool mode2D;

		private State3D currState;

		private QuaternionRotation3D quatRotation;
		private RotateTransform3D tranform;

		private bool measuring;

		private bool applyTropism;
		private double tropismCoef;
		private Vector3D tropismVect;

		private uint colorEvents;
		private uint colorEventsMeasured;

		private bool continousColoring = false;
		private bool colorContinously = false;
		private ColorGradient colorGradient;

		private ColorParser colorParser;

		private Vector3D fwdVect;
		private Vector3D upVect;
		private Vector3D rightVect;  // counted from forward and up vectors by cross product

		private Point3D initPosition;
		private Quaternion initRotation;
		private ColorF initColor;

		private Stack<State3D> statesStack = new Stack<State3D>();

		// 2d and 3d polygons are separated because of performance
		private bool reversePolygonOrder = false;  // only for 2D
		private Polygon2D currPolygon2d;
		private Stack<Polygon2D> polygonStack2d = new Stack<Polygon2D>();
		private List<Polygon2D> polygonReverseHistory = new List<Polygon2D>();

		private Polygon3D currPolygon3d;
		private Stack<Polygon3D> polygonStack3d = new Stack<Polygon3D>();


		public TurtleInterpreter() {
			quatRotation = new QuaternionRotation3D();
			tranform = new RotateTransform3D(quatRotation, 0, 0, 0);
		}


		public IMessageLogger Logger { get; set; }


		#region User gettable & settable properties

		/// <summary>
		/// Origin (start position) of "turtle".
		/// </summary>
		/// <expected>Array of 2 or 3 numbers representing x, y and optionally z coordinate.</expected>
		/// <default>{0, 0, 0}</default>
		[AccessName("origin")]
		[UserGettable(IsGettableBeforeInitialiation = true)]
		[UserSettable]
		public ValuesArray Origin {
			get { return origin; }
			set {
				if (!value.IsConstArray() || (value.Length != 2 && value.Length != 3)) {
					throw new InvalidUserValueException("Origin must be array of 2 or 3 numbers representing x, y and optionally z coordinate.");
				}
				origin = value;
			}
		}
		private ValuesArray origin;

		/// <summary>
		/// Forward vector of "turtle".
		/// </summary>
		/// <expected>Array of 3 numbers representing x, y and z coordinate.</expected>
		/// <default>{1, 0, 0}</default>
		[AccessName("forwardVector")]
		[UserGettable(IsGettableBeforeInitialiation = true)]
		[UserSettable]
		public ValuesArray ForwardVector {
			get { return forwardVector; }
			set {
				if (!value.IsConstArrayOfLength(3)) {
					throw new InvalidUserValueException("Forward vector value must be array of 3 numbers representing x, y and z coordinate.");
				}
				forwardVector = value;
			}
		}
		private ValuesArray forwardVector;

		/// <summary>
		/// Up vector of "turtle".
		/// </summary>
		/// <expected>Array of 3 constants representing x, y and z coordinate.</expected>
		/// <default>{0, 1, 0}</default>
		[AccessName("upVector")]
		[UserGettable(IsGettableBeforeInitialiation = true)]
		[UserSettable]
		public ValuesArray UpVector {
			get { return upVector; }
			set {
				if (!value.IsConstArrayOfLength(3)) {
					throw new InvalidUserValueException("Up vector value must be array of 3 numbers representing x, y and z coordinate.");
				}
				upVector = value;
			}
		}
		private ValuesArray upVector;



		[AccessName("rotationQuaternion")]
		[UserSettable]
		public ValuesArray RotationQuaternion {
			set {
				if (!value.IsConstArrayOfLength(4)) {
					throw new InvalidUserValueException("Rotation quaternion must be array of 4 numbers representing direction quaternion.");
				}
				rotationQuaternion = value;
			}
		}
		private ValuesArray rotationQuaternion;

		/// <summary>
		/// Initial angle (in degrees) in 2D mode (angle in plane given by forward and up vectors).
		/// </summary>
		/// <expected>Number representing angle in degrees.</expected>
		/// <default>0</default>
		[AccessName("initialAngle")]
		[UserSettable]
		public Constant InitialAngleZ { get; set; }

		/// <summary>
		/// Initial width of drawn line.
		/// </summary>
		/// <expected>Number representing width. Unit of value depends on used renderer.</expected>
		/// <default>2</default>
		[AccessName("initialLineWidth")]
		[UserSettable]
		public Constant InitialLineWidth { get; set; }

		/// <summary>
		/// Initial color of drawn line.
		/// </summary>
		/// <expected>Number representing ARGB color (in range from 0 to 2^32 - 1) or array of numbers (in range from 0.0 to 1.0) with length of 3 (RGB) or 4 (ARGB).</expected>
		/// <default>0 (black)</default>
		[AccessName("initialColor")]
		[UserSettable]
		public IValue InitialColor { get; set; }

		/// <summary>
		/// Continuous coloring gradient.
		/// If enabled all colors will be ignored and given gradient (or default gradient of rainbow) will be used to continuously color all objects.
		/// </summary>
		/// <expected>Boolean false disables continuous coloring, true uses default rainbow gradient to continuous coloring.
		///		Array representing color gradient can be also set (see documentation or examples for syntax).</expected>
		/// <default>false</default>
		[AccessName("continuousColoring")]
		[UserSettable]
		public IValue ContinuousColoring { get; set; }

		/// <summary>
		/// Reverses order of drawn polygons from first-opened last-drawn to first-opened first-drawn.
		/// This in only valid when 2D renderer is attached (in 3D is order insignificant).
		/// </summary>
		/// <expected>true or false</expected>
		/// <default>false</default>
		[AccessName("reversePolygonOrder")]
		[UserSettable]
		public Constant ReversePolygonOrder { get; set; }

		/// <summary>
		/// Tropism vector affects drawn or moved lines to derive to tropism vector.
		/// </summary>
		/// <expected>Array of 3 constants representing x, y and z coordinate.</expected>
		/// <default>{0, 1, 0}</default>
		[AccessName("tropismVector")]
		[UserSettable]
		public ValuesArray TropismVector {
			get { return tropismVector; }
			set {
				if (!value.IsConstArrayOfLength(3)) {
					throw new InvalidUserValueException("Tropism vector value must be array of 3 numbers representing x, y and z coordinate.");
				}
				tropismVector = value;
			}
		}
		private ValuesArray tropismVector;

		/// <summary>
		/// Tropism coefficient affects speed of derivation to tropism vector.
		/// </summary>
		/// <expected>Number.</expected>
		/// <default>0</default>
		[AccessName("tropismCoefficient")]
		[UserSettable]
		public Constant TropismCoefficient { get; set; }

		#endregion


		#region IInterpreter Members

		/// <summary>
		/// All render events will be called on connected renderer.
		/// Both IRenderer2D or IRenderer3D can be connected.
		/// </summary>
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

			initPosition.X = ((Constant)origin[0]).Value;
			initPosition.Y = ((Constant)origin[1]).Value;
			initPosition.Z = origin.Length == 3 ? ((Constant)origin[2]).Value : 0.0;

			fwdVect.X = ((Constant)forwardVector[0]).Value;
			fwdVect.Y = ((Constant)forwardVector[1]).Value;
			fwdVect.Z = ((Constant)forwardVector[2]).Value;

			upVect.X = ((Constant)upVector[0]).Value;
			upVect.Y = ((Constant)upVector[1]).Value;
			upVect.Z = ((Constant)upVector[2]).Value;

			colorParser = new ColorParser(Logger);

			rightVect = Vector3D.CrossProduct(fwdVect, upVect);

			fwdVect.Normalize();
			upVect.Normalize();
			rightVect.Normalize();

			if (double.IsNaN(fwdVect.Length) || fwdVect.Length.EpsilonCompareTo(0) == 0) {
				throw new ComponentException("Forward vector must be non-zero.");
			}
			if (double.IsNaN(upVect.Length) || upVect.Length.EpsilonCompareTo(0) == 0) {
				throw new ComponentException("Up vector must be non-zero.");
			}
			if (double.IsNaN(rightVect.Length) || rightVect.Length.EpsilonCompareTo(0) == 0) {
				throw new ComponentException("Forward and up vectors must be linearly independent.");
			}

			initRotation.W = ((Constant)rotationQuaternion[0]).Value;
			initRotation.X = ((Constant)rotationQuaternion[1]).Value;
			initRotation.Y = ((Constant)rotationQuaternion[2]).Value;
			initRotation.Z = ((Constant)rotationQuaternion[3]).Value;

			if (!InitialAngleZ.IsZero) {
				initRotation = new Quaternion(rightVect, InitialAngleZ.Value);
			}

			if (InitialColor != null) {
				colorParser.TryParseColor(InitialColor, out initColor, Logger);
			}

			if (ContinuousColoring != null) {
				if (ContinuousColoring.IsConstant) {
					if (((Constant)ContinuousColoring).IsTrue) {
						continousColoring = true;
						colorGradient = ColorGradients.Rainbow;
					}
				}
				else if (ContinuousColoring.IsArray) {
					continousColoring = true;
					colorGradient = new ColorGradientFactory().CreateFromValuesArray((ValuesArray)ContinuousColoring, Logger);
				}
			}

			bool reversePoly = ReversePolygonOrder.IsTrue;
			if (reversePoly && !mode2D) {
				Logger.LogMessage(Message.ReversePolyIn3d);
				reversePolygonOrder = false;
			}
			else {
				reversePolygonOrder = reversePoly;
			}

			applyTropism = !TropismCoefficient.IsZero;
			if (applyTropism) {
				tropismCoef = TropismCoefficient.Value;
				if (double.IsNaN(tropismCoef) || double.IsInfinity(tropismCoef)) {
					applyTropism = false;
					Logger.LogMessage(Message.InvalidTropismCoefValue);
				}

				tropismVect.X = ((Constant)tropismVector[0]).Value;
				tropismVect.Y = ((Constant)tropismVector[1]).Value;
				tropismVect.Z = ((Constant)tropismVector[2]).Value;

				if (double.IsNaN(tropismVect.Length) || tropismVect.Length.EpsilonCompareTo(0) == 0) {
					applyTropism = false;
					Logger.LogMessage(Message.InvalidTropismVectValue);
				}
			}

		}

		public void Cleanup() {
			Origin = new ValuesArray(Constant.Zero, Constant.Zero, Constant.Zero);
			ForwardVector = new ValuesArray(Constant.One, Constant.Zero, Constant.Zero);
			UpVector = new ValuesArray(Constant.Zero, Constant.One, Constant.Zero);
			InitialAngleZ = Constant.Zero;
			InitialLineWidth = new Constant(2d);
			InitialColor = Constant.Zero;
			ContinuousColoring = Constant.False;
			ReversePolygonOrder = Constant.False;
			RotationQuaternion = new ValuesArray(Constant.One, Constant.Zero, Constant.Zero, Constant.Zero);
			TropismVector = new ValuesArray(Constant.Zero, Constant.One, Constant.Zero);
			TropismCoefficient = Constant.Zero;
		}

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
				Width = InitialLineWidth.Value,
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
					Logger.LogMessage(Message.BranchNotClosedAtEnd);
					statesStack.Clear();
				}

				if (mode2D) {
					if (polygonStack2d.Count > 0) {
						Logger.LogMessage(Message.PolygonNotClosedAtEnd);
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
						Logger.LogMessage(Message.PolygonNotClosedAtEnd);
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
			Vector3D moveVector = tranform.Transform(fwdVect * length);
			currState.Position += moveVector;

			if (double.IsNaN(currState.Position.X) || double.IsNaN(currState.Position.Y) || double.IsNaN(currState.Position.Z)) {
				return;
			}

			if (mode2D) {
				renderer2D.MoveTo(currState.Position.ToPoint2D(), currState.Width, currState.Color);
			}
			else {
				renderer3D.MoveTo(currState.Position, currState.Rotation, currState.Width, currState.Color);
			}

			if (applyTropism) {
				rotateByTropism(moveVector);
			}
		}

		/// <summary>
		/// Draws line in current direction with length equal to value of first parameter.
		/// </summary>
		/// <parameters>
		/// Length of line.
		/// Width of line.
		/// Color of line. Can be ARGB number in range from 0 to 2^32 - 1 or array with 3 (RGB) or 4 (ARGB) items in range from 0.0 to 1.0.
		/// Quality of rendered line in 3D.
		/// </parameters>
		[SymbolInterpretation(4, 1)]
		public void DrawForward(ArgsStorage args) {

			double length = getArgumentAsDouble(args, 0);
			double width = getArgumentAsDouble(args, 1, currState.Width);
			ColorF color = getArgumentAsColor(args, 2);

			quatRotation.Quaternion = currState.Rotation;
			Vector3D moveVector = tranform.Transform(fwdVect * length);
			currState.Position += moveVector;

			if (double.IsNaN(currState.Position.X) || double.IsNaN(currState.Position.Y) || double.IsNaN(currState.Position.Z)) {
				return;
			}

			colorEvents++;
			if (mode2D) {
				renderer2D.DrawTo(currState.Position.ToPoint2D(), width, color);
			}
			else {
				double quality = getArgumentAsDouble(args, 3);
				renderer3D.DrawTo(currState.Position, currState.Rotation, width, color, quality);
			}

			if (applyTropism) {
				rotateByTropism(moveVector);
			}
		}

		/// <summary>
		/// Draws circle with center in current position and radius equal to value of the first parameter.
		/// </summary>
		/// <parameters>
		/// Radius of circle.
		/// Color of circle.
		/// </parameters>
		[SymbolInterpretation(2, 1)]
		public void DrawCircle(ArgsStorage args) {
			DrawSphere(args);
		}

		/// <summary>
		/// Draws sphere with center in current position and radius equal to value of the first parameter.
		/// </summary>
		/// <parameters>
		/// Radius of sphere.
		/// Color of sphere.
		/// Quality of sphere (number of triangles).
		/// </parameters>
		[SymbolInterpretation(3, 1)]
		public void DrawSphere(ArgsStorage args) {

			double radius = getArgumentAsDouble(args, 0);
			ColorF color = getArgumentAsColor(args, 1);

			colorEvents++;
			if (mode2D) {
				renderer2D.DrawCircle(currState.Position.ToPoint2D(), radius, color);
			}
			else {
				double quality = getArgumentAsDouble(args, 2);
				renderer3D.DrawSphere(currState.Position, currState.Rotation, radius, color, quality);
			}
		}


		/// <summary>
		/// Turns left by value of the first parameter (in degrees) (in X-Y plane, around axis Z).
		/// </summary>
		/// <parameters>
		/// Angle in degrees.
		/// </parameters>
		[SymbolInterpretation(1)]
		public void TurnLeft(ArgsStorage args) {
			Pitch(args);
		}

		/// <summary>
		/// Turns left around up vector axis (default Y axis [0, 1, 0]) by value given in the first parameter (in degrees).
		/// </summary>
		/// <parameters>
		/// Angle in degrees.
		/// </parameters>
		[SymbolInterpretation(1)]
		public void Yaw(ArgsStorage args) {

			double angle = getArgumentAsDouble(args, 0);

			currState.Rotation *= new Quaternion(upVect, angle);
		}

		/// <summary>
		/// Turns up around right-hand vector axis (default Z axis [0, 0, 1]) by value given in the first parameter (in degrees).
		/// </summary>
		/// <parameters>
		/// Angle in degrees.
		/// </parameters>
		[SymbolInterpretation(1)]
		public void Pitch(ArgsStorage args) {

			double angle = getArgumentAsDouble(args, 0);

			currState.Rotation *= new Quaternion(rightVect, angle);
		}

		/// <summary>
		/// Rolls clock-wise around forward vector axis (default X axis [1, 0, 0]) by value given in the first parameter (in degrees).
		/// </summary>
		/// <parameters>
		/// Angle in degrees.
		/// </parameters>
		[SymbolInterpretation(1)]
		public void Roll(ArgsStorage args) {

			double angle = getArgumentAsDouble(args, 0);

			currState.Rotation *= new Quaternion(fwdVect, angle);
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
		/// <parameters>
		/// Color of polygon.
		/// Stroke width.
		/// Stroke color.
		/// </parameters>
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

			if (double.IsNaN(currState.Position.X) || double.IsNaN(currState.Position.Y) || double.IsNaN(currState.Position.Z)) {
				return;
			}

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
				double value = ((Constant)args[index]).Value;
				if (double.IsNaN(value)) {
					return defaultValue;
				}
				else {
					return value;
				}
			}
			else {
				return defaultValue;
			}
		}

		private ColorF getArgumentAsColor(ArgsStorage args, int index) {

			if (colorContinously) {
				return colorGradient.GetColorbyPercentage((double)colorEvents / (double)colorEventsMeasured);
			}
			else if (index < args.ArgsCount) {
				ColorF color;
				if (colorParser.TryParseColor(args[index], out color)) {
					return color;
				}
			}

			return currState.Color;
		}


		private void rotateByTropism(Vector3D moveVector) {

			Vector3D axis = Vector3D.CrossProduct(moveVector, tropismVect);
			double angle = axis.Length * tropismCoef;
			if (angle.EpsilonCompareTo(0) == 0) {
				return;
			}

			axis.Normalize();
			currState.Rotation = new Quaternion(axis, angle) * currState.Rotation;
		}


		public enum Message {

			[Message(MessageType.Warning, "Some branches were not closed while interpretation ended.")]
			BranchNotClosedAtEnd,
			[Message(MessageType.Warning, "Some polygons were not closed while interpretation ended.")]
			PolygonNotClosedAtEnd,
			[Message(MessageType.Warning, "Reversing polygon order has no sense in 3D, ignoring value.")]
			ReversePolyIn3d,
			[Message(MessageType.Warning, "Invalid tropism coefficient value, ignoring tropism.")]
			InvalidTropismCoefValue,
			[Message(MessageType.Warning, "Invalid tropism vector value, ignoring tropism.")]
			InvalidTropismVectValue,

		}
	}
}
