using System.Collections.Generic;
using System.Linq;
using Malsys.SemanticModel.Compiled;

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
			return new OptionalParameter(optParam) {
				Name = optParam.NameId.Name,
				DefaultValue = exprCompiler.Compile(optParam.DefaultValue, logger),
			};
		}

		public List<OptionalParameter> CompileList(IEnumerable<Ast.OptionalParameter> parameters, IMessageLogger logger) {

			bool wasOptional = false;
			var result = parameters.Select(param => {
				var p = Compile(param, logger);
				if (p.IsOptional) {
					wasOptional = true;
				}
				else if (wasOptional) {
					logger.LogMessage(Message.RequiredParamAfterOptional, param.Position, param.NameId.Name);
				}
				return p;
			}).ToList();


			// Check whether parameters names are unique.
			foreach (var values in result.GetValuePairs((l, r) => { return l.Name.Equals(r.Name); })) {
				logger.LogMessage(Message.ParamNameNotUnique, values.Item1.AstNode.Position, values.Item1.Name, values.Item1.AstNode.Position);
			}

			return result;
		}


		public enum Message {

			[Message(MessageType.Error, "Optional parameters must appear after all required parameters, but required parameter `{0}` is after optional.")]
			RequiredParamAfterOptional,
			[Message(MessageType.Error, "Parameter name `{0}` is not unique. See also {1}.")]
			ParamNameNotUnique,

		}

	}
}
