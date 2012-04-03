using System;
using System.Collections.Generic;
using Malsys.Evaluators;
using Malsys.Media;
using Malsys.Processing.Components.Renderers;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components.Interpreters {
	[Component("2D interpreter", ComponentGroupNames.Interpreters)]
	public class Interpreter2D : IInterpreter {


		private IRenderer2D renderer;
		private IMessageLogger logger;

		private State2D currState;

		private bool measuring;

		private uint colorEvents;
		private uint colorEventsMeasured;

		private bool continuousColoring = false;
		private bool colorContinously = false;
		private ColorGradient colorGradient;

		private bool interpretLines = true;
		private bool interpretPloygons = true;

		private double initX = 0, initY = 0;
		private double initAngle = 0;
		private double initWdth = 2;
		private ColorF initColor = ColorF.Black;
		private Stack<State2D> statesStack = new Stack<State2D>();

		private bool reversePolygonOrder = false;
		private Polygon2D currPolygon;
		private Stack<Polygon2D> polygonStack = new Stack<Polygon2D>();
		private List<Polygon2D> polygonReverseHistory = new List<Polygon2D>();


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

		[Alias("reversePolygonOrder")]
		[UserSettable]
		public Constant ReversePolygonOrder {
			set { reversePolygonOrder = !value.IsZero; }
		}

		[Alias("origin")]
		[UserSettable]
		public ValuesArray Origin {
			set {
				if (!value.IsConstArrayOfLength(2)) {
					throw new InvalidUserValueException("Origin have to be array of 2 constants representing x and y coordinate.");
				}

				initX = ((Constant)value[0]).Value;
				initY = ((Constant)value[1]).Value;
			}
		}

		[Alias("initialAngle")]
		[UserSettable]
		public Constant InitialAngle {
			set { initAngle = value.Value % 360; }
		}

		[Alias("initialLineWidth")]
		[UserSettable]
		public Constant InitialLineWidth {
			set { initWdth = value.Value; }
		}

		[Alias("initialColor")]
		[UserSettable]
		public ValuesArray InitialColor { get; set; }


		[Alias("continuousColoring")]
		[UserSettable]
		public IValue ContinuousColoring { get; set; }

		#endregion


		#region IInterpreter Members
		// optional for rendering L-system in L-system, there is no connection for renderer
		// the check has to be done in initialization
		[UserConnectable(IsOptional = true)]
		public IRenderer Renderer {
			set {
				if (!typeof(IRenderer2D).IsAssignableFrom(value.GetType())) {
					throw new InvalidConnectedComponentException("Renderer do not implement `{0}`.".Fmt(typeof(IRenderer2D).FullName));
				}
				renderer = (IRenderer2D)value;
			}
		}


		public bool RequiresMeasure { get { return continuousColoring; } }

		public void Initialize(ProcessContext ctxt) {

			if (renderer == null) {
				throw new ComponentException("Renderer is not set.");
			}

			logger = ctxt.Logger;

			if (ContinuousColoring != null) {
				if (ContinuousColoring.IsConstant) {
					if (!((Constant)ContinuousColoring).IsZero) {
						continuousColoring = true;
						colorGradient = ColorGradients.Rainbow;
					}
				}
				else {
					continuousColoring = true;
					colorGradient = new ColorGradientFactory().CreateFromValuesArray((ValuesArray)ContinuousColoring, logger);
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
			polygonReverseHistory.Clear();

			if (measuring) {
				colorEventsMeasured = 0;
				colorContinously = false;
			}
			else {
				colorContinously = continuousColoring;
			}

			colorEvents = 0;
			currState = new State2D() {
				X = initX,
				Y = initY,
				LineWidth = initWdth,
				CurrentAngle = initAngle,
				Color = initColor
			};

			renderer.BeginProcessing(measuring);
			renderer.InitializeState(new PointF((float)currState.X, (float)currState.Y), (float)currState.LineWidth, currState.Color);
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
				polygonReverseHistory.Clear();
			}

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
		/// Moves pen forward (without drawing) by distance equal to value of the first parameter.
		/// </summary>
		[SymbolInterpretation(1)]
		public void MoveForward(ArgsStorage args) {

			double length = getArgumentAsDouble(args, 0);

			currState.X += length * Math.Cos(currState.CurrentAngle * MathHelper.PiOver180);
			currState.Y += length * Math.Sin(currState.CurrentAngle * MathHelper.PiOver180);

			if (interpretLines) {
				ColorF color = colorContinously ? colorGradient[(float)colorEvents / colorEventsMeasured] : currState.Color;
				renderer.MoveTo(new PointF((float)currState.X, (float)currState.Y), (float)currState.LineWidth, color);
			}
		}

		/// <summary>
		/// Draws line in current direction with length equal to value of first parameter.
		/// </summary>
		[SymbolInterpretation(1)]
		public void DrawForward(ArgsStorage args) {

			double length = getArgumentAsDouble(args, 0);

			currState.X += length * Math.Cos(currState.CurrentAngle * MathHelper.PiOver180);
			currState.Y += length * Math.Sin(currState.CurrentAngle * MathHelper.PiOver180);

			if (interpretLines) {
				double width = getArgumentAsDouble(args, 1, currState.LineWidth);

				ColorF color = currState.Color;

				if (colorContinously) {
					color = colorGradient[(float)colorEvents / colorEventsMeasured];
				}
				else if (args.ArgsCount >= 3) {
					ColorHelper.ParseColor(args[2], ref color);
				}

				colorEvents++;
				renderer.DrawTo(new PointF((float)currState.X, (float)currState.Y), (float)width, color);
			}
		}

		/// <summary>
		/// Adds value of first parameter (in degrees) to current direction angle.
		/// </summary>
		[SymbolInterpretation(1)]
		public void TurnLeft(ArgsStorage args) {

			double angle = getArgumentAsDouble(args, 0);

			currState.CurrentAngle += angle;
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
				renderer.MoveTo(new PointF((float)currState.X, (float)currState.Y), (float)currState.LineWidth, currState.Color);
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
			float strokeWidth = (float)currState.LineWidth;



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


			currPolygon = new Polygon2D(color, strokeWidth, strokeColor);
			if (reversePolygonOrder) {
				polygonReverseHistory.Add(currPolygon);
			}

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

			currPolygon.Ponits.Add(new PointF((float)currState.X, (float)currState.Y));
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

			if (reversePolygonOrder) {
				if (polygonStack.Count == 0) {
					foreach (var p in polygonReverseHistory) {
						renderer.DrawPolygon(p);
					}
					polygonReverseHistory.Clear();
				}

			}
			else {
				renderer.DrawPolygon(currPolygon);
			}

			currPolygon = polygonStack.Count > 0 ? polygonStack.Pop() : null;
		}

		[SymbolInterpretation]
		public void DrawCircle(ArgsStorage args) {

			double radius = getArgumentAsDouble(args, 0);

			ColorF color = currState.Color;

			if (colorContinously) {
				color = colorGradient[(float)colorEvents / colorEventsMeasured];
			}
			else if (args.ArgsCount >= 2) {
				ColorHelper.ParseColor(args[1], ref color);
			}

			colorEvents++;
			renderer.DrawCircle(new PointF((float)currState.X, (float)currState.Y), (float)radius, color);

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
