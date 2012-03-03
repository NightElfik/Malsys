using Malsys.SemanticModel.Compiled;
using ConstsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;
using ConstsMapAst = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.Ast.ConstantDefinition>;
using FunsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Compiled.FunctionEvaledParams>;
using LsysMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Compiled.LsystemEvaledParams>;
using ProcConfsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Compiled.ProcessConfigurationStatement>;

namespace Malsys.SemanticModel.Evaluated {
	public class InputBlock {

		public readonly ConstsMap GlobalConstants;
		public readonly ConstsMapAst GlobalConstantsAstNodes;
		public readonly FunsMap GlobalFunctions;
		public readonly LsysMap Lsystems;
		public readonly ProcConfsMap ProcessConfigurations;
		public readonly ImmutableList<ProcessStatement> ProcessStatements;

		public string SourceName;


		public InputBlock(ConstsMap consts, ConstsMapAst constsAst, FunsMap funs, LsysMap lsystems,
				ProcConfsMap procConfigs, ImmutableList<ProcessStatement> procStats, string sourceName) {

			GlobalConstants = consts;
			GlobalConstantsAstNodes = constsAst;
			GlobalFunctions = funs;
			Lsystems = lsystems;
			ProcessConfigurations = procConfigs;
			ProcessStatements = procStats;

			SourceName = sourceName;
		}


		public InputBlock JoinWith(InputBlock inputBlock) {

			return new InputBlock(
				GlobalConstants.AddRange(inputBlock.GlobalConstants),
				GlobalConstantsAstNodes.AddRange(inputBlock.GlobalConstantsAstNodes),
				GlobalFunctions.AddRange(inputBlock.GlobalFunctions),
				Lsystems.AddRange(inputBlock.Lsystems),
				ProcessConfigurations.AddRange(inputBlock.ProcessConfigurations),
				ProcessStatements.AddRange(inputBlock.ProcessStatements),
				inputBlock.SourceName);

		}


	}
}
