using Malsys.Expressions;

namespace Malsys {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class FunctionDefinition : RichExpression {

		public readonly string Name;
		public readonly ImmutableList<OptionalParameter> Parameters;


		public FunctionDefinition(string name, ImmutableList<OptionalParameter> prms, ImmutableList<VariableDefinition<IExpression>> varDefs, IExpression expr)
			: base(varDefs, expr) {

			Name = name;
			Parameters = prms;
		}
	}
}
