using System;
using System.Runtime.Serialization;

namespace Malsys.Processing {

	[Serializable]
	public class ProcessException : ApplicationException {

		public ProcessException() { }
		public ProcessException(string message) : base(message) { }
		public ProcessException(string message, Exception inner) : base(message, inner) { }

		protected ProcessException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }

	}

}
