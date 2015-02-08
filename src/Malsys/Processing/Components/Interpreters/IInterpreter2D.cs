using Malsys.Evaluators;

namespace Malsys.Processing.Components.Interpreters {
	/// <summary>
	/// 2D interpreters interprets L-system symbols as image on 2D canvas.
	/// </summary>
	/// <name>2D interpreter interface</name>
	/// <group>Interpreters</group>
	public interface IInterpreter2D : IInterpreter {

		/// <summary>
		/// Symbol is ignored.
		/// </summary>
		[SymbolInterpretation]
		void Nothing(ArgsStorage args);

		/// <summary>
		/// Moves forward in current direction (without drawing).
		/// </summary>
		[SymbolInterpretation]
		void MoveForward(ArgsStorage args);

		/// <summary>
		/// Draws line in current direction.
		/// </summary>
		[SymbolInterpretation]
		void DrawForward(ArgsStorage args);

		/// <summary>
		/// Draws circle with center in current position.
		/// </summary>
		[SymbolInterpretation]
		void DrawCircle(ArgsStorage args);

		/// <summary>
		/// Turns left.
		/// </summary>
		[SymbolInterpretation]
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
		/// Starts to record polygon vertices.
		/// </summary>
		[SymbolInterpretation]
		void StartPolygon(ArgsStorage args);

		/// <summary>
		/// Records current position to opened polygon.
		/// </summary>
		[SymbolInterpretation]
		void RecordPolygonVertex(ArgsStorage args);

		/// <summary>
		/// Ends current polygon.
		/// </summary>
		[SymbolInterpretation]
		void EndPolygon(ArgsStorage args);


	}
}
