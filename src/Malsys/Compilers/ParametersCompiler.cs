﻿using Malsys.SemanticModel.Compiled;
using System.Collections.Generic;

namespace Malsys.Compilers {
	/// <remarks>
	/// All public members are thread safe if supplied compilers are thread safe.
	/// </remarks>
	public class ParametersCompiler : IParametersCompiler {

		protected readonly IExpressionCompiler exprCompiler;


		public ParametersCompiler(IExpressionCompiler expressionCompiler) {
			exprCompiler = expressionCompiler;
		}


		public OptionalParameter Compile(Ast.OptionalParameter optParam, IMessageLogger logger) {
			return new OptionalParameter(optParam.NameId.Name, exprCompiler.Compile(optParam.DefaultValue, logger), optParam);
		}

		public ImmutableList<OptionalParameter> CompileList(IList<Ast.OptionalParameter> parameters, IMessageLogger logger) {

			int parametersCount = parameters.Count;
			bool wasOptional = false;
			var result = new OptionalParameter[parametersCount];

			for (int i = 0; i < parametersCount; i++) {

				result[i] = Compile(parameters[i], logger);

				if (result[i].IsOptional) {
					wasOptional = true;
				}
				else if (wasOptional) {
					logger.LogMessage(Message.RequiredParamAfterOptional, parameters[i].Position, parameters[i].NameId.Name);
				}
			}

			// check whether parameters names are unique
			foreach (var indices in result.GetEqualValuesIndices((l, r) => { return l.Name.Equals(r.Name); })) {
				logger.LogMessage(Message.ParamNameNotUnique, parameters[indices.Item1].Position, result[indices.Item1].Name, parameters[indices.Item2].Position);
			}

			return new ImmutableList<OptionalParameter>(result, true);
		}


		public enum Message {

			[Message(MessageType.Error, "Optional parameters must appear after all required parameters, but required parameter `{0}` is after optional.")]
			RequiredParamAfterOptional,
			[Message(MessageType.Error, "Parameter name `{0}` is not unique. See also {1}.")]
			ParamNameNotUnique,

		}

	}
}
