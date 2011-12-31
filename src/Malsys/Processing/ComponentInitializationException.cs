using System;
using System.Runtime.Serialization;

namespace Malsys.Processing {
	[Serializable]
	public class ComponentInitializationException : Exception {
		public ComponentInitializationException() { }
		public ComponentInitializationException(string message) : base(message) { }
		public ComponentInitializationException(string message, Exception inner) : base(message, inner) { }
		protected ComponentInitializationException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
	}
}
