using Malsys.Compilers;
using Malsys.Evaluators;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing {
	public class RenderManager {

		public RenderManager() {

		}


		public void RenderAllLsystemsDefault(string src, FilesManager fm, MessagesCollection msgs, bool svg) {

			var compiler = new InputCompiler(msgs);
			var inCompiled = compiler.CompileFromString(src, "webInput");

			if (msgs.ErrorOcured) {
				return;
			}

			var exprEvaluator = new ExpressionEvaluator();
			var evaluator = new InputEvaluator(exprEvaluator);
			var inBlockEvaled = evaluator.Evaluate(inCompiled);
			var lsysEvaluator = new LsystemEvaluator(exprEvaluator);

			foreach (var lsystemKvp in inBlockEvaled.Lsystems) {
				var lsysEvaled = lsysEvaluator.Evaluate(lsystemKvp.Value, ImmutableList<IValue>.Empty,
					inBlockEvaled.GlobalConstants, inBlockEvaled.GlobalFunctions);

				var context = new ProcessContext(lsysEvaled, fm, inBlockEvaled, exprEvaluator);

				var setupMgr = new RenderSetupManager(context);

				if (svg) {
					setupMgr.BuildSvgRenderModel();
				}
				else {
					setupMgr.BuildTextSymbolsModel();
				}

				setupMgr.starter.Start();

				setupMgr.ClearComponents();

				fm.TryDeleteAllTempFiles();
			}

		}

	}
}
