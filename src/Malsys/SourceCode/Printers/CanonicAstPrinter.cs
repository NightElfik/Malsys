using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.Ast;
using System.Diagnostics;
using Malsys.IO;

namespace Malsys.SourceCode.Printers {
	public class CanonicAstPrinter : IAstVisitor {

		private IndentTextWriter writer;


		public CanonicAstPrinter(IndentTextWriter writer) {
			this.writer = writer;
		}


		public void Visit(Ast.InputBlock input) {
			foreach (var statement in input) {
				statement.Accept(this);
			}
		}

		/// <summary>
		/// Prints separator between items.
		/// </summary>
		public void VisitSeparated<T>(IEnumerable<T> tokens, string separator = ", ") where T : IToken {

			bool first = true;

			foreach (var tok in tokens) {
				if (first) {
					first = false;
				}
				else {
					writer.Write(separator);
				}

				tok.Accept(this);
			}
		}

		public void Visit<T>(ImmutableList<T> tokList) where T : IToken {
			foreach (var tok in tokList) {
				tok.Accept(this);
			}
		}

		#region IAstVisitor Members

		public void Visit(Comment comment) {
			// skip any comments
		}

		public void Visit(EmptyStatement emptyStat) {
			writer.WriteLine(";");
		}

		public void Visit(Expression expr) {
			foreach (var member in expr) {
				member.Accept(this);
			}
		}

		public void Visit(ExpressionBracketed bracketedExpr) {
			writer.Write("(");
			Visit(bracketedExpr.Expression);
			writer.Write(")");
		}

		public void Visit(ExpressionFunction funExpr) {
			writer.Write(funExpr.NameId.Name);
			writer.Write("(");
			VisitSeparated(funExpr.Arguments);
			writer.Write(")");
		}

		public void Visit(ExpressionIndexer indexerExpr) {
			writer.Write("[");
			Visit(indexerExpr.Index);
			writer.Write("]");
		}

		public void Visit(ExpressionsArray arrExpr) {
			writer.Write("{");
			VisitSeparated(arrExpr);
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

		public void Visit(Ast.FunctionDefinition funDef) {
			writer.Write("fun ");
			writer.Write(funDef.NameId.Name);
			writer.Write("(");
			VisitSeparated(funDef.Parameters);
			writer.Write(")");

			writer.Write(" {");
			writer.NewLine();
			writer.Indent();

			Visit(funDef.LocalVarDefs);
			writer.Write("return ");
			Visit(funDef.ReturnExpression);
			writer.Write(";");

			writer.Unindent();
			writer.NewLine();
			writer.Write("}");
			writer.NewLine();
		}

		public void Visit(KeywordPos keyword) {
			throw new NotImplementedException();
		}

		public void Visit(Lsystem lsystem) {
			writer.NewLine();
			writer.Write("lsystem ");
			writer.Write(lsystem.NameId.Name);
			if (!lsystem.Parameters.IsEmpty) {
				writer.Write("(");
				VisitSeparated(lsystem.Parameters);
				writer.Write(")");
			}
			writer.Write(" {");
			writer.Indent();

			foreach (var statement in lsystem.Body) {
				writer.NewLine();
				statement.Accept(this);
			}

			writer.Unindent();
			writer.Write("}");
			writer.NewLine();
		}

		public void Visit(Ast.OptionalParameter optParam) {
			writer.Write(optParam.NameId.Name);
			if (optParam.IsOptional) {
				writer.Write(" = ");
				Visit(optParam.OptionalValue);
			}
		}

		public void Visit(Identificator id) {
			writer.Write(id.Name);
		}

		public void Visit<T>(ImmutableListPos<T> tokList) where T : IToken {
			foreach (var tok in tokList) {
				tok.Accept(this);
			}
		}

		public void Visit(Ast.RewriteRule rewriteRule) {
			writer.Write("rewrite ");

			if (!rewriteRule.LeftContext.IsEmpty) {
				writer.Write("{");
				Visit(rewriteRule.LeftContext);
				writer.Write("} ");
			}

			Visit(rewriteRule.Pattern);

			if (!rewriteRule.RightContext.IsEmpty) {
				writer.Write(" {");
				Visit(rewriteRule.RightContext);
				writer.Write("}");
			}

			writer.Indent();

			if (!rewriteRule.LocalVariables.IsEmpty) {
				writer.NewLine();
				writer.Write("with ");
				VisitSeparated(rewriteRule.LocalVariables);
			}

			if (!rewriteRule.Condition.IsEmpty) {
				writer.NewLine();
				writer.Write("where ");
				Visit(rewriteRule.Condition);
			}


			bool first = true;
			foreach (var replac in rewriteRule.Replacements) {
				writer.NewLine();
				if (first) {
					first = false;
				}
				else {
					writer.Write("or ");
				}

				Visit(replac);
			}

			writer.WriteLine(";");
			writer.Unindent();
		}

		public void Visit(RewriteRuleReplacement rrReplacment) {

			writer.Write("to ");

			if (rrReplacment.Replacement.IsEmpty) {
				writer.Write("nothing");
			}
			else {
				Visit(rrReplacment.Replacement);
			}

			if (!rrReplacment.Weight.IsEmpty) {
				writer.Write(" weight ");
				Visit(rrReplacment.Weight);
			}
		}

		public void Visit(Ast.RichExpression richExpr) {
		}

		public void Visit<T>(Ast.Symbol<T> symbol) where T : IToken {
			writer.Write(symbol.Name);

			if (!symbol.Arguments.IsEmpty) {
				writer.Write("(");
				VisitSeparated(symbol.Arguments);
				writer.Write(")");
			}
		}

		public void Visit(SymbolsDefinition symbolDef) {
			throw new NotImplementedException();
		}

		public void Visit(Operator op) {
			writer.Write(" ");
			writer.Write(op.Syntax);
			writer.Write(" ");
		}

		public void Visit(VariableDefinition variableDef) {
			if (!variableDef.Keyword.IsEmpty) {
				writer.Write("let ");
			}
			writer.Write(variableDef.NameId.Name);
			writer.Write(" = ");
			Visit(variableDef.Expression);

			if (!variableDef.Keyword.IsEmpty) {
				writer.WriteLine(";");
			}
		}

		public void Visit(InvalidExpression invExpr) {

		}

		#endregion
	}
}
