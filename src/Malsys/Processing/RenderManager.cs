using Malsys.Compilers;
using Malsys.Evaluators;
using Malsys.SemanticModel.Evaluated;
using System;

namespace Malsys.Processing {
	public class RenderManager {

		public TimeSpan Timeout { get; set; }

		public RenderManager() {
			Timeout = new TimeSpan(0, 0, 20);
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

				var context = new ProcessContext(lsysEvaled, fm, inBlockEvaled, exprEvaluator, msgs);

				var setupMgr = new RenderSetupManager();

				if (svg) {
					setupMgr.BuildSvgRenderModel();
				}
				else {
					setupMgr.BuildTextSymbolsModel();
				}

				setupMgr.Initialize(context);

				setupMgr.starter.Start(setupMgr.RequiresMeasure, Timeout);

				setupMgr.Cleanup();
				setupMgr.ClearComponents();

				fm.TryDeleteAllTempFiles();
			}

		}

	}
}
