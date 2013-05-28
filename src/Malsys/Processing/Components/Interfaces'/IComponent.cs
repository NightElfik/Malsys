// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.Runtime.Serialization;

namespace Malsys.Processing.Components {
	/// <summary>
	///	General component interface. All components must implement this interface.
	/// </summary>
	/// <name>Component interface</name>
	/// <group>Common</group>
	/// <remarks>
	/// This interface marks a class as component.
	/// Component must also have a parameter-less constructor.
	///
	/// Every component must be reusable and individual usages must be independent. The Reset method is used for
	/// resetting component to its default (initial) state.
	///
	/// Before start of processing of the L-system Initialize method is called giving the component ProcessContext.
	/// During processing, the component should use only resources from given ProcessContext or in memory.
	/// Especially it mustn't create temporary files on its own.
	/// For output and temporary files should be used the IOutputProvider from the ProcessContext.
	/// If the component needs to use some other resources than they should be properly freed in Reset and Cleanup
	/// methods. Cleanup method is called even if the processing failed to allow.
	///
	/// Public properties marked with UserGettableAttribute can be read after initialization.
	///	If IsGettableBeforeInitialiation property of attribute is true they can be read even before initialization but
	///	after cleanup.
	///
	/// Public properties marked with UserSettableAttribute (UserSettableSybolsAttribute) can be set after initialization.
	/// Remember that component can be reused and individual usages must be independent so set values should not
	/// preserve the reset of the component (Reset method should reset all the settable properties to their default values).
	///
	/// Value of public properties marked with UserConnectableAttribute will be set after reset before initialization.
	/// If connected component is not valid InvalidConnectedComponentException should be thrown.
	///
	/// Public methods marked with UserCallableFunctionAttribute can be called after initialization.
	///	If IsCallableBeforeInitialiation property of attribute is true they can be called even before initialization but
	///	after cleanup.
	///
	/// Public methods marked with SymbolInterpretationAttribute are symbol interpretation methods which will be called
	/// to interpret symbol (if symbol interpretation is defined correctly). Note that concrete behavior of calling of
	/// the interpretation methods depends on used components.
	///
	/// All properties (methods) marked with any attribute described above can also be marked with AccessNameAttribute
	/// which can change access name(s) of property (method). AccessNameAttribute can be configured to change name of
	/// marked member or to preserve member's original name (it can only add aliases).
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
		/// Logger for logging any messages (especially warnings) during initialization or processing of the component.
		/// Error state of the logger is not checked during processing so for fatal errors ComponentException should be
		/// thrown. However logging errors over warnings could have significant impact for processing of results.
		/// For example in Web user interface if error occurred during processing outputs are not shown to the user
		/// and L-system can not be saved to the DB by user.
		/// </summary>
		/// <remarks>
		/// The Logger property is set after resetting of the component which comes immediately after its instantiating.
		/// However, the Logger property can be set to new value so it should not be cached, component should use
		/// always current instance. Changing of the logger is done when component is reused. Individual processes will
		/// surely use different instance of the logger.
		///
		/// Logger property is automatically set to null before on reset thus, setting null value must not throw any
		/// exception.
		/// </remarks>
		IMessageLogger Logger { set; }

		/// <summary>
		/// Reset method is called once before each usage of the component for processing an L-system.
		/// If more L-systems are processed with this component, Reset method is called before each of process.
		/// </summary>
		/// <remarks>
		/// It should be used for setting component (its members and properties) to default state (value).
		/// This method should also reset all connectable properties.
		/// This is important because ProcessManager can not reset settable or connectable properties because supplying
		/// null is not acceptable (also null value will probably cause an exception).
		///
		/// ComponentException can be thrown when component can not be resetted correctly.
		/// Message from this exception will be shown to the user as an error message.
		/// Other exceptions are also caught but only name of thrown exception is shown to user.
		/// </remarks>
		void Reset();

		/// <summary>
		/// Initializes the component. This method is called once before processing an L-system (not before each iteration).
		/// </summary>
		/// <remarks>
		/// This method can be used for validating settable properties or parsing their values.
		///
		/// ComponentException can be thrown when component can not be initialized correctly.
		/// Message from this exception will be shown to user in error message.
		///
		/// In this method can be called user defined gettable variables and callable functions on other components.
		/// However these calls could occur on non-initialized components so caller have to be careful.
		/// </remarks>
		void Initialize(ProcessContext context);

		/// <summary>
		/// At the end of processing of the L-system is called Cleanup method to allow finishing of the processing
		/// and freeing any resources. This method is called regardless of process result (success/fail).
		/// </summary>
		/// <remarks>
		/// Cleanup method is not used for setting the component to initial state, this method is intended only for
		/// finalizing processing and freeing resources. The Reset method will be called before another use of this
		/// component to set it to the initial state.
		///
		/// ComponentException can be thrown when component can not be cleaned up correctly.
		/// Message from this exception will be shown to the user as an error message.
		/// Other exceptions are also caught but only name of thrown exception is shown to user.
		///
		/// Cleanup method may be called at 'any' time (even when component is not initialized properly). This will
		/// occur when unhandled exception is thrown when component graph created. Reset method do not need to
		/// do the same things as Cleanup method since Cleanup method will be called always after component usage.
		/// </remarks>
		void Cleanup();

		/// <summary>
		/// Dispose method is called at the very end of processing. After call of dispose method the component will
		/// never be used.
		/// </summary>
		/// <remarks>
		/// Dispose method is intended for freeing unmanaged resources that can not be freed in Cleanup method (for
		/// for example one instance of resource for any number of processed L-systems).
		/// There is no need to set managed members to initial values or to null since GC will do the stuff.
		///
		/// Dispose method should not explicitly throw any exceptions. Unhandled are caught to allow of disposing all
		/// components.
		/// </remarks>
		void Dispose();

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
