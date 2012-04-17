using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Malsys.Evaluators;
using Malsys.IO;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Compiled.Expressions;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.SourceCode.Printers {
	public class CanonicPrinter {


		private IndentWriter writer;


		public CanonicPrinter(IndentWriter writer) {
			this.writer = writer;
		}


		public void Write(string str) {
			writer.Write(str);
		}

		public void WriteLine(string str) {
			writer.WriteLine(str);
		}

		public void NewLine() {
			writer.NewLine();
		}

		public void Indent() {
			writer.Indent();
		}

		public void Unindent() {
			writer.Unindent();
		}


		public void Print(Ast.Keyword kw, bool includeSpace = true) {
			writer.Write(EnumHelper.GetStringVal(kw));
			if (includeSpace) {
				writer.Write(" ");
			}
		}

		public void PrintSeparated<T>(IEnumerable<T> values, Action<T> printFunc, string separator = ", ") {

			bool first = true;

			foreach (var val in values) {
				if (first) {
					first = false;
				}
				else {
					writer.Write(separator);
				}

				printFunc(val);
			}
		}

		public void Print(IEnumerable<IValue> values) {
			PrintSeparated(values, s => Print(s));
		}

		public void Print(InputBlockEvaled evaledInput) {

			var variables = evaledInput.ExpressionEvaluatorContext.GetAllStoredVariables()
				.Where(x => x.Metadata != null & x.Metadata is Ast.ConstantDefinition)
				.OrderBy(x => x.Name);

			foreach (var var in variables) {
				Print(Ast.Keyword.Let);
				writer.Write(var.Name);
				writer.Write(" = ");
				Print(var.ValueFunc());
				writer.WriteLine(";");
			}

			var functions = evaledInput.ExpressionEvaluatorContext.GetAllStoredFunctions()
				.Where(x => x.Metadata != null & x.Metadata is FunctionData)
				.Select(x => (FunctionData)x.Metadata)
				.GroupBy(x => x)
				.Select(x => x.Key)
				.OrderBy(x => x.Name);

			foreach (var fun in functions) {
				Print(fun);
			}

			foreach (var l in evaledInput.Lsystems.Select(kvp => kvp.Value).OrderBy(l => l.Name)) {
				Print(l);
			}

			foreach (var c in evaledInput.ProcessConfigurations.Select(kvp => kvp.Value).OrderBy(c => c.Name)) {
				Print(c);
			}

			foreach (var p in evaledInput.ProcessStatements.OrderBy(p => p.TargetLsystemName)) {
				Print(p);
			}

		}


		public void Print(ImmutableList<Symbol<IValue>> symbols) {

			PrintSeparated(symbols, s => Print(s), " ");

		}

		public void Print(Symbol<VoidStruct> symbol) {
			writer.Write(symbol.Name);
		}

		public void Print(Symbol<string> symbol) {
			writer.Write(symbol.Name);

			if (!symbol.Arguments.IsEmpty) {
				writer.Write("(");
				writer.Write(string.Join(", ", symbol.Arguments));
				writer.Write(")");
			}
		}

		public void Print(Symbol<IExpression> symbol) {
			writer.Write(symbol.Name);

			if (!symbol.Arguments.IsEmpty) {
				writer.Write("(");
				PrintSeparated(symbol.Arguments, s => Print(s));
				writer.Write(")");
			}
		}

		public void Print(Symbol<IValue> symbol) {
			writer.Write(symbol.Name);

			if (!symbol.Arguments.IsEmpty) {
				writer.Write("(");
				PrintSeparated(symbol.Arguments, s => Print(s));
				writer.Write(")");
			}
		}


		public void Print(FunctionData funData) {

			Print(Ast.Keyword.Fun);
			writer.Write(funData.Name);
			Print(funData.Parameters, true);
			writer.WriteLine(" {");
			writer.Indent();

			Print(funData.Statements);

			writer.Unindent();
			writer.WriteLine("}");
		}


		public void Print(ImmutableList<OptionalParameterEvaled> optParamsEvaled, bool forceParens = false) {

			if (forceParens || !optParamsEvaled.IsEmpty) {
				writer.Write("(");
				PrintSeparated(optParamsEvaled, op => Print(op));
				writer.Write(")");
			}

		}

		public void Print(OptionalParameterEvaled optParamEvaled) {

			writer.Write(optParamEvaled.Name);

			if (optParamEvaled.IsOptional) {
				writer.Write(" = ");
				Print(optParamEvaled.DefaultValue);
			}
		}

		public void Print(ImmutableList<OptionalParameter> optParams) {

			writer.Write("(");
			PrintSeparated(optParams, op => Print(op));
			writer.Write(")");

		}

		public void Print(OptionalParameter optParam) {

			writer.Write(optParam.Name);

			if (optParam.IsOptional) {
				writer.Write(" = ");
				Print(optParam.DefaultValue);
			}
		}


		public void Print(IValue value) {
			if (value.IsConstant) {
				Print((Constant)value);
			}
			else {
				Print((ValuesArray)value);
			}
		}

		public void Print(ValuesArray arr) {
			writer.Write("{");
			PrintSeparated(arr, v => Print(v));
			writer.Write("}");
		}


		#region L-system statements

		public void Print(LsystemEvaledParams lsysEvaledParams) {

			if (lsysEvaledParams.IsAbstract) {
				Print(Ast.Keyword.Abstract);
			}

			Print(Ast.Keyword.Lsystem);
			writer.Write(lsysEvaledParams.Name);
			Print(lsysEvaledParams.Parameters, false);
			writer.Write(" ");

			if (!lsysEvaledParams.BaseLsystems.IsEmpty) {
				Print(Ast.Keyword.Extends);
				PrintSeparated(lsysEvaledParams.BaseLsystems, x => Print(x));
				writer.Write(" ");
			}

			writer.WriteLine("{");
			writer.Indent();

			foreach (var stat in lsysEvaledParams.Statements) {
				Print(stat);
				writer.NewLine();
			}

			writer.Unindent();
			writer.WriteLine("}");
		}

		public void Print(ILsystemStatement stat, bool noDelimiter = false) {

			switch (stat.StatementType) {

				case LsystemStatementType.Constant:
					Print((ConstantDefinition)stat);
					break;
				case LsystemStatementType.SymbolsConstant:
					Print((SymbolsConstDefinition)stat);
					break;
				case LsystemStatementType.SymbolsInterpretation:
					Print((SymbolsInterpretation)stat);
					break;
				case LsystemStatementType.Function:
					Print((Function)stat);
					return;  // never print delimiter
				case LsystemStatementType.RewriteRule:
					Print((RewriteRule)stat);
					break;

				default:
					Debug.Fail("Unknown L-system statement type `{0}`.".Fmt(stat.StatementType.ToString()));
					return;  // never print delimiter

			}

			if (!noDelimiter) {
				writer.Write(";");
			}
		}

		public void Print(BaseLsystem baseLsys) {
			writer.Write(baseLsys.Name);
			if (!baseLsys.Arguments.IsEmpty) {
				writer.Write("(");
				PrintSeparated(baseLsys.Arguments, x => Print(x));
				writer.Write(")");
			}
		}

		public void Print(ConstantDefinition constDef) {
			if (constDef.IsComponentAssign) {
				Print(Ast.Keyword.Set);
			}
			else {
				Print(Ast.Keyword.Let);
			}
			writer.Write(constDef.Name);
			writer.Write(" = ");
			Print(constDef.Value);
		}

		public void Print(Function fun) {

			Print(Ast.Keyword.Fun);
			writer.Write(fun.Name);
			Print(fun.Parameters);
			writer.WriteLine(" {");
			writer.Indent();

			Print(fun.Statements);

			writer.Unindent();
			writer.Write("}");
		}

		public void Print(SymbolsConstDefinition symbolsDef) {
			Print(Ast.Keyword.Set);
			Print(Ast.Keyword.Symbols);
			writer.Write(symbolsDef.Name);
			writer.Write(" = ");
			PrintSeparated(symbolsDef.Symbols, s => Print(s), " ");
		}

		public void Print(RewriteRule rewriteRule) {
			Print(Ast.Keyword.Rewrite);

			if (!rewriteRule.LeftContext.IsEmpty) {
				writer.Write("{");
				PrintSeparated(rewriteRule.LeftContext, s => Print(s), " ");
				writer.Write("} ");
			}

			Print(rewriteRule.SymbolPattern);

			if (!rewriteRule.RightContext.IsEmpty) {
				writer.Write(" {");
				PrintSeparated(rewriteRule.RightContext, s => Print(s), " ");
				writer.Write("}");
			}

			writer.Indent();

			if (!rewriteRule.LocalConstantDefs.IsEmpty) {
				writer.NewLine();
				Print(Ast.Keyword.With);
				PrintSeparated(rewriteRule.LocalConstantDefs,
					cd => {
						writer.Write(cd.Name);
						writer.Write(" = ");
						Print(cd.Value);
					});
			}

			// do not print empty wheres or wheres with condition expressions equal to constant true
			if (rewriteRule.Condition.ExpressionType != ExpressionType.EmptyExpression && !isTrue(rewriteRule.Condition)) {
				writer.NewLine();
				Print(Ast.Keyword.Where);
				Print(rewriteRule.Condition);
			}


			bool first = true;
			foreach (var replac in rewriteRule.Replacements) {
				writer.NewLine();
				if (first) {
					first = false;
				}
				else {
					Print(Ast.Keyword.Or);
				}

				Print(replac);
			}

			writer.Unindent();
		}

		public void Print(RewriteRuleReplacement rrReplacment) {

			Print(Ast.Keyword.To);

			if (rrReplacment.Replacement.IsEmpty) {
				Print(Ast.Keyword.Nothing, false);
			}
			else {
				PrintSeparated(rrReplacment.Replacement, s => Print(s), " ");
			}

			if (rrReplacment.Weight.ExpressionType != ExpressionType.EmptyExpression && !isTrue(rrReplacment.Weight)) {
				writer.Write(" ");
				Print(Ast.Keyword.Weight);
				Print(rrReplacment.Weight);
			}
		}

		public void Print(SymbolsInterpretation symInt) {

			Print(Ast.Keyword.Interpret);
			PrintSeparated(symInt.Symbols, s => Print(s), " ");

			if (!symInt.Parameters.IsEmpty) {
				writer.Write("(");
				PrintSeparated(symInt.Parameters, e => Print(e));
				writer.Write(")");
			}

			writer.Write(" ");
			Print(Ast.Keyword.As);
			if (symInt.InstructionIsLsystemName) {
				Print(Ast.Keyword.Lsystem);
			}

			writer.Write(symInt.InstructionName);

			if (!symInt.InstructionParameters.IsEmpty) {
				writer.Write("(");
				PrintSeparated(symInt.InstructionParameters, e => Print(e));
				writer.Write(")");
			}
		}


		#endregion


		#region Expression members

		public void Print(IExpression expr) {

			switch (expr.ExpressionType) {
				case ExpressionType.BinaryOperator: Print((BinaryOperator)expr); break;
				case ExpressionType.Constant: Print((Constant)expr); break;
				case ExpressionType.EmptyExpression: Print((EmptyExpression)expr); break;
				case ExpressionType.ExpressionValuesArray: Print((ExpressionValuesArray)expr); break;
				case ExpressionType.ExprVariable: Print((ExprVariable)expr); break;
				case ExpressionType.FunctionCall: Print((FunctionCall)expr); break;
				case ExpressionType.Indexer: Print((Indexer)expr); break;
				case ExpressionType.UnaryOperator: Print((UnaryOperator)expr); break;
				default: Debug.Fail("Unknown expression member type `{0}`.".Fmt(expr.ExpressionType.ToString())); break;
			}

		}

		public void Print(BinaryOperator binaryOperator) {
			writer.Write("(");
			Print(binaryOperator.LeftOperand);
			writer.Write(" ");
			writer.Write(binaryOperator.Syntax);
			writer.Write(" ");
			Print(binaryOperator.RightOperand);
			writer.Write(")");
		}

		public void Print(Constant constant) {

			Ast.ConstantFormat fmt = Ast.ConstantFormat.Float;
			if (constant.AstNode != null) {
				fmt = constant.AstNode.Format;
			}

			switch (fmt) {

				case Ast.ConstantFormat.Binary:
					long valBin = (long)Math.Round(constant.Value);
					writer.Write("0b");
					writer.Write(Convert.ToString(valBin, 2));
					break;

				case Ast.ConstantFormat.Octal:
					long valOct = (long)Math.Round(constant.Value);
					writer.Write("0o");
					writer.Write(Convert.ToString(valOct, 8));
					break;

				case Ast.ConstantFormat.Hexadecimal:
					long valHex = (long)Math.Round(constant.Value);
					writer.Write("0x");
					writer.Write(Convert.ToString(valHex, 16).ToUpper());
					break;

				case Ast.ConstantFormat.HashHexadecimal:
					long valHash = (long)Math.Round(constant.Value);
					writer.Write("#");
					writer.Write(Convert.ToString(valHash, 16).ToUpper());
					break;

				default:
					writer.Write(constant.Value.ToStringInvariant());
					break;

			}
		}

		public void Print(EmptyExpression emptyExpression) { }

		public void Print(ExpressionValuesArray expressionValuesArray) {
			writer.Write("{");
			PrintSeparated(expressionValuesArray, e => Print(e));
			writer.Write("}");
		}

		public void Print(ExprVariable variable) {
			writer.Write(variable.Name);
		}

		public void Print(FunctionCall functionCall) {
			writer.Write(functionCall.Name);
			writer.Write("(");
			PrintSeparated(functionCall.Arguments, e => Print(e));
			writer.Write(")");
		}

		public void Print(Indexer indexer) {
			Print(indexer.Array);
			writer.Write("[");
			Print(indexer.Index);
			writer.Write("]");
		}

		public void Print(UnaryOperator unaryOperator) {
			writer.Write("(");
			writer.Write(unaryOperator.Syntax);
			Print(unaryOperator.Operand);
			writer.Write(")");
		}

		#endregion


		#region Function statements

		public void Print(IEnumerable<IFunctionStatement> funStats) {
			foreach (var stat in funStats) {
				Print(stat);
				writer.NewLine();
			}
		}

		public void Print(IFunctionStatement stat) {

			switch (stat.StatementType) {
				case FunctionStatementType.ConstantDefinition:
					Print((ConstantDefinition)stat);
					break;
				case FunctionStatementType.ReturnExpression:
					Print(Ast.Keyword.Return);
					Print(((FunctionReturnExpr)stat).ReturnValue);
					break;
				default: Debug.Fail("Unknown function statement type `{0}`.".Fmt(stat.StatementType.ToString())); break;
			}
			writer.Write(";");
		}

		#endregion


		#region Process configuration statements

		public void Print(ProcessConfigurationStatement processConf) {

			Print(Ast.Keyword.Configuration);
			writer.Write(processConf.Name);

			writer.Write(" {");
			writer.Indent();

			foreach (var comp in processConf.Components.OrderBy(c => c.Name)) {
				writer.NewLine();
				Print(comp);
			}

			foreach (var cont in processConf.Containers.OrderBy(c => c.Name)) {
				writer.NewLine();
				Print(cont);
			}

			foreach (var conn in processConf.Connections.OrderBy(c => c.SourceName)) {
				writer.NewLine();
				Print(conn);
			}

			writer.Unindent();
			writer.NewLine();
			writer.WriteLine("}");
		}

		public void Print(ProcessStatementEvaled procStat) {

			Print(Ast.Keyword.Process);
			if (string.IsNullOrEmpty(procStat.TargetLsystemName)) {
				Print(Ast.Keyword.All, false);
			}
			else {
				writer.Write(procStat.TargetLsystemName);
			}

			if (!procStat.Arguments.IsEmpty) {
				writer.Write("(");
				PrintSeparated(procStat.Arguments, x => Print(x));
				writer.Write(")");
			}

			writer.Write(" ");

			Print(Ast.Keyword.With);
			writer.Write(procStat.ProcessConfiName);

			writer.Indent();
			foreach (var assign in procStat.ComponentAssignments) {
				writer.NewLine();
				Print(assign);
			}
			foreach (var lsysStat in procStat.AdditionalLsystemStatements) {
				writer.NewLine();
				Print(lsysStat, true);
			}
			writer.Unindent();

			writer.WriteLine(";");
		}

		public void Print(ProcessComponentAssignment assign) {
			Print(Ast.Keyword.Use);
			writer.Write(assign.ComponentTypeName);
			writer.Write(" ");
			Print(Ast.Keyword.As);
			writer.Write(assign.ContainerName);
		}

		public void Print(ProcessComponent component) {
			Print(Ast.Keyword.Component);
			writer.Write(component.Name);
			writer.Write(" ");
			Print(Ast.Keyword.Typeof);
			writer.Write(component.TypeName);
			writer.Write(";");
		}

		public void Print(ProcessContainer container) {
			Print(Ast.Keyword.Container);
			writer.Write(container.Name);
			writer.Write(" ");
			Print(Ast.Keyword.Typeof);
			writer.Write(container.TypeName);
			writer.Write(" ");
			Print(Ast.Keyword.Default);
			writer.Write(container.DefaultTypeName);
			writer.Write(";");
		}

		public void Print(ProcessComponentsConnection connection) {
			if (connection.IsVirtual) {
				Print(Ast.Keyword.Virtual);
			}
			Print(Ast.Keyword.Connect);
			writer.Write(connection.SourceName);
			writer.Write(" ");
			Print(Ast.Keyword.To);
			writer.Write(connection.TargetName);
			writer.Write(".");
			writer.Write(connection.TargetInputName);
			writer.Write(";");
		}

		#endregion


		private bool isTrue(IExpression expr){
			return expr.ExpressionType == ExpressionType.Constant && ((Constant)expr).Value.EpsilonCompareTo(1) == 0;
		}

	}
}