using Malsys.Evaluators;
using Malsys.SemanticModel.Evaluated;
using Malsys.Processing.Output;

namespace Malsys.Processing {
	public class ProcessContext {

		public LsystemEvaled Lsystem { get; private set; }

		public IOutputProvider OutputProvider { get; private set; }

		public InputBlock InputData { get; private set; }

		public EvaluatorsContainer Evaluator { get; private set; }

		public IMessageLogger Logger { get; private set; }


		public ProcessContext(LsystemEvaled lsystem, IOutputProvider outputProvider,
				InputBlock data, EvaluatorsContainer evaluator, IMessageLogger logger) {

			Lsystem = lsystem;
			OutputProvider = outputProvider;
			InputData = data;
			Evaluator = evaluator;
			Logger = logger;

		}
	}
}
