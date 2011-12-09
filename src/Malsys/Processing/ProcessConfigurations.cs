using Malsys.Processing.Components.Common;
using Malsys.Processing.Components.RewriterIterators;
using Malsys.Processing.Components.Rewriters;
using Malsys.SemanticModel.Compiled;
using Malsys.Processing.Components.Interpreters;
using Malsys.Processing.Components.Interpreters.TwoD;
using Malsys.Processing.Components;
using Malsys.Processing.Components.Renderers.TwoD;

namespace Malsys.Processing {
	public static class ProcessConfigurations {

		public static readonly ProcessConfiguration PrintSymbolsConfig = new ProcessConfiguration(
			"PrintSymbolsConfig",
			new ImmutableList<ProcessComponent>(
				new ProcessComponent("Interpret", typeof(SymbolsSaver).FullName),
				new ProcessComponent("Rewriter", typeof(SymbolRewriter).FullName),
				new ProcessComponent("Iterator", typeof(SingleRewriterIterator).FullName)
			),
			ImmutableList<ProcessContainer>.Empty,
			new ImmutableList<ProcessComponentsConnection>(
				new ProcessComponentsConnection("Iterator", "Rewriter", "OutputProcessor"),
				new ProcessComponentsConnection("Rewriter", "Iterator", "Rewriter"),
				new ProcessComponentsConnection("Interpret", "Iterator", "OutputProcessor")
			)
		);


		public static readonly ProcessConfiguration InterpretConfig = new ProcessConfiguration(
			"InterpretConfig",
			new ImmutableList<ProcessComponent>(
				new ProcessComponent("Rewriter", typeof(SymbolRewriter).FullName),
				new ProcessComponent("Iterator", typeof(SingleRewriterIterator).FullName),
				new ProcessComponent("InterpreterCaller", typeof(InterpreterCaller).FullName)
			),
			new ImmutableList<ProcessContainer>(
				new ProcessContainer("Interpreter", typeof(IInterpreter).FullName, typeof(Interpreter2D).FullName),
				new ProcessContainer("Renderer", typeof(IRenderer).FullName, typeof(SvgRenderer2D).FullName)
			),
			new ImmutableList<ProcessComponentsConnection>(
				new ProcessComponentsConnection("Iterator", "Rewriter", "OutputProcessor"),
				new ProcessComponentsConnection("Rewriter", "Iterator", "Rewriter"),
				new ProcessComponentsConnection("InterpreterCaller", "Iterator", "OutputProcessor"),
				new ProcessComponentsConnection("Interpreter", "InterpreterCaller", "Interpreter"),
				new ProcessComponentsConnection("Renderer", "Interpreter", "Renderer")
			)
		);

	}
}
