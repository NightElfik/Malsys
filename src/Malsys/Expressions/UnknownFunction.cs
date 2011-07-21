using System;

namespace Malsys.Expressions {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class UnknownFunction : IPostfixExpressionMember {
		/// <summary>
		/// Arguments indexer.
		/// </summary>
		public IExpressionValue this[int i] { get { return arguments[i]; } }

		public readonly string Name;
		public readonly byte Arity;

		private IExpressionValue[] arguments;

		public UnknownFunction(string name, byte arity, IExpressionValue[] arguments) {
			Name = name;
			Arity = arity;
			this.arguments = arguments;
		}

		public override string ToString() {
			return Name + "@" + Arity;
		}

		#region IPostfixExpressionMember Members

		public bool IsConstant { get { return false; } }
		public bool IsArray { get { return false; } }
		public bool IsVariable { get { return false; } }
		public bool IsEvaluable { get { return false; } }
		public bool IsUnknownFunction { get { return true; } }

		#endregion
	}
}
