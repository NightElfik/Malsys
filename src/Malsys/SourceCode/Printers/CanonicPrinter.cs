using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.Expressions;
using Malsys.IO;
using Malsys.Ast;

namespace Malsys.SourceCode.Printers {
	public class CanonicPrinter : IExpressionVisitor {


		private IndentTextWriter writer;


		public CanonicPrinter(IndentTextWriter writer) {
			this.writer = writer;
		}



		public void Print(IExpression expr) {
			expr.Accept(this);
		}

		/// <summary>
		/// Prints separator between items.
		/// </summary>
		public void PrintExprSeparated<T>(IEnumerable<T> tokens, string separator = ", ") where T : IExpression {

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


		#region IExpressionVisitor Members

		public void Visit(Constant constant) {
			ConstantFormat fmt = ConstantFormat.Float;
			if (constant.AstNode != null) {
				fmt = constant.AstNode.Format;
			}

			switch (fmt) {
				case ConstantFormat.Binary:
					long valBin = (long)Math.Round(constant.Value);
					writer.Write("0b");
					writer.Write(Convert.ToString(valBin, 2));
					break;
				case ConstantFormat.Octal:
					long valOct = (long)Math.Round(constant.Value);
					writer.Write("0o");
					writer.Write(Convert.ToString(valOct, 8));
					break;
				case ConstantFormat.Hexadecimal:
					long valHex = (long)Math.Round(constant.Value);
					writer.Write("0x");
					writer.Write(Convert.ToString(valHex, 16).ToUpper());
					break;
				case ConstantFormat.HashHexadecimal:
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
			PrintExprSeparated(expressionValuesArray);
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
			PrintExprSeparated(functionCall.Arguments);
			writer.Write(")");
		}

		public void Visit(UserFunctionCall userFunction) {
			writer.Write(userFunction.Name);
			writer.Write("(");
			PrintExprSeparated(userFunction.Arguments);
			writer.Write(")");
		}

		#endregion
	}
}
