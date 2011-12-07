using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	internal class SymbolsCompiler : ISymbolCompiler {

		private IExpressionCompiler exprCompiler;


		public SymbolsCompiler(IExpressionCompiler expressionCompiler) {
			exprCompiler = expressionCompiler;
		}


		Symbol<VoidStruct> ICompiler<Ast.LsystemSymbol, Symbol<VoidStruct>>.Compile(Ast.LsystemSymbol symbol, IMessageLogger logger) {

			for (int i = 0; i < symbol.Arguments.Length; i++) {
				if (symbol.Arguments[i].Members.Length != 0) {
					logger.LogMessage(Message.SymbolsParamsNotAllowed, symbol.Arguments[i].Position, symbol.Name);
				}
			}

			return new Symbol<VoidStruct>(symbol.Name, ImmutableList<VoidStruct>.Empty, symbol);
		}

		Symbol<string> ICompiler<Ast.LsystemSymbol, Symbol<string>>.Compile(Ast.LsystemSymbol symbol, IMessageLogger logger) {

			string[] names = new string[symbol.Arguments.Length];

			for (int i = 0; i < symbol.Arguments.Length; i++) {
					if (symbol.Arguments[i].Members.Length != 1 || !(symbol.Arguments[i].Members[0] is Ast.Identificator)) {
						logger.LogMessage(Message.PatternParamCanBeOnlyId, symbol.Arguments[i].Position);
					}

					names[i] = ((Ast.Identificator)symbol.Arguments[i].Members[0]).Name;

			}

			var namesImm = new ImmutableList<string>(names, true);
			return new Symbol<string>(symbol.Name, namesImm, symbol);
		}

		Symbol<IExpression> ICompiler<Ast.LsystemSymbol, Symbol<IExpression>>.Compile(Ast.LsystemSymbol symbol, IMessageLogger logger) {
			return new Symbol<IExpression>(symbol.Name, exprCompiler.CompileList(symbol.Arguments, logger), symbol);
		}



		public enum Message {

			[Message(MessageType.Error, "Parameters of symbol pattern can be only identifiers.")]
			PatternParamCanBeOnlyId,
			[Message(MessageType.Error, "Parameters of symbol are not allowed in this context.")]
			SymbolsParamsNotAllowed,

		}

	}
}
