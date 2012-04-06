using Malsys.Evaluators;

namespace Malsys.Processing.Components.Interpreters {
	public interface IInterpreter2D : IInterpreter {

		/// <summary>
		/// Symbol is ignored.
		/// </summary>
		[SymbolInterpretation]
		void Nothing(ArgsStorage args);

		/// <summary>
		/// Moves forward in current direction (without drawing) by distance equal to value of the first parameter.
		/// </summary>
		[SymbolInterpretation(1)]
		void MoveForward(ArgsStorage args);

		/// <summary>
		/// Draws line in current direction with length equal to value of the first parameter.
		/// </summary>
		[SymbolInterpretation(1)]
		void DrawForward(ArgsStorage args);

		/// <summary>
		/// Adds value of the first parameter (in degrees) to current direction angle.
		/// </summary>
		[SymbolInterpretation(1)]
		void TurnLeft(ArgsStorage args);

		/// <summary>
		/// Saves current state (on stack).
		/// </summary>
		[SymbolInterpretation]
		void StartBranch(ArgsStorage args);

		/// <summary>
		/// Loads previously saved state (returns to last saved position).
		/// </summary>
		[SymbolInterpretation]
		void EndBranch(ArgsStorage args);

		/// <summary>
		/// Starts to record polygon vertices (do not saves current position as first vertex).
		/// If another polygon is opened its state is saved and will be restored after closing of current polygon.
		/// </summary>
		[SymbolInterpretation]
		void StartPolygon(ArgsStorage args);

		/// <summary>
		/// Records current position to opened polygon.
		/// </summary>
		[SymbolInterpretation]
		void RecordPolygonVertex(ArgsStorage args);

		/// <summary>
		/// Ends current polygon (do not saves current position as last vertex).
		/// </summary>
		[SymbolInterpretation]
		void EndPolygon(ArgsStorage args);

		/// <summary>
		/// Draws circle with center in current position with radius equal to value of the first parameter.
		/// </summary>
		[SymbolInterpretation(1)]
		void DrawCircle(ArgsStorage args);


	}
}
