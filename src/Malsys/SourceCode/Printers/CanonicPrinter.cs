using System;
using System.Linq;
using System.Collections.Generic;
using Malsys.IO;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;
using Malsys.SemanticModel.Compiled.Expressions;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.SourceCode.Printers {
	public class CanonicPrinter : IExpressionVisitor {


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

		public void Print(Ast.Keyword kw, bool includeSpace = true) {
			writer.Write(EnumHelper.GetStringVal(kw));
			if (includeSpace) {
				writer.Write(" ");
			}
		}


		public void Print(Malsys.SemanticModel.Evaluated.InputBlock evaledInput) {

			foreach (var kvp in evaledInput.GlobalConstants.OrderBy(kvp => kvp.Key)) {
				Print(Ast.Keyword.Let);
				writer.Write(kvp.Key);
				writer.Write(" = ");
				Print(kvp.Value);
				writer.WriteLine(";");
			}

			foreach (var f in evaledInput.GlobalFunctions.Select(kvp => kvp.Value).OrderBy(f => f.Name)) {
				Print(f);
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

		public void Print(LsystemEvaledParams lsysEvaledParams) {

			Print(Ast.Keyword.Lsystem);
			writer.Write(lsysEvaledParams.Name);
			Print(lsysEvaledParams.Parameters, false);
			writer.WriteLine(" {");
			writer.Indent();

			foreach (var stat in lsysEvaledParams.Statements) {
				switch (stat.StatementType) {
					case LsystemStatementType.Constant:
						Print((ConstantDefinition)stat);
						break;
					case LsystemStatementType.Function:
						Print((Function)stat);
						break;
					case LsystemStatementType.SymbolsConstant:
						Print((SymbolsConstDefinition)stat);
						break;
					case LsystemStatementType.RewriteRule:
						Print((RewriteRule)stat);
						break;
					case LsystemStatementType.SymbolsInterpretation:
						Print((SymbolsInterpretation)stat);
						break;
					case LsystemStatementType.ProcessStatement:
						Print((ProcessStatement)stat);
						break;
					default:
						break;
				}
			}

			writer.Unindent();
			writer.WriteLine("}");
		}

		public void Print(ProcessConfiguration processConf) {

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

		public void Print(ProcessStatement procStat) {

			Print(Ast.Keyword.Process);
			if (string.IsNullOrEmpty(procStat.TargetLsystemName)) {
				Print(Ast.Keyword.This);
			}
			else {
				writer.Write(procStat.TargetLsystemName);
				writer.Write(" ");
			}
			Print(Ast.Keyword.With);
			writer.Write(procStat.ProcessConfiName);

			writer.Indent();
			foreach (var assign in procStat.ComponentAssignments) {
				writer.NewLine();
				Print(Ast.Keyword.Use);
				writer.Write(assign.ComponentTypeName);
				writer.Write(" ");
				Print(Ast.Keyword.As);
				writer.Write(assign.ContainerName);
			}
			writer.Unindent();

			writer.WriteLine(";");
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

			Print(Ast.Keyword.Connect);
			writer.Write(connection.SourceName);
			writer.Write(" ");
			Print(Ast.Keyword.To);
			writer.Write(connection.TargetName);
			writer.Write(".");
			writer.Write(connection.TargetInputName);
			writer.Write(";");
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

			if (!rewriteRule.Condition.IsEmptyExpression) {
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

			writer.WriteLine(";");
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

			if (!rrReplacment.Weight.IsEmptyExpression) {
				writer.Write(" ");
				Print(Ast.Keyword.Weight);
				Print(rrReplacment.Weight);
			}
		}

		public void Print(SymbolsInterpretation symInt) {

			Print(Ast.Keyword.Interpret);
			PrintSeparated(symInt.Symbols, s => Print(s), " ");

			writer.Write(" ");
			Print(Ast.Keyword.As);

			writer.Write(symInt.InstructionName);

			if (!symInt.InstructionParameters.IsEmpty) {
				writer.Write("(");
				PrintSeparated(symInt.InstructionParameters, e => Print(e));
				writer.Write(")");
			}

			writer.WriteLine(";");
		}

		public void Print(SymbolsConstDefinition symbolsDef) {
			Print(Ast.Keyword.Set);
			writer.Write(symbolsDef.Name);
			writer.Write(" = ");
			PrintSeparated(symbolsDef.Symbols, s => Print(s), " ");
			writer.WriteLine(";");
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

		public void Print(FunctionEvaledParams funEvaledParams) {

			Print(Ast.Keyword.Fun);
			writer.Write(funEvaledParams.Name);
			Print(funEvaledParams.Parameters, true);
			writer.WriteLine(" {");
			writer.Indent();

			Print(funEvaledParams.Statements);

			writer.Unindent();
			writer.WriteLine("}");
		}

		public void Print(Function fun) {

			Print(Ast.Keyword.Fun);
			writer.Write(fun.Name);
			Print(fun.Parameters);
			writer.WriteLine(" {");
			writer.Indent();

			Print(fun.Statements);

			writer.Unindent();
			writer.WriteLine("}");
		}

		public void Print(ImmutableList<IFunctionStatement> funStats) {

			foreach (var stat in funStats) {
				switch (stat.StatementType) {
					case FunctionStatementType.ConstantDefinition:
						Print((ConstantDefinition)stat);
						break;
					case FunctionStatementType.ReturnExpression:
						Print(Ast.Keyword.Return);
						Print(((FunctionReturnExpr)stat).ReturnValue);
						writer.WriteLine(";");
						break;
					default:
						break;
				}
			}
		}

		public void Print(ConstantDefinition constDef) {
			Print(Ast.Keyword.Let);
			writer.Write(constDef.Name);
			writer.Write(" = ");
			Print(constDef.Value);
			writer.WriteLine(";");
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


		public void Print(IExpression expr) {
			expr.Accept(this);
		}

		public void Print(IValue value) {
			if (value.IsConstant) {
				Visit((Constant)value);
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


		#region IExpressionVisitor Members

		public void Visit(Constant constant) {

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

		public void Visit(ExprVariable variable) {
			writer.Write(variable.Name);
		}

		public void Visit(ExpressionValuesArray expressionValuesArray) {
			writer.Write("{");
			PrintSeparated(expressionValuesArray, e => Print(e));
			writer.Write("}");
		}

		public void Visit(UnaryOperator unaryOperator) {
			writer.Write("(");
			writer.Write(unaryOperator.Syntax);
			unaryOperator.Operand.Accept(this);
			writer.Write(")");
		}

		public void Visit(BinaryOperator binaryOperator) {
			writer.Write("(");
			binaryOperator.LeftOperand.Accept(this);
			writer.Write(" ");
			writer.Write(binaryOperator.Syntax);
			writer.Write(" ");
			binaryOperator.RightOperand.Accept(this);
			writer.Write(")");
		}

		public void Visit(Indexer indexer) {
			indexer.Array.Accept(this);
			writer.Write("[");
			indexer.Index.Accept(this);
			writer.Write("]");
		}

		public void Visit(FunctionCall functionCall) {
			writer.Write(functionCall.Name);
			writer.Write("(");
			PrintSeparated(functionCall.Arguments, e => Print(e));
			writer.Write(")");
		}

		public void Visit(UserFunctionCall userFunction) {
			writer.Write(userFunction.Name);
			writer.Write("(");
			PrintSeparated(userFunction.Arguments, e => Print(e));
			writer.Write(")");
		}

		public void Visit(EmptyExpression emptyExpression) {
			writer.WriteLine(";");
		}

		#endregion
	}
}