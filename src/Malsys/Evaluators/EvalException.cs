/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using System.Runtime.Serialization;

namespace Malsys.Evaluators {
	/// <summary>
	/// Evaluation exception is thrown by some evaluators who do not use IMessageLogger to log errors.
	/// </summary>
	[Serializable]
	public class EvalException : Exception {

		public EvalException() { }
		public EvalException(string message) : base(message) { }
		public EvalException(string message, Exception inner) : base(message, inner) { }


		protected EvalException(SerializationInfo info, StreamingContext context) : base(info, context) { }

		/// <summary>
		/// Returns full list of EvalException messages (if there are EvalException as inner exceptions).
		/// </summary>
		public string GetFullMessage() {
			if (InnerException != null) {
				if (InnerException is EvalException) {
					return Message + " " + ((EvalException)InnerException).GetFullMessage();
				}
				else {
					return Message + " " + InnerException.Message;
				}
			}
			else {
				return Message;
			}
		}

	}
}
