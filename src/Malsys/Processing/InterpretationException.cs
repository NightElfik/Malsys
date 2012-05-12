/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using System.Runtime.Serialization;

namespace Malsys.Processing {

	[Serializable]
	public class InterpretationException : ProcessException {

		public InterpretationException() { }
		public InterpretationException(string message) : base(message) { }
		public InterpretationException(string message, Exception inner) : base(message, inner) { }

		protected InterpretationException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }

	}

}
