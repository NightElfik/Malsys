using System;

namespace Malsys.SemanticModel.Compiled {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class Binding : IInputStatement, ILsystemStatement {

		public readonly string Name;

		public readonly IBindable Value;

		public readonly BindingType BindingType;


		public Binding(string name, IBindable value, BindingType bindType) {

			Name = name;
			Value = value;
			BindingType = bindType;
		}

		#region IInputStatement Members

		public InputStatementType StatementType { get { return InputStatementType.Binding; } }

		#endregion

		#region ILsystemStatement Members

		public LsystemStatementType StatementType { get { return LsystemStatementType.Binding; } }

		#endregion
	}

	[Flags]
	public enum BindingType {
		Expression = 0x01,
		Function = 0x02,
		SymbolList = 0x04,
		Lsystem = 0x08,

		ExpressionsAndFunctions = Expression | Function,
		All = Expression | Function | SymbolList | Lsystem
	}
}
