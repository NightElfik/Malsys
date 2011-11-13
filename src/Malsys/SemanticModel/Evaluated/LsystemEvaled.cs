using Malsys.SemanticModel.Compiled;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.FunctionEvaledParams>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;

namespace Malsys.SemanticModel.Evaluated {
	public class LsystemEvaled {

		/// <summary>
		/// Contains global vars from evaluation + parameters + local vars.
		/// </summary>
		public readonly VarMap Variables;

		/// <summary>
		/// Contains global functions from evaluation + local functions.
		/// </summary>
		public readonly FunMap Functions;

		public readonly ImmutableList<RewriteRule> RewriteRules;

		public readonly Ast.Binding AstNode;


		public LsystemEvaled(VarMap vars, FunMap funs, ImmutableList<RewriteRule> rRules, Ast.Binding astNode) {

			Variables = vars;
			Functions = funs;
			RewriteRules = rRules;
			AstNode = astNode;
		}
	}
}
