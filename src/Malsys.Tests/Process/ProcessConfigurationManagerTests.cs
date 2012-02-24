using System;
using System.Linq;
using Malsys.Processing;
using Malsys.Processing.Components;
using Microsoft.FSharp.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Malsys.Processing.Output;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Tests.Process {
	[TestClass]
	public class ProcessConfigurationManagerTests {
		[TestMethod]
		public void ComponentsOnlyTests() {
			doTest(string.Join("\n", "configuration Config {",
					"component Rewriter typeof DummyRewriter;",
					"component Processor typeof DummySymbolProcessor;",
					"connect Processor to Rewriter.SymbolProcessor;",
					" }"),
				"process this with Config;",
				new string[]{
					"DummySymbolProcessor:",
					"DummyRewriter:DummySymbolProcessor" });
		}

		[TestMethod]
		public void ContainerTests() {
			doTest(string.Join("\n", "configuration Config {",
					"container Boy typeof IDummyContainer default DummyContaineredBoy;",
					"container Girl typeof IDummyContainer default DummyContaineredBoy;",
					"connect Girl to Boy.Component;",
					"connect Boy to Girl.Component;",
					" }"),
				"process this with Config use DummyContaineredGirl as Girl;",
				new string[] {
					"DummyContaineredBoy:DummyContaineredGirl",
					"DummyContaineredGirl:DummyContaineredBoy"});
		}




		private void doTest(string configDefinition, string configStatement, string[] exceptedMessages) {

			var input = TestUtils.EvaluateLsystem(configDefinition + configStatement);

			if (input.ProcessStatements.Length != 1) {
				Assert.Fail("Expected 1 process statement.");
			}

			var procStat = input.ProcessStatements[0];
			var maybeConfig = input.ProcessConfigurations.TryFind(procStat.ProcessConfiName);
			if (OptionModule.IsNone(maybeConfig)) {
				Assert.Fail("Configuration `{0}` not found.".Fmt(procStat.ProcessConfiName));
			}

			var procConfig = maybeConfig.Value;

			var resolver = new ComponentResolver();
			resolver.RegisterComponent(typeof(DummyRewriter));
			resolver.RegisterComponent(typeof(DummySymbolProcessor));
			resolver.RegisterComponent(typeof(DummyContaineredBoy));
			resolver.RegisterComponent(typeof(DummyContaineredGirl));
			resolver.RegisterComponent(typeof(IDummyContainer));

			var logger = new MessageLogger();
			var ctxt = new ProcessContext(new LsystemEvaled("EmptyLsystem"), new FileOutputProvider("./"), input, null, logger);
			var manager = new ProcessConfigurationManager();
			if (!manager.TryBuildConfiguration(procConfig, procStat.ComponentAssignments, resolver, ctxt, new MessageLogger())) {
				Console.WriteLine(logger.ToString());
				Assert.Fail("Failed to build configuration.");
			}


			manager.ClearComponents();

			var actualMsgs = logger.Select(msg => msg.Id + ":" + msg.MessageStr).ToList();

			CollectionAssert.AreEquivalent(exceptedMessages, actualMsgs);
		}



		private interface IDummyContainer {

			[UserConnectable]
			IProcessComponent Component { set; }

		}

		private class DummyContaineredBoy : IDummyContainer, IProcessComponent {

			[UserConnectable]
			public IProcessComponent Component { get; set; }


			#region IComponent Members

			public bool RequiresMeasure { get { return false; } }

			public void Initialize(ProcessContext context) {
				context.Logger.LogError(typeof(DummyContaineredBoy).Name, Position.Unknown, Component.GetType().Name);
			}

			public void Cleanup() { }

			public void BeginProcessing(bool measuring) { throw new NotImplementedException(); }

			public void EndProcessing() { throw new NotImplementedException(); }

			#endregion
		}

		private class DummyContaineredGirl : IDummyContainer, IProcessComponent {

			[UserConnectable]
			public IProcessComponent Component { get; set; }


			#region IComponent Members

			public bool RequiresMeasure { get { return false; } }

			public void Initialize(ProcessContext context) {
				context.Logger.LogError(typeof(DummyContaineredGirl).Name, Position.Unknown, Component.GetType().Name);
			}

			public void Cleanup() { }

			public void BeginProcessing(bool measuring) { throw new NotImplementedException(); }

			public void EndProcessing() { throw new NotImplementedException(); }

			#endregion
		}

		private class DummyRewriter : IProcessComponent {

			[UserConnectable]
			public IProcessComponent SymbolProcessor { get; set; }


			#region IComponent Members

			public bool RequiresMeasure { get { return false; } }

			public void Initialize(ProcessContext context) {
				context.Logger.LogError(typeof(DummyRewriter).Name, Position.Unknown, SymbolProcessor.GetType().Name);
			}

			public void Cleanup() { }

			public void BeginProcessing(bool measuring) { throw new NotImplementedException(); }

			public void EndProcessing() { throw new NotImplementedException(); }

			#endregion
		}

		private class DummySymbolProcessor : IProcessComponent {

			#region IComponent Members

			public bool RequiresMeasure { get { return false; } }

			public void Initialize(ProcessContext context) {
				context.Logger.LogError(typeof(DummySymbolProcessor).Name, Position.Unknown, "");
			}

			public void Cleanup() { }

			public void BeginProcessing(bool measuring) { throw new NotImplementedException(); }

			public void EndProcessing() { throw new NotImplementedException(); }

			#endregion
		}
	}


}
