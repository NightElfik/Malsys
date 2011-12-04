using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests {
	public static class AssertEx {
		/// <summary>
		/// Verifies if the specified exception type is thrown.
		/// </summary>
		/// <typeparam name="TException">The type of the expected exception.</typeparam>
		/// <param name="action">The action.</param>
		public static void Throws<TException>(Action action) where TException : Exception {
			try {
				action();
				Assert.Fail("No expected '{0}' exception thrown.", typeof(TException));
			}
			catch (TException) {
				// expected exception
			}
		}
	}
}
