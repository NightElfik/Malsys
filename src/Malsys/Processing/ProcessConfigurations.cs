using Malsys.Processing.Components.Common;
using Malsys.Processing.Components.RewriterIterators;
using Malsys.Processing.Components.Rewriters;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Processing {
	public static class ProcessConfigurations {

		public static readonly ProcessConfiguration PrintSymbolsConfig = new ProcessConfiguration(
			"PrintSymbolsConfig",
			new ImmutableList<ProcessComponent>(
				new ProcessComponent("Interpret", typeof(SymbolsSaver).FullName),
				new ProcessComponent("Rewriter", typeof(SymbolRewriter).FullName),
				new ProcessComponent("Iterator", typeof(SingleRewriterIterator).FullName)),
			ImmutableList<ProcessContainer>.Empty,
			new ImmutableList<ProcessComponentsConnection>(
				new ProcessComponentsConnection("Iterator", "Rewriter", "OutputProcessor"),
				new ProcessComponentsConnection("Rewriter", "Iterator", "Rewriter"),
				new ProcessComponentsConnection("Interpret", "Iterator", "OutputProcessor")));

	}
}
