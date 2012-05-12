/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using Malsys.Evaluators;

namespace Malsys.Processing.Components.Interpreters {
	/// <summary>
	/// 3D interpreters interprets L-system symbols as scene in 3D space.
	/// </summary>
	/// <name>3D interpreter interface</name>
	/// <group>Interpreters</group>
	public interface IInterpreter3D : IInterpreter {

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
		/// Draws sphere with center in current position.
		/// </summary>
		[SymbolInterpretation]
		void DrawSphere(ArgsStorage args);


		/// <summary>
		/// Turns left around up vector axis.
		/// </summary>
		[SymbolInterpretation]
		void Yaw(ArgsStorage args);

		/// <summary>
		/// Turns up around right-hand vector axis.
		/// </summary>
		[SymbolInterpretation]
		void Pitch(ArgsStorage args);

		/// <summary>
		/// Rolls clock-wise around forward vector axis.
		/// </summary>
		[SymbolInterpretation]
		void Roll(ArgsStorage args);


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
