using System.Diagnostics;
using System.Text;
using Malsys;
using Malsys.Compilers;
using Malsys.Expressions;
using Malsys.Parsing;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Text.Lexing;
using FunMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.FunctionDefinition>;
using VarMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.Expressions.IValue>;

namespace ExpressionsInteractive {
	public class Evaluator {

		public VarMap DefinedVariables { get { return variables; } }
		public FunMap DefinedFunctions { get { return functions; } }

		private VarMap variables = MapModule.Empty<string, IValue>();
		private FunMap functions = MapModule.Empty<string, FunctionDefinition>();


		public string EvaluateStr(string input, string sourceName) {

			var sb = new StringBuilder();
			Malsys.Ast.IExprInteractiveStatement[] parsedStmnts;

			{
				var msgs = new MessagesCollection();
				var lexBuff = LexBuffer<char>.FromString(input);
				parsedStmnts = ParserUtils.ParseExprInteractiveStatements(lexBuff, msgs, sourceName);


				if (msgs.ErrorOcured) {
					foreach (var msg in msgs) {
						sb.AppendLine(msg.GetFullMessage());
					}

					return sb.ToString();
				}
			}



			foreach (var statement in parsedStmnts) {
				var msgs = new MessagesCollection();
				msgs.DefaultSourceName = sourceName;
				var compiler = new InputCompiler(msgs);

				if (statement is Malsys.Ast.Expression) {
					var expr = compiler.ExpressionCompiler.CompileExpression((Malsys.Ast.Expression)statement);
					sb.AppendLine(evaluateExpression(expr));
				}

				else if (statement is Malsys.Ast.VariableDefinition) {
					var varDef = compiler.CompileFailSafe((Malsys.Ast.VariableDefinition)statement);
					if (!msgs.ErrorOcured) {
						sb.AppendLine(evaluateVarDef(varDef));
					}
				}

				else if (statement is Malsys.Ast.FunctionDefinition) {
					var funDef = compiler.CompileFailSafe((Malsys.Ast.FunctionDefinition)statement);
					if (!msgs.ErrorOcured) {
						sb.AppendLine(evaluateFunDef(funDef));
					}
				}


				else if (statement is Malsys.Ast.EmptyStatement) {
					continue;
				}

				else {
					Debug.Fail("Unhandled parsed type `{0}`".Fmt(parsedStmnts.GetType().ToString()));
					return "Internal error.";
				}

				foreach (var msg in msgs) {
					sb.AppendLine(msg.GetFullMessage());
				}

			}

			return sb.ToString();
		}

		private string evaluateExpression(IExpression expr) {
			try {
				return ExpressionEvaluator.Evaluate(expr, variables, functions).ToString();
			}
			catch (EvalException ex) {
				return ex.Message;
			}
		}

		private string evaluateVarDef(VariableDefinition<IExpression> varDef) {
			try {
				variables = VariableDefinitionEvaluator.EvaluateAndAdd(varDef, variables, functions);
				return "Variable `{0}` defined to {1}.".Fmt(varDef.Name, MapModule.Find(varDef.Name, variables).ToString());
			}
			catch (EvalException ex) {
				return ex.Message;
			}
		}

		private string evaluateFunDef(FunctionDefinition funDef) {
			functions = MapModule.Add(funDef.Name, funDef, functions);
			return "Function `{0}` defined.".Fmt(funDef.Name);
		}
	}
}
