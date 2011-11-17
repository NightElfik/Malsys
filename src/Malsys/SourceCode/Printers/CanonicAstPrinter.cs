using System;
using Malsys.Ast;
using Malsys.IO;

namespace Malsys.SourceCode.Printers {
	public class CanonicAstPrinter : IInputVisitor, ILsystemVisitor, IExpressionVisitor, IFunctionVisitor {

		private IndentTextWriter writer;



		public CanonicAstPrinter(IndentTextWriter writer) {
			this.writer = writer;
		}


		public void Print(InputBlock input) {
			foreach (var statement in input) {
				statement.Accept(this);
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

		public void Print(Expression expr) {
			foreach (var member in expr) {
				member.Accept(this);
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


		#region IInputVisitor Members

		public void Visit(ConstantDefinition constDef) {

			Print(Keyword.Let);
			writer.Write(constDef.NameId.Name);
			writer.Write(" = ");
			Print(constDef.ValueExpr);
			writer.Write(";");
		}

		public void Visit(EmptyStatement emptyStat) {
			writer.WriteLine(";");
		}

		public void Visit(FunctionDefinition funDef) {
			Print(Keyword.Fun);
			writer.Write(funDef.NameId.Name);
			Print(funDef.Parameters, true);

			writer.Write(" {");
			writer.Indent();

			foreach (var stat in funDef.Statements) {
				writer.NewLine();
				stat.Accept(this);
			}

			writer.Unindent();
			writer.WriteLine("}");
		}

		public void Visit(LsystemDefinition lsysDef) {

			Print(Keyword.Lsystem);
			writer.Write(lsysDef.NameId.Name);
			Print(lsysDef.Parameters);

			writer.Write(" {");
			writer.Indent();

			foreach (var statement in lsysDef.Statements) {
				writer.NewLine();
				statement.Accept(this);
			}

			writer.Unindent();
			writer.WriteLine("}");
		}

		#endregion

		#region ILsystemVisitor Members

		public void Visit(RewriteRule rewriteRule) {
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

			writer.WriteLine(";");
			writer.Unindent();
		}

		public void Visit(SymbolsInterpretDef symIntDef) {

			Print(Keyword.Interpret);
			PrintSeparated(symIntDef.Symbols, s => Print(s), " ");

			writer.Write(" ");
			Print(Keyword.As);

			writer.Write(symIntDef.Instruction.Name);

			if (!symIntDef.DefaultParameters.IsEmpty) {
				writer.Write("(");
				PrintSeparated(symIntDef.DefaultParameters, e => Print(e));
				writer.Write(")");
			}

			writer.WriteLine(";");
		}

		public void Visit(SymbolsConstDefinition symbolsDef) {
			Print(Keyword.Set);
			writer.Write(symbolsDef.NameId.Name);
			writer.Write(" = ");
			PrintSeparated(symbolsDef.SymbolsList, s => Print(s), " ");
			writer.WriteLine(";");
		}

		#endregion

		#region IExpressionVisitor Members

		public void Visit(ExpressionBracketed bracketedExpr) {
			writer.Write("(");
			Print(bracketedExpr.Expression);
			writer.Write(")");
		}

		public void Visit(ExpressionFunction funExpr) {
			writer.Write(funExpr.NameId.Name);
			writer.Write("(");
			PrintSeparated(funExpr.Arguments, s => Print(s));
			writer.Write(")");
		}

		public void Visit(ExpressionIndexer indexerExpr) {
			writer.Write("[");
			Print(indexerExpr.Index);
			writer.Write("]");
		}

		public void Visit(ExpressionsArray arrExpr) {
			writer.Write("{");
			PrintSeparated(arrExpr, s => Print(s));
			writer.Write("}");
		}

		public void Visit(FloatConstant floatConstant) {

			double val = floatConstant.Value;

			switch (floatConstant.Format) {
				case ConstantFormat.Binary:
					long valBin = (long)Math.Round(val);
					writer.Write("0b");
					writer.Write(Convert.ToString(valBin, 2));
					break;

				case ConstantFormat.Octal:
					long valOct = (long)Math.Round(val);
					writer.Write("0o");
					writer.Write(Convert.ToString(valOct, 8));
					break;

				case ConstantFormat.Hexadecimal:
					long valHex = (long)Math.Round(val);
					writer.Write("0x");
					writer.Write(Convert.ToString(valHex, 16).ToUpper());
					break;

				case ConstantFormat.HashHexadecimal:
					long valHash = (long)Math.Round(val);
					writer.Write("#");
					writer.Write(Convert.ToString(valHash, 16).ToUpper());
					break;

				default:
					writer.Write(val.ToStringInvariant());
					break;
			}
		}

		public void Visit(Identificator variable) {
			writer.Write(variable.Name);
		}

		public void Visit(Operator optor) {
			writer.Write(" ");
			writer.Write(optor.Syntax);
			writer.Write(" ");
		}

		#endregion

		#region IFunctionVisitor Members

		void IFunctionVisitor.Visit(Expression expr) {
			Print(Keyword.Return);
			Print(expr);
			writer.Write(";");
		}

		#endregion
	}
}
