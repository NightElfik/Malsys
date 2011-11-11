using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.Processing.Components.Rewriters;
using Malsys.Processing.Components.RewriterIterators;
using Malsys.Processing.Components.Interpreters;
using Malsys.Processing.Components.Interpreters.TwoD;
using Malsys.Processing.Components.Renderers.TwoD;

namespace Malsys.Processing {
	public class RenderSetupManager {

		private ProcessContext context;

		private Action startAction;


		public void BuildRenderGraph() {

			var rewriter = new SymbolRewriter() { Context = context };
			var iterator = new SingleRewriterIterator();
			var caller = new InterpreterCaller();
			var interpreter = new Interpreter2D();
			var renderer = new SvgRenderer2D() { Context = context };

			rewriter.OutputProcessor = iterator;
			iterator.Rewriter = rewriter;
			iterator.OutputProcessor = caller;
			caller.Interpreter = interpreter;
			interpreter.Renderer = renderer;

			startAction = iterator.Start;
		}

	}
}
