using Malsys.Processing;
using Malsys.Processing.Components.Interpreters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Interpreters {
	[TestClass]
	public class InterpreterCallerTests {

		[TestMethod]
		public void EmptyInputTests() {
			GenericInterpreterCallerTests.EmptyInputTests(new InterpreterCaller());
		}

		[TestMethod]
		public void ExistingActionsTests() {
			GenericInterpreterCallerTests.ExistingActionsTests(new InterpreterCaller());
		}

		[TestMethod]
		[ExpectedException(typeof(InterpretationException))]
		public void UnknownSymbolsTests() {
			GenericInterpreterCallerTests.UnknownActionsTests(new InterpreterCaller());
		}

		[TestMethod]
		public void UnknownActionsTests() {
			GenericInterpreterCallerTests.UnknownSymbolsTests(new InterpreterCaller());
		}

		[TestMethod]
		public void ParametersFromSymbolTests() {
			GenericInterpreterCallerTests.ParametersFromSymbolTests(new InterpreterCaller());
		}

		[TestMethod]
		public void ParametersTests() {
			GenericInterpreterCallerTests.ParametersTests(new InterpreterCaller());
		}

		[TestMethod]
		public void OptionalParametersTests() {
			GenericInterpreterCallerTests.OptionalParametersTests(new InterpreterCaller());
		}

		[TestMethod]
		public void NativelyOptionalParametersTests() {
			GenericInterpreterCallerTests.NativelyOptionalParametersTests(new InterpreterCaller());
		}
	}
}
