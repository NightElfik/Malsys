using System.Collections.Generic;
using System.Diagnostics;
using Malsys.Compilers;

namespace Malsys.SourceCode.Highlighters {
	static class MarksCollector {

		public static void Collect(Ast.InputBlock ib, List<PositionMark> marks) {
			foreach (var statement in ib) {
				if (statement is Ast.Lsystem) {
					Collect((Ast.Lsystem)statement, marks);
				}
				else if (statement is Ast.VariableDefinition) {
					Collect((Ast.VariableDefinition)statement, marks);
				}
				else if (statement is Ast.FunctionDefinition) {
					Collect((Ast.FunctionDefinition)statement, marks);
				}
				else if (statement is Ast.EmptyStatement) {
					continue;
				}
				else {
					Debug.Fail("Unhandled type `{0}` of {1} while creating highlighter marks.".Fmt(
						statement.GetType().Name, typeof(Ast.IInputStatement).Name));
					continue;
				}
			}
		}


		public static void Collect(MessagesCollection msgs, List<PositionMark> marks) {

			foreach (var msg in msgs) {
				MarkType mt;
				switch (msg.Type) {
					case CompilerMessageType.Error:
						mt = MarkType.MsgError; break;
					case CompilerMessageType.Warning:
						mt = MarkType.MsgWarning; break;
					case CompilerMessageType.Notice:
						mt = MarkType.MsgNotice; break;
					default:
						mt = MarkType.Unknown; break;
				}

				addMarks(mt, msg.Position, marks);
			}
		}

		public static void Collect(Ast.Comment cmt, List<PositionMark> marks) {
			if (cmt.Text.StartsWith("//")) {
				addMarks(MarkType.SingleLineComment, cmt.Position, marks);
			}
			else if (cmt.Text.StartsWith("/**")) {
				addMarks(MarkType.DocComment, cmt.Position, marks);
			}
			else {
				addMarks(MarkType.MultiLineComment, cmt.Position, marks);
			}
		}

		public static void Collect(IEnumerable<Ast.Comment> cmts, List<PositionMark> marks) {
			foreach (var cmt in cmts) {
				Collect(cmt, marks);
			}
		}

		public static void Collect(Ast.Lsystem lsd, List<PositionMark> marks) {

			addMarks(MarkType.Lsystem, lsd.Position, marks);

			Collect(lsd.Keyword, marks);
			addMarks(MarkType.LsystemName, lsd.NameId.Position, marks);

			foreach (var statement in lsd.Statements) {
				if (statement is Ast.RewriteRule) {
					Collect((Ast.RewriteRule)statement, marks);
				}

				else if (statement is Ast.VariableDefinition) {
					Collect((Ast.VariableDefinition)statement, marks);
				}

				else if (statement is Ast.FunctionDefinition) {
					Collect((Ast.FunctionDefinition)statement, marks);
				}

				else if (statement is Ast.EmptyStatement) {
					continue;
				}

				else {
					Debug.Fail("Unhandled type `{0}` of {1} while creating highlighter marks of L-system `{2}`".Fmt(
						statement.GetType().Name, typeof(Ast.ILsystemStatement).Name, lsd.NameId.Name));
					continue;
				}
			}

		}

		public static void Collect(Ast.VariableDefinition vd, List<PositionMark> marks) {

			addMarks(MarkType.VariableDefinition, vd.Position, marks);

			addMarks(MarkType.Keyword, vd.Keyword.Position, marks);
			addMarks(MarkType.VariableName, vd.NameId.Position, marks);

			Collect(vd.Expression, marks);
		}

		public static void Collect(Ast.FunctionDefinition fd, List<PositionMark> marks) {

			addMarks(MarkType.FunctionDefinition, fd.Position, marks);

			addMarks(MarkType.Keyword, fd.Keyword.Position, marks);
			addMarks(MarkType.FunctionName, fd.NameId.Position, marks);

			foreach (var p in fd.Parameters) {
				Collect(p, marks);
			}

			foreach (var vd in fd.VariableDefinitions) {
				Collect(vd, marks);
			}

			addMarks(MarkType.Expression, fd.Expression.Position, marks);
		}

		public static void Collect(Ast.OptionalParameter p, List<PositionMark> marks) {
			addMarks(MarkType.ParameterName, p.NameId.Position, marks);

			if (p.OptionalValue != null) {
				Collect(p.OptionalValue, marks);
			}
		}

		public static void Collect(Ast.RewriteRule rr, List<PositionMark> marks) {

			addMarks(MarkType.RewriteRule, rr.Position, marks);

			addMarks(MarkType.RrPattern, rr.Pattern.Position, marks);
			Collect(rr.Pattern, marks);

			addMarks(MarkType.RrLeftCtxt, rr.LeftContext.Position, marks);
			foreach (var pat in rr.LeftContext) {
				Collect(pat, marks);
			}

			addMarks(MarkType.RrRightCtxt, rr.RightContext.Position, marks);
			foreach (var pat in rr.RightContext) {
				Collect(pat, marks);
			}

			addMarks(MarkType.RrCondition, rr.Condition.Position, marks);
			Collect(rr.Condition, marks);

			addMarks(MarkType.RrProbability, rr.Probability.Position, marks);
			Collect(rr.Probability, marks);

			foreach (var vd in rr.VariableDefs) {
				Collect(vd, marks);
			}

			foreach (var sym in rr.Replacement) {
				Collect(sym, marks);
			}
		}

		public static void Collect(Ast.RichExpression re, List<PositionMark> marks) {
			foreach (var vd in re.VariableDefinitions) {
				Collect(vd, marks);
			}

			Collect(re.Expression, marks);
		}

		public static void Collect(Ast.SymbolPattern ptrn, List<PositionMark> marks) {
			addMarks(MarkType.Symbol, ptrn.Position, marks);

			foreach (var name in ptrn.ParametersNames) {
				addMarks(MarkType.VariableName, name.Position, marks);
			}
		}

		public static void Collect(Ast.SymbolExprArgs symExpr, List<PositionMark> marks) {
			addMarks(MarkType.Symbol, symExpr.Position, marks);

			foreach (var expr in symExpr.Arguments) {
				Collect(expr, marks);
			}
		}

		public static void Collect(Ast.Keyword kw, List<PositionMark> marks) {
			addMarks(MarkType.Keyword, kw.Position, marks);
		}

		public static void Collect(Ast.Expression expr, List<PositionMark> marks) {
			if (expr.IsEmpty) {
				return;
			}

			addMarks(MarkType.Expression, expr.Position, marks);
		}


		private static void addMarks(MarkType type, Position pos, List<PositionMark> marks) {
			if (pos.IsUnknown) {
				return;
			}

			marks.Add(new PositionMark(type, pos.BeginLine, pos.BeginColumn, true));
			marks.Add(new PositionMark(type, pos.EndLine, pos.EndColumn, false));
		}
	}
}
