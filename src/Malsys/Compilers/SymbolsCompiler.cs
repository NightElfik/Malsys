// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Collections.Generic;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Compilers {
	/// <remarks>
	/// All public members are thread safe if supplied compilers are thread safe.
	/// </remarks>
	public class SymbolsCompiler : ISymbolCompiler {

		protected readonly IExpressionCompiler exprCompiler;


		public SymbolsCompiler(IExpressionCompiler expressionCompiler) {
			exprCompiler = expressionCompiler;
		}


		Symbol<string> ICompiler<Ast.LsystemSymbol, Symbol<string>>.Compile(Ast.LsystemSymbol symbol, IMessageLogger logger) {

			List<string> names = new List<string>(symbol.Arguments.Count);

			for (int i = 0; i < symbol.Arguments.Count; i++) {
				if (symbol.Arguments[i].Members.Count != 1 || !(symbol.Arguments[i].Members[0] is Ast.Identifier)) {
					logger.LogMessage(Message.PatternParamCanBeOnlyId, symbol.Arguments[i].Position);
				}

				names.Add(((Ast.Identifier)symbol.Arguments[i].Members[0]).Name);

			}

			return new Symbol<string>(symbol) {
				Name = symbol.Name,
				Arguments = names,
			};
		}

		Symbol<IExpression> ICompiler<Ast.LsystemSymbol, Symbol<IExpression>>.Compile(Ast.LsystemSymbol symbol, IMessageLogger logger) {
			return new Symbol<IExpression>(symbol) {
				Name = symbol.Name,
				Arguments = exprCompiler.CompileList(symbol.Arguments, logger),
			};
		}



		public enum Message {

			[Message(MessageType.Error, "Parameters of symbol pattern can be only identifiers.")]
			PatternParamCanBeOnlyId,

		}

	}
}
