using Malsys.Expressions;

namespace Malsys {
	public class InputBlock {

		public readonly ImmutableList<LsystemDefinition> Lsystems;
		public readonly ImmutableList<VariableDefinition<IExpression>> Variables;
		public readonly ImmutableList<FunctionDefinition> Functions;

		public InputBlock(ImmutableList<LsystemDefinition> lsyss, ImmutableList<VariableDefinition<IExpression>> vars, ImmutableList<FunctionDefinition> funs) {
			Lsystems = lsyss;
			Variables = vars;
			Functions = funs;
		}
	}
}
