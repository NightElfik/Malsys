using System;
using System.Collections.Generic;
using System.Linq;
using Malsys.Evaluators;
using Malsys.Media;
using Malsys.Processing.Components.Renderers;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components.Interpreters.TwoD {
	[Component("2D Interpreter", ComponentGroupNames.Interpreters)]
	public class Interpreter2D : IInterpreter {

		private readonly ColorFactory colorFactory = new ColorFactory();


		private IRenderer2D renderer;
		private IMessageLogger logger;

		private State2D currState;

		private bool measuring;

		private uint drawCommandsCalled;
		private uint drawCommandsMeasured;

		private bool continousColoring = false;
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

		[UserSettable]
		public Constant InterpretLines {
			set { interpretLines = !value.IsZero; }
		}

		[UserSettable]
		public Constant InterpretPolygons {
			set { interpretPloygons = !value.IsZero; }
		}

		[UserSettable]
		public Constant ReversePolygonOrder {
			set { reversePolygonOrder = !value.IsZero; }
		}

		[UserSettable]
		public ValuesArray Origin {
			set {
				if (!value.IsConstArrayOfLength(2)) {
					throw new InvalidUserValueException("Origin have to be array of 2 constants representing x and y coordination, like `{-1, 3}`.");
				}

				initX = ((Constant)value[0]).Value;
				initY = ((Constant)value[1]).Value;
			}
		}

		[UserSettable]
		public Constant InitialAngle {
			set { initAngle = value.Value % 360; }
		}

		[UserSettable]
		public Constant InitialLineWidth {
			set { initWdth = value.Value; }
		}

		[UserSettable]
		public ValuesArray InitialColor { get; set; }


		[UserSettable]
		public IValue ContinousColoring { get; set; }

		#endregion


		#region IInterpreter Members

		[UserConnectable]
		public IRenderer Renderer {
			set {
				if (!IsRendererCompatible(value)) {
					throw new ArgumentException("Renderer is not compatible.");
				}
				renderer = (IRenderer2D)value;
			}
		}


		public bool IsRendererCompatible(IRenderer renderer) {
			return renderer.GetType().GetInterfaces().Contains(typeof(IRenderer2D));
		}

		public bool RequiresMeasure { get { return continousColoring; } }

		public void Initialize(ProcessContext ctxt) {

			logger = ctxt.Logger;

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
				initColor = colorFactory.FromIValue(InitialColor, logger);
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
				drawCommandsMeasured = 0;
				colorContinously = false;
			}
			else {
				colorContinously = continousColoring;
			}

			drawCommandsCalled = 0;
			currState = new State2D() {
				X = initX,
				Y = initY,
				LineWidth = initWdth,
				CurrentAngle = initAngle,
				Color = initColor
			};

			renderer.InitializeState(new PointF((float)currState.X, (float)currState.Y), (float)currState.LineWidth, currState.Color);
			renderer.BeginProcessing(measuring);
		}

		public void EndProcessing() {

			if (measuring) {
				drawCommandsMeasured = drawCommandsCalled;
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
				ColorF color = colorContinously ? colorGradient[(float)drawCommandsCalled / drawCommandsMeasured] : currState.Color;
				renderer.MoveTo(new PointF((float)currState.X, (float)currState.Y), (float)currState.LineWidth, color);
			}
		}

		/// <summary>
		/// Draws line in current direction with length equal to value of first parameter.
		/// </summary>
		[SymbolInterpretation(1)]
		public void DrawLine(ArgsStorage args) {

			double length = getArgumentAsDouble(args, 0);

			currState.X += length * Math.Cos(currState.CurrentAngle * MathHelper.PiOver180);
			currState.Y += length * Math.Sin(currState.CurrentAngle * MathHelper.PiOver180);

			if (interpretLines) {
				double width = getArgumentAsDouble(args, 1, currState.LineWidth);

				ColorF color = currState.Color;

				if (colorContinously) {
					color = colorGradient[(float)drawCommandsCalled / drawCommandsMeasured];
				} else if (args.ArgsCount >= 3) {
					colorFactory.ParseColor(args[2], ref color);
				}

				drawCommandsCalled++;
				renderer.LineTo(new PointF((float)currState.X, (float)currState.Y), (float)width, color);
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

			if (args.ArgsCount >= 3) {
				colorFactory.ParseColor(args[2], ref strokeColor);
			}

			if (args.ArgsCount >= 2 && args[1].IsConstant) {
				strokeWidth = (float)((Constant)args[1]).Value;
			}

			if (args.ArgsCount >= 1) {
				colorFactory.ParseColor(args[0], ref color);
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
