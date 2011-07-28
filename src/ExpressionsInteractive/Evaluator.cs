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

		private VarMap variables = MapModule.Empty<string, IValue>();
		private FunMap functions = MapModule.Empty<string, FunctionDefinition>();


		public string EvaluateStr(string input) {

			Malsys.Ast.IExprInteractiveStatement[] parsedStmnts;

			try {
				var lexBuff = LexBuffer<char>.FromString(input);
				parsedStmnts = ParserUtils.parseExprInteractiveStatements(lexBuff, "input");
			}
			catch (LexerException ex) {
				return "Failed to lex input. {0}".Fmt(ex.Message);
			}
			catch (ParserException<char> ex) {
				return "Failed to parse input. {0}".Fmt(ex.Message);
			}

			StringBuilder sb = new StringBuilder();

			foreach (var stmnt in parsedStmnts) {
				var cp = new CompilerParametersInternal(new CompilerParameters());

				if (stmnt is Malsys.Ast.Expression) {
					IExpression expr;
					if (ExpressionCompiler.TryCompile((Malsys.Ast.Expression)stmnt, cp, out expr)) {
						sb.AppendLine(evaluateExpression(expr));
					}
				}
				else if (stmnt is Malsys.Ast.VariableDefinition) {
					VariableDefinition varDef;
					if (VariableDefinitionCompiler.TryCompile((Malsys.Ast.VariableDefinition)stmnt, cp, out varDef)) {
						sb.AppendLine(evaluateVarDef(varDef));
					}
				}
				else if (stmnt is Malsys.Ast.FunctionDefinition) {
					FunctionDefinition funDef;
					if (FunctionDefinitionCompiler.TryCompile((Malsys.Ast.FunctionDefinition)stmnt, cp, out funDef)) {
						sb.AppendLine(evaluateFunDef(funDef));
					}
				}
				else {
					Debug.Fail("Unhandled parsed type `{0}`".Fmt(parsedStmnts.GetType().ToString()));
					return "Internal error.";
				}

				foreach (var msg in cp.Messages) {
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

		private string evaluateVarDef(VariableDefinition varDef) {
			try {
				variables = VariableDefinitionEvaluator.EvaluateAndAdd(varDef, variables, functions);
				return "{0} = {1}".Fmt(varDef.Name, MapModule.Find(varDef.Name, variables).ToString());
			}
			catch (EvalException ex) {
				return ex.Message;
			}
		}

		private string evaluateFunDef(FunctionDefinition funDef) {
			//functions = MapModule.Add(funDef.Name, funDef, functions);
			return "{0} defined".Fmt(funDef.Name);
		}
	}
}
