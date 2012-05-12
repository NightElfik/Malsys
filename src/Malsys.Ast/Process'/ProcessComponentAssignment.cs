/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.Ast {
	/// <remarks>
	/// Immutable.
	/// </remarks>
	public class ProcessComponentAssignment : IToken {

		public readonly Identificator ComponentTypeNameId;
		public readonly Identificator ContainerNameId;


		public ProcessComponentAssignment(Identificator componentType, Identificator container, Position pos) {

			ComponentTypeNameId = componentType;
			ContainerNameId = container;

			Position = pos;
		}


		public Position Position { get; private set; }

	}
}
