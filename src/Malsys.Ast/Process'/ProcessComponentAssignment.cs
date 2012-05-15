/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ProcessComponentAssignment : IAstNode {

		public readonly Identifier ComponentTypeNameId;
		public readonly Identifier ContainerNameId;


		public ProcessComponentAssignment(Identifier componentType, Identifier container, PositionRange pos) {

			ComponentTypeNameId = componentType;
			ContainerNameId = container;

			Position = pos;
		}


		public PositionRange Position { get; private set; }

	}
}
