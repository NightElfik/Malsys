using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.Processing.Components.Rewriters;
using Malsys.Processing.Components.RewriterIterators;
using Malsys.Processing.Components.Interpreters;
using Malsys.Processing.Components.Interpreters.TwoD;
using Malsys.Processing.Components.Renderers.TwoD;
using Malsys.Processing.Components;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Core;
using Malsys.SemanticModel;
using Malsys.Processing.Components.Common;

namespace Malsys.Processing {
	public class RenderSetupManager {

		private ProcessContext context;

		public IProcessStarter starter { get; private set; }

		private List<IComponent> components = new List<IComponent>();


		public RenderSetupManager(ProcessContext ctxt) {
			context = ctxt;
		}


		public void BuildSvgRenderModel() {

			components.Clear();

			var rewriter = new SymbolRewriter();
			components.Add(rewriter);
			var iterator = new SingleRewriterIterator();
			components.Add(iterator);
			var caller = new InterpreterCaller();
			components.Add(caller);
			var interpreter = new Interpreter2D();
			components.Add(interpreter);
			var renderer = new SvgRenderer2D();
			components.Add(renderer);

			rewriter.OutputProcessor = iterator;
			iterator.Rewriter = rewriter;
			iterator.OutputProcessor = caller;
			caller.Interpreter = interpreter;
			interpreter.Renderer = renderer;

			starter = iterator;

			foreach (var cp in components) {
				cp.Context = context;
				setUserSettableProperties(cp, context);
			}
		}

		public void BuildTextSymbolsModel() {

			components.Clear();

			var rewriter = new SymbolRewriter();
			components.Add(rewriter);
			var iterator = new SingleRewriterIterator();
			components.Add(iterator);
			var saver = new SymbolsSaver();
			components.Add(saver);

			rewriter.OutputProcessor = iterator;
			iterator.Rewriter = rewriter;
			iterator.OutputProcessor = saver;

			starter = iterator;

			foreach (var cp in components) {
				cp.Context = context;
				setUserSettableProperties(cp, context);
			}
		}

		public void ClearComponents() {
			components.Clear();
		}

		private void setUserSettableProperties(IComponent component, ProcessContext ctxt) {

			foreach (var propInfo in component.GetType().GetProperties()) {

				if (propInfo.GetCustomAttributes(typeof(UserSettableAttribute), true).Length != 1) {
					continue;
				}

				if (propInfo.PropertyType.Equals(typeof(IValue))) {
					var maybeConst = ctxt.Lsystem.Constants.TryFind(propInfo.Name.ToLowerInvariant());
					if (OptionModule.IsSome(maybeConst)) {
						propInfo.SetValue(component, maybeConst.Value, null);
					}
				}
				else if (propInfo.PropertyType.Equals(typeof(ImmutableList<Symbol<IValue>>))) {
					var maybeSyms = ctxt.Lsystem.SymbolsConstants.TryFind(propInfo.Name.ToLowerInvariant());
					if (OptionModule.IsSome(maybeSyms)) {
						propInfo.SetValue(component, maybeSyms.Value, null);
					}
				}

			}

		}

	}
}
