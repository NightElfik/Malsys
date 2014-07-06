// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.Ast {
	public class ProcessComponentAssignment : IAstNode {

		public Identifier ComponentTypeNameId;
		public Identifier ContainerNameId;

		public PositionRange Position { get; private set; }


		public ProcessComponentAssignment(Identifier componentType, Identifier container, PositionRange pos) {

			ComponentTypeNameId = componentType;
			ContainerNameId = container;

			Position = pos;
		}


	}
}
