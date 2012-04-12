using System;
using System.Runtime.Serialization;

namespace Malsys.Processing.Components {
	/// <summary>
	///	General component interface. All components must implement this interface.
	/// </summary>
	/// <name>Component interface</name>
	/// <group>Common</group>
	/// <remarks>
	/// This interface marks class as component.
	/// </remarks>
	public interface IComponent {

		/// <remarks>
		/// ComponentException can be thrown when component can not be initialized correctly.
		/// Message from this exception will be shown to user in error message.
		/// Other exceptions are also caught but only name of thrown exception is shown to user.
		///
		/// In this method can be called user defined gettable variables and callable functions on other components.
		/// However these calls could occur on non-initialized components so caller have to be careful.
		/// </remarks>
		void Initialize(ProcessContext context);

		/// <remarks>
		/// ComponentException can be thrown when component can not be cleaned up correctly.
		/// Message from this exception will be shown to user in error message.
		/// Other exceptions are also caught but only name of thrown exception is shown to user.
		/// </remarks>
		void Cleanup();

	}



	[Serializable]
	public class ComponentException : Exception {

		public ComponentException() { }
		public ComponentException(string message) : base(message) { }
		public ComponentException(string message, Exception inner) : base(message, inner) { }

		protected ComponentException(SerializationInfo info, StreamingContext context) : base(info, context) { }

	}

}
