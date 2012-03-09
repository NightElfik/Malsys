using System;
using Malsys.Ast;
using Malsys.IO;
using System.Diagnostics;

namespace Malsys.SourceCode.Printers {
	public class CanonicAstPrinter {

		private IndentWriter writer;



		public CanonicAstPrinter(IndentWriter writer) {
			this.writer = writer;
		}



		public void Print(InputBlock input) {
			foreach (var stat in input.Statements) {
				Print(stat);
			}
		}

		public void Print(Keyword kw, bool includeSpace = true) {
			writer.Write(EnumHelper.GetStringVal(kw));
			if (includeSpace) {
				writer.Write(" ");
			}
		}

		/// <summary>
		/// Prints separator between values.
		/// </summary>
		public void PrintSeparated<T>(ImmutableList<T> values, Action<T> printFunc, string separator = ", ") {

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


		public void Print(LsystemSymbol symbol) {
			writer.Write(symbol.Name);

			if (!symbol.Arguments.IsEmpty) {
				writer.Write("(");
				PrintSeparated(symbol.Arguments, s => Print(s));
				writer.Write(")");
			}
		}

		public void Print(RewriteRuleReplacement rrReplacment) {

			Print(Keyword.To);

			if (rrReplacment.Replacement.IsEmpty) {
				Print(Keyword.Nothing, false);
			}
			else {
				PrintSeparated(rrReplacment.Replacement, s => Print(s), " ");
			}

			if (!rrReplacment.Weight.IsEmpty) {
				writer.Write(" ");
				Print(Keyword.Weight);
				Print(rrReplacment.Weight);
			}
		}

		public void Print(OptionalParameter optParam) {

			writer.Write(optParam.NameId.Name);

			if (optParam.IsOptional) {
				writer.Write(" = ");
				Print(optParam.DefaultValue);
			}
		}

		public void Print(ImmutableList<OptionalParameter> optParams, bool forceParens = false) {
			if (forceParens || !optParams.IsEmpty) {
				writer.Write("(");
				PrintSeparated(optParams, op => Print(op));
				writer.Write(")");
			}

		}

		public void Print(ProcessComponentAssignment processCompAssign) {
			Print(Keyword.Use);
			writer.Write(processCompAssign.ComponentTypeNameId.Name);
			writer.Write(" ");

			Print(Keyword.As);
			writer.Write(processCompAssign.ContainerNameId.Name);
		}


		#region Input statements

		public void Print(IInputStatement stat) {

			switch (stat.StatementType) {
				case InputStatementType.EmptyStatement: Print((Ast.EmptyStatement)stat); break;
				case InputStatementType.ConstantDefinition: Print((Ast.ConstantDefinition)stat); break;
				case InputStatementType.FunctionDefinition: Print((Ast.FunctionDefinition)stat); break;
				case InputStatementType.LsystemDefinition: Print((Ast.LsystemDefinition)stat); break;
				case InputStatementType.ProcessStatement: Print((Ast.ProcessStatement)stat); break;
				case InputStatementType.ProcessConfigurationDefinition: Print((Ast.ProcessConfigurationDefinition)stat); break;
				default: break;
			}

		}

		public void Print(EmptyStatement emptyStat) {
			writer.Write(";");
		}

		public void Print(ConstantDefinition constDef) {

			if (constDef.IsComponentAssign) {
				Print(Keyword.Set);
			}
			else {
				Print(Keyword.Let);
			}
			writer.Write(constDef.NameId.Name);
			writer.Write(" = ");
			Print(constDef.ValueExpr);
			writer.Write(";");
		}

		public void Print(FunctionDefinition funDef) {
			Print(Keyword.Fun);
			writer.Write(funDef.NameId.Name);
			Print(funDef.Parameters, true);

			writer.Write(" {");
			writer.Indent();

			foreach (var stat in funDef.Statements) {
				writer.NewLine();
				Print(stat);
			}

			writer.Unindent();
			writer.NewLine();
			writer.Write("}");
		}

		public void Print(LsystemDefinition lsysDef) {

			Print(Keyword.Lsystem);
			writer.Write(lsysDef.NameId.Name);
			Print(lsysDef.Parameters);

			writer.Write(" {");
			writer.Indent();

			foreach (var stat in lsysDef.Statements) {
				writer.NewLine();
				Print(stat);
			}

			writer.Unindent();
			writer.NewLine();
			writer.WriteLine("}");
		}

		public void Print(ProcessStatement processDef) {

			Print(Keyword.Process);
			if (!processDef.TargetLsystemNameId.IsEmpty) {
				writer.Write(processDef.TargetLsystemNameId.Name);
			}
			else {
				Print(Keyword.All, false);
			}

			if (!processDef.Arguments.IsEmpty) {
				writer.Write("(");
				PrintSeparated(processDef.Arguments, x => Print(x));
				writer.Write(")");
			}

			writer.Write(" ");

			Print(Keyword.With);
			writer.Write(processDef.ProcessConfiNameId.Name);

			writer.Indent();
			foreach (var assign in processDef.ComponentAssignments) {
				writer.NewLine();
				Print(assign);
			}

			writer.Unindent();
			writer.Write(";");
		}

		public void Print(ProcessConfigurationDefinition processConfDef) {

			Print(Keyword.Configuration);
			writer.Write(processConfDef.NameId.Name);

			writer.Write(" {");
			writer.Indent();

			foreach (var stat in processConfDef.Statements) {
				writer.NewLine();
				Print(stat);
			}

			writer.Unindent();
			writer.NewLine();
			writer.WriteLine("}");
		}

		#endregion


		#region L-system statements

		public void Print(ILsystemStatement stat) {

			switch (stat.StatementType) {
				case LsystemStatementType.EmptyStatement: Print((Ast.EmptyStatement)stat); break;
				case LsystemStatementType.ConstantDefinition: Print((Ast.ConstantDefinition)stat); break;
				case LsystemStatementType.SymbolsConstDefinition: Print((Ast.SymbolsConstDefinition)stat); break;
				case LsystemStatementType.SymbolsInterpretDef: Print((Ast.SymbolsInterpretDef)stat); break;
				case LsystemStatementType.FunctionDefinition: Print((Ast.FunctionDefinition)stat); break;
				case LsystemStatementType.RewriteRule: Print((Ast.RewriteRule)stat); break;
				default: Debug.Fail("Unknown L-system statement type `{0}`.".Fmt(stat.StatementType.ToString())); break;
			}

		}

		public void Print(SymbolsConstDefinition symbolsDef) {
			Print(Keyword.Set);
			Print(Keyword.Symbols);
			writer.Write(symbolsDef.NameId.Name);
			writer.Write(" = ");
			PrintSeparated(symbolsDef.SymbolsList, s => Print(s), " ");
			writer.Write(";");
		}

		public void Print(SymbolsInterpretDef symIntDef) {

			Print(Keyword.Interpret);
			PrintSeparated(symIntDef.Symbols, s => writer.Write(s.Name), " ");

			writer.Write(" ");
			Print(Keyword.As);

			writer.Write(symIntDef.Instruction.Name);

			if (!symIntDef.InstructionParameters.IsEmpty) {
				writer.Write("(");
				PrintSeparated(symIntDef.InstructionParameters, e => Print(e));
				writer.Write(")");
			}

			writer.Write(";");
		}

		public void Print(RewriteRule rewriteRule) {
			Print(Keyword.Rewrite);

			if (!rewriteRule.LeftContext.IsEmpty) {
				writer.Write("{");
				PrintSeparated(rewriteRule.LeftContext, s => Print(s), " ");
				writer.Write("} ");
			}

			Print(rewriteRule.Pattern);

			if (!rewriteRule.RightContext.IsEmpty) {
				writer.Write(" {");
				PrintSeparated(rewriteRule.RightContext, s => Print(s), " ");
				writer.Write("}");
			}

			writer.Indent();

			if (!rewriteRule.LocalConstDefs.IsEmpty) {
				writer.NewLine();
				Print(Keyword.With);
				PrintSeparated(rewriteRule.LocalConstDefs,
					cd => {
						writer.Write(cd.NameId.Name);
						writer.Write(" = ");
						Print(cd.ValueExpr);
					});
			}

			if (!rewriteRule.Condition.IsEmpty) {
				writer.NewLine();
				Print(Keyword.Where);
				Print(rewriteRule.Condition);
			}


			bool first = true;
			foreach (var replac in rewriteRule.Replacements) {
				writer.NewLine();
				if (first) {
					first = false;
				}
				else {
					Print(Keyword.Or);
				}


				Print(replac);
			}

			writer.Write(";");
			writer.Unindent();
		}

		#endregion


		#region Expression members

		public void Print(Expression expr) {
			foreach (var member in expr) {
				Print(member);
			}
		}

		public void Print(IExpressionMember member) {

			switch (member.MemberType) {
				case ExpressionMemberType.EmptyExpression: Print((Ast.EmptyExpression)member); break;
				case ExpressionMemberType.ExpressionBracketed: Print((Ast.ExpressionBracketed)member); break;
				case ExpressionMemberType.ExpressionFunction: Print((Ast.ExpressionFunction)member); break;
				case ExpressionMemberType.ExpressionIndexer: Print((Ast.ExpressionIndexer)member); break;
				case ExpressionMemberType.ExpressionsArray: Print((Ast.ExpressionsArray)member); break;
				case ExpressionMemberType.FloatConstant: Print((Ast.FloatConstant)member); break;
				case ExpressionMemberType.Identificator: Print((Ast.Identificator)member); break;
				case ExpressionMemberType.Operator: Print((Ast.Operator)member); break;
				default: Debug.Fail("Unknown expression member type `{0}`.".Fmt(member.MemberType.ToString())); break;
			}

		}

		public void Print(EmptyExpression emptyExpr) {

		}

		public void Print(ExpressionBracketed bracketedExpr) {
			writer.Write("(");
			Print(bracketedExpr.Expression);
			writer.Write(")");
		}

		public void Print(ExpressionFunction funExpr) {
			writer.Write(funExpr.NameId.Name);
			writer.Write("(");
			PrintSeparated(funExpr.Arguments, s => Print(s));
			writer.Write(")");
		}

		public void Print(ExpressionIndexer indexerExpr) {
			writer.Write("[");
			Print(indexerExpr.Index);
			writer.Write("]");
		}

		public void Print(ExpressionsArray arrExpr) {
			writer.Write("{");
			PrintSeparated(arrExpr, s => Print(s));
			writer.Write("}");
		}

		public void Print(FloatConstant floatConstant) {
			writer.Write(floatConstant.ToString());
		}

		public void Print(Identificator variable) {
			writer.Write(variable.Name);
		}

		public void Print(Operator optor) {
			writer.Write(" ");
			writer.Write(optor.Syntax);
			writer.Write(" ");
		}

		#endregion


		#region Function statements

		public void Print(IFunctionStatement stat) {

			switch (stat.StatementType) {
				case FunctionStatementType.ConstantDefinition: Print((ConstantDefinition)stat); break;
				case FunctionStatementType.Expression:
					Print(Keyword.Return);
					Print((Expression)stat);
					writer.Write(";");
					break;
				default: Debug.Fail("Unknown function statement type `{0}`.".Fmt(stat.StatementType.ToString())); break;
			}

		}

		#endregion


		#region Process configuration statements

		public void Print(IProcessConfigStatement stat) {

			switch (stat.StatementType) {
				case ProcessConfigStatementType.EmptyStatement: Print((EmptyStatement)stat); break;
				case ProcessConfigStatementType.ProcessComponent: Print((ProcessComponent)stat); break;
				case ProcessConfigStatementType.ProcessContainer: Print((ProcessContainer)stat); break;
				case ProcessConfigStatementType.ProcessConfigConnection: Print((ProcessConfigConnection)stat); break;
				default: Debug.Fail("Unknown configuration statement type `{0}`.".Fmt(stat.StatementType.ToString())); break;
			}

		}

		public void Print(ProcessComponent component) {
			Print(Keyword.Component);
			writer.Write(component.NameId.Name);
			writer.Write(" ");
			Print(Keyword.Typeof);
			writer.Write(component.TypeNameId.Name);
			writer.Write(";");
		}

		public void Print(ProcessContainer container) {
			Print(Keyword.Container);
			writer.Write(container.NameId.Name);
			writer.Write(" ");
			Print(Keyword.Typeof);
			writer.Write(container.TypeNameId.Name);
			writer.Write(" ");
			Print(Keyword.Default);
			writer.Write(container.DefaultTypeNameId.Name);
			writer.Write(";");
		}

		public void Print(ProcessConfigConnection connection) {
			Print(Keyword.Connect);
			writer.Write(connection.SourceNameId.Name);
			writer.Write(" ");
			Print(Keyword.To);
			writer.Write(connection.TargetNameId.Name);
			writer.Write(".");
			writer.Write(connection.TargetInputNameId.Name);
			writer.Write(";");
		}

		#endregion


	}
}
