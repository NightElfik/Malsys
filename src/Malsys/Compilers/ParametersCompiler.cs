using Malsys.SemanticModel.Compiled;
using System.Collections.Generic;

namespace Malsys.Compilers {
	internal class ParametersCompiler : IParametersCompiler {

		private MessageLogger msgs;
		private ExpressionCompiler exprCompiler;


		public ParametersCompiler(MessageLogger messageLogger, ExpressionCompiler expressionCompiler) {

			msgs = messageLogger;
			exprCompiler = expressionCompiler;
		}


		public OptionalParameter Compile(Ast.OptionalParameter optParam) {
			return new OptionalParameter(optParam.NameId.Name, exprCompiler.Compile(optParam.DefaultValue));
		}

		public ImmutableList<OptionalParameter> CompileList(IList<Ast.OptionalParameter> parameters) {

			int parametersCount = parameters.Count;
			bool wasOptional = false;
			var result = new OptionalParameter[parametersCount];

			for (int i = 0; i < parametersCount; i++) {

				result[i] = Compile(parameters[i]);

				if (result[i].IsOptional) {
					wasOptional = true;
				}
				else if (wasOptional) {
					msgs.LogMessage(Message.RequiredParamAfterOptional, parameters[i].Position, parameters[i].NameId.Name);
				}
			}

			// check whether parameters names are unique
			foreach (var indices in result.GetEqualValuesIndices((l, r) => { return l.Name.Equals(r.Name); })) {
				msgs.LogMessage(Message.ParamNameNotUnique, parameters[indices.Item1].Position, result[indices.Item1].Name, parameters[indices.Item2].Position);
			}

			return new ImmutableList<OptionalParameter>(result, true);
		}


		public enum Message {
			[Message(MessageType.Error, "Parameter name `{0}` is not unique. See also {1}.")]
			RequiredParamAfterOptional,
			[Message(MessageType.Error, "Optional parameters must appear after all required parameters, but required parameter `{0}` is after optional.")]
			ParamNameNotUnique,
		}

	}
}
