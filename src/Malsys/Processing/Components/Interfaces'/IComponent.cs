using System;
using System.Runtime.Serialization;

namespace Malsys.Processing.Components {

	[Component("Generic component", ComponentGroupNames.Common)]
	public interface IComponent {

		/// <remarks>
		/// ComponentInitializationException can be thrown when component can not be initialized.
		/// Message from this exception will be shown to user in error message.
		/// Other exceptions are also caught but only name is shown to user.
		/// </remarks>
		void Initialize(ProcessContext ctxt);

		void Cleanup();

	}



	[Serializable]
	public class ComponentInitializationException : Exception {

		public ComponentInitializationException() { }
		public ComponentInitializationException(string message) : base(message) { }
		public ComponentInitializationException(string message, Exception inner) : base(message, inner) { }

		protected ComponentInitializationException(SerializationInfo info, StreamingContext context) : base(info, context) { }

	}

}
