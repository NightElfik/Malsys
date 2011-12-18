using Malsys.Evaluators;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing {
	public class ProcessContext {

		public LsystemEvaled Lsystem { get; private set; }

		public IOutputProvider OutputProvider { get; private set; }

		public InputBlock InputData { get; private set; }

		public EvaluatorsContainer Evaluator { get; private set; }

		public IMessageLogger Logger { get; private set; }


		public ProcessContext(LsystemEvaled lsystem, IFilesManager filesManager,
				InputBlock data, EvaluatorsContainer evaluator, IMessageLogger logger) {

			Lsystem = lsystem;
			OutputProvider = filesManager;
			InputData = data;
			Evaluator = evaluator;
			Logger = logger;

			filesManager.CurrentLsystem = lsystem;

		}
	}
}
