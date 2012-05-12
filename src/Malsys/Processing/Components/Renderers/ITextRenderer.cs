/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.Processing.Components.Renderers {
	/// <summary>
	/// Provides commands for rendering plain text.
	/// </summary>
	/// <name>Text renderer interface</name>
	/// <group>Renderers</group>
	public interface ITextRenderer : IRenderer {

		/// <summary>
		/// Writes given character at given coordinates.
		/// Coordinates can be both positive and negative.
		/// </summary>
		void PutCharAt(char c, int x, int y);

	}
}
