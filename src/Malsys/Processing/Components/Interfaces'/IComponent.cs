using System;
using System.Runtime.Serialization;

namespace Malsys.Processing.Components {
	/// <summary>
	///	General component interface. All components must implement this interface.
	/// </summary>
	/// <name>Component interface</name>
	/// <group>Common</group>
	/// <remarks>
	/// This interface marks class as component. Component must have parameter-less constructor.
	///
	/// Cleanup method will be called before each usage of component. It can be used for setting component members to
	/// their default values. Cleanup method can be also called at the end of processing to allow freeing any temporary
	/// data but this is not guaranteed. Every component must be reusable and individual usages must be independent.
	///
	/// Before start of processing of L-system Initialize method is called giving component ProcessContext.
	/// During processing component should use only resources from given ProcessContext or in memory.
	/// Especially it mustn't create temporary files on its own. For output and temporary files should be used
	/// IOutputProvider from ProcessContext.
	///
	/// Public properties marked with UserGettableAttribute can be read after initialization.
	///	If IsGettableBeforeInitialiation property of attribute is true they can be read even before initialization but
	///	after cleanup.
	///
	/// Public properties marked with UserSettableAttribute can be set after initialization.
	/// Remember that component can be reused and individual usages must be independent so set values should not
	/// preserve cleanup (Cleanup method should set all settable properties to default values).
	///
	/// Value of public properties marked with UserConnectableAttribute will be set after cleanup before initialization.
	/// If connected component is not valid InvalidConnectedComponentException should be thrown.
	///
	/// Public methods marked with UserCallableFunctionAttribute can be called after initialization.
	///	If IsCallableBeforeInitialiation property of attribute is true they can be called even before initialization but
	///	after cleanup.
	///
	/// Public methods marked with SymbolInterpretationAttribute are symbol interpretation methods which will be called
	/// to interpret symbol (if symbol interpretation is defined appropriately). Note that concrete behavior of
	/// calling of interpretation methods depends on used components.
	///
	/// All properties (methods) marked with any attribute described above can also be marked with AccessNameAttribute
	/// which can change access name(s) to property (method). AccessNameAttribute can be configured to give more
	/// names to marked member or to preserve member's original name (it can only add aliases).
	///
	/// When L-system statements are processed components are not initialized yet.
	/// This means that values of all properties marked with UserSettableAttribute or UserConnectableAttribute
	/// attributes are set before initialization. This also means that if user wants to be able to allow user to read
	/// property (or call function) in L-system statements that are evaluated before evaluation of L-system
	/// (for example in `let` or `set` statements) corresponding attributes must have property
	/// IsGettableBeforeInitialiation (resp. IsCallableBeforeInitialiation) set to true. This is designed to allow
	/// components to 'define' functions and constants. For example random function on RandomGeneratorProvider component
	/// is callable before initialization thus you can use random function in L-system statements.
	/// </remarks>
	public interface IComponent {

		/// <summary>
		/// Logger for logging any messages (especially warnings) during initialization or processing. Error state of
		/// logger is not checked during processing so for fatal errors ComponentException should be thrown.
		/// Value of this property can be changed during processing and component must use always current instance.
		/// </summary>
		IMessageLogger Logger { set; }

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


	/// <summary>
	/// This exception can be thrown by component if any fatal error occurred.
	/// </summary>
	[Serializable]
	public class ComponentException : Exception {

		public ComponentException() { }
		public ComponentException(string message) : base(message) { }
		public ComponentException(string message, Exception inner) : base(message, inner) { }

		protected ComponentException(SerializationInfo info, StreamingContext context) : base(info, context) { }

	}

}
