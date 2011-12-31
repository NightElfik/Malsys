using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Malsys.Processing {

	[Serializable]
	public class InterpretationException : ApplicationException {
		public InterpretationException() { }
		public InterpretationException(string message) : base(message) { }
		public InterpretationException(string message, Exception inner) : base(message, inner) { }
		protected InterpretationException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
	}

}
