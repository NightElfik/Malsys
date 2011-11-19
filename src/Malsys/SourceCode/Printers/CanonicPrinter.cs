using System;
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



		public void Print(IExpression expr) {
			expr.Accept(this);
		}

		public void PrintExprSeparated(IEnumerable<IExpression> exprs, string separator = ", ") {

			bool first = true;

			foreach (var expr in exprs) {
				if (first) {
					first = false;
				}
				else {
					writer.Write(separator);
				}

				expr.Accept(this);
			}
		}

		public void PrintValueSeparated(IEnumerable<IValue> values, string separator = ", ") {

			bool first = true;

			foreach (var val in values) {
				if (first) {
					first = false;
				}
				else {
					writer.Write(separator);
				}

				Print(val);
			}
		}

		public void Print(Symbol<IValue> symbol) {
			writer.Write(symbol.Name);
			if (!symbol.Arguments.IsEmpty) {
				writer.Write("(");
				PrintValueSeparated(symbol.Arguments);
				writer.Write(")");
			}
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
			PrintValueSeparated(arr);
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

		public void Visit(EmptyExpression emptyExpression) {
			writer.WriteLine(";");
		}

		#endregion
	}
}