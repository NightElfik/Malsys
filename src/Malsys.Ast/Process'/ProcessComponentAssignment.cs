﻿
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
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