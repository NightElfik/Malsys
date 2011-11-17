using System.Collections.Generic;
using System.Diagnostics;
using Malsys.Compilers;
/*
namespace Malsys.SourceCode.Highlighters {
	static class MarksCollector {

		public static void Collect(Ast.InputBlock ib, List<PositionMark> marks) {
			foreach (var statement in ib) {
				if (statement is Ast.Lsystem) {
					Collect((Ast.Lsystem)statement, marks);
				}
				else if (statement is Ast.Binding) {
					Collect((Ast.Binding)statement, marks);
				}
				else if (statement is Ast.Function) {
					Collect((Ast.Function)statement, marks);
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

				var data = new object[] { msg.Message };

				addMarks(mt, msg.Position.ToNonZeroLength(), marks, data);

				foreach (var pos in msg.OtherPositions) {
					addMarks(mt, pos, marks, data);
				}
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

			foreach (var p in lsd.Parameters) {
				Collect(p, marks);
			}

			addMarks(MarkType.LsystemBody, lsd.Statements.Position, marks);
			addMarks(MarkType.Separator, lsd.Statements.BeginSeparator, marks);

			foreach (var statement in lsd.Statements) {
				if (statement is Ast.RewriteRule) {
					Collect((Ast.RewriteRule)statement, marks);
				}

				else if (statement is Ast.Binding) {
					Collect((Ast.Binding)statement, marks);
				}

				else if (statement is Ast.SymbolsDefinition) {
					Collect((Ast.SymbolsDefinition)statement, marks);
				}

				else if (statement is Ast.Function) {
					Collect((Ast.Function)statement, marks);
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

			addMarks(MarkType.Separator, lsd.Statements.EndSeparator, marks);
		}

		public static void Collect(Ast.Binding vd, List<PositionMark> marks) {

			addMarks(MarkType.VariableDefinition, vd.Position, marks);

			addMarks(MarkType.Keyword, vd.Keyword.Position, marks);
			addMarks(MarkType.VariableName, vd.NameId.Position, marks);

			Collect(vd.Expression, marks);
		}

		public static void Collect(Ast.SymbolsDefinition sd, List<PositionMark> marks) {

			addMarks(MarkType.VariableDefinition, sd.Position, marks);

			addMarks(MarkType.Keyword, sd.Keyword.Position, marks);
			addMarks(MarkType.VariableName, sd.NameId.Position, marks);

			foreach (var s in sd.Symbols) {
				Collect(s, marks);
			}
		}

		public static void Collect(Ast.Function fd, List<PositionMark> marks) {

			addMarks(MarkType.FunctionDefinition, fd.Position, marks);

			addMarks(MarkType.FunctionName, fd.NameId.Position, marks);
			foreach (var p in fd.Parameters) {
				Collect(p, marks);
			}


			addMarks(MarkType.Separator, fd.LocalBindings.BeginSeparator, marks);

			foreach (var vd in fd.LocalBindings) {
				Collect(vd, marks);
			}
			Collect(fd.ReturnExpression, marks);

			addMarks(MarkType.Separator, fd.LocalBindings.EndSeparator, marks);

			foreach (var kw in fd.keywords) {
				Collect(kw, marks);
			}
		}

		public static void Collect(Ast.OptionalParameter p, List<PositionMark> marks) {
			addMarks(MarkType.Parameter, p.Position, marks);
			addMarks(MarkType.ParameterName, p.NameId.Position, marks);

			if (p.OptionalValue != null) {
				Collect(p.OptionalValue, marks);
			}
		}

		public static void Collect(Ast.RewriteRule rr, List<PositionMark> marks) {

			addMarks(MarkType.RewriteRule, rr.Position, marks);

			addMarks(MarkType.RrLeftCtxt, rr.LeftContext.Position, marks);
			addMarks(MarkType.Separator, rr.LeftContext.BeginSeparator, marks);
			foreach (var pat in rr.LeftContext) {
				Collect(pat, marks);
			}
			addMarks(MarkType.Separator, rr.LeftContext.EndSeparator, marks);

			addMarks(MarkType.RrPattern, rr.Pattern.Position, marks);
			Collect(rr.Pattern, marks);

			addMarks(MarkType.RrRightCtxt, rr.RightContext.Position, marks);
			addMarks(MarkType.Separator, rr.RightContext.BeginSeparator, marks);
			foreach (var pat in rr.RightContext) {
				Collect(pat, marks);
			}
			addMarks(MarkType.Separator, rr.RightContext.EndSeparator, marks);

			addMarks(MarkType.RrVariables, rr.LocalBindings.Position, marks);
			foreach (var v in rr.LocalBindings) {
				Collect(v, marks);
			}

			addMarks(MarkType.RrCondition, rr.Condition.Position, marks);
			Collect(rr.Condition, marks);

			//foreach (var sym in rr.Replacement) {
			//    Collect(sym, marks);
			//}

			//addMarks(MarkType.RrProbability, rr.Weight.Position, marks);
			//Collect(rr.Weight, marks);

			foreach (var kw in rr.Keywords) {
				Collect(kw, marks);
			}
		}

		public static void Collect(Ast.LsystemSymbol<Ast.Identificator> ptrn, List<PositionMark> marks) {
			addMarks(MarkType.SymbolPattern, ptrn.Position, marks);
			addMarks(MarkType.Symbol, ptrn.Position, marks);

			foreach (var name in ptrn.Arguments) {
				addMarks(MarkType.PatternVarName, name.Position, marks);
			}
		}

		public static void Collect(Ast.LsystemSymbol<Ast.Expression> symExpr, List<PositionMark> marks) {
			addMarks(MarkType.SymbolExpr, symExpr.Position, marks);
			addMarks(MarkType.Symbol, symExpr.Position, marks);

			foreach (var expr in symExpr.Arguments) {
				Collect(expr, marks);
			}
		}

		public static void Collect(Ast.KeywordPos kw, List<PositionMark> marks) {
			addMarks(MarkType.Keyword, kw.Position, marks);
		}

		public static void Collect(Ast.Expression expr, List<PositionMark> marks) {
			if (expr.IsEmpty) {
				return;
			}

			addMarks(MarkType.Expression, expr.Position, marks);

			foreach (var member in expr.Members) {
				switch (member.MemberType) {
					case Ast.ExpressionMemberType.Constant:
						addMarks(MarkType.ExprConstant, member.Position, marks);
						break;
					case Ast.ExpressionMemberType.Variable:
						addMarks(MarkType.ExprVariable, member.Position, marks);
						break;
					case Ast.ExpressionMemberType.Array:
						foreach (var e in ((Ast.ExpressionsArray)member)) {
							Collect(e, marks);
						}
						break;
					case Ast.ExpressionMemberType.Operator:
						break;
					case Ast.ExpressionMemberType.Indexer:
						Collect(((Ast.ExpressionIndexer)member).Index, marks);
						break;
					case Ast.ExpressionMemberType.Function:
						addMarks(MarkType.ExprFunName, ((Ast.ExpressionFunction)member).NameId.Position, marks);
						foreach (var e in ((Ast.ExpressionFunction)member).Arguments) {
							Collect(e, marks);
						}
						break;
					case Ast.ExpressionMemberType.BracketedExpression:
						Collect(((Ast.ExpressionBracketed)member).Expression, marks);
						break;
					default:
						break;
				}
			}
		}


		private static void addMarks(MarkType type, Position pos, List<PositionMark> marks) {
			if (pos.IsZeroLength) {
				return;
			}

			var begin = new PositionMark(type, pos.BeginLine, pos.BeginColumn, true);
			marks.Add(begin);
			marks.Add(new PositionMark(type, pos.EndLine, pos.EndColumn, false, begin.Generation));
		}

		private static void addMarks(MarkType type, Position pos, List<PositionMark> marks, object[] data) {
			if (pos.IsZeroLength) {
				return;
			}

			var begin = new PositionMark(type, pos.BeginLine, pos.BeginColumn, true);
			begin.Data = data;
			marks.Add(begin);
			marks.Add(new PositionMark(type, pos.EndLine, pos.EndColumn, false, begin.Generation));
		}
	}
}
*/