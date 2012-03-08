using Malsys.Evaluators;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing {
	public class ProcessContext {

		public LsystemEvaled Lsystem { get; private set; }

		public IOutputProvider OutputProvider { get; private set; }

		public InputBlockEvaled InputData { get; private set; }

		public IExpressionEvaluatorContext ExpressionEvaluatorContext { get; private set; }

		public IMessageLogger Logger { get; private set; }


		public ProcessContext(LsystemEvaled lsystem, IOutputProvider outputProvider,
				InputBlockEvaled data, IExpressionEvaluatorContext exprEvalCtxt, IMessageLogger logger) {

			Lsystem = lsystem;
			OutputProvider = outputProvider;
			InputData = data;
			ExpressionEvaluatorContext = exprEvalCtxt;
			Logger = logger;

		}
	}
}
