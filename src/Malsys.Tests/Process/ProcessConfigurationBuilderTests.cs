using System;
using System.Linq;
using Malsys.Processing;
using Malsys.Processing.Components;
using Malsys.Processing.Output;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Process {
	[TestClass]
	public class ProcessConfigurationBuilderTests {

		[TestMethod]
		public void ComponentsOnlyTests() {
			doTest(string.Join("\n", "configuration Config {",
					"component Starter typeof DummyStarter;",
					"component Rewriter typeof DummyRewriter;",
					"component Processor typeof DummySymbolProcessor;",
					"connect Processor to Rewriter.SymbolProcessor;",
					" }"),
				"process this with Config;",
				new string[]{
					"DummyStarter",
					"DummySymbolProcessor",
					"DummyRewriter:DummySymbolProcessor" });
		}

		[TestMethod]
		public void ContainerTests() {
			doTest(string.Join("\n", "configuration Config {",
					"component Starter typeof DummyStarter;",
					"container Boy typeof IDummyContainer default DummyContaineredBoy;",
					"container Girl typeof IDummyContainer default DummyContaineredBoy;",
					"connect Girl to Boy.Component;",
					"connect Boy to Girl.Component;",
					" }"),
				"process this with Config use DummyContaineredGirl as Girl;",
				new string[] {
					"DummyStarter",
					"DummyContaineredBoy:DummyContaineredGirl",
					"DummyContaineredGirl:DummyContaineredBoy"});
		}

		[TestMethod]
		public void ComponentDoNotFitContainerTests() {
			doTest(string.Join("\n", "configuration Config {",
					"container Starter typeof IDummyContainer default DummyStarter;",
					" }"),
				"process this with Config;",
				new string[] {
					toId(ProcessConfigurationBuilder.Message.ComponentDontFitContainer)});

			doTest(string.Join("\n", "configuration Config {",
					"container Girl typeof IDummyContainer default DummyContaineredGirl;",
					" }"),
				"process this with Config use DummyStarter as Girl;",
				new string[] {
					toId(ProcessConfigurationBuilder.Message.ComponentDontFitContainer) });
		}

		[TestMethod]
		public void UnusedContainerAssocNameTests() {
			doTest(string.Join("\n", "configuration Config {",
					"container Starter typeof DummyStarter default DummyStarter;",
					" }"),
				"process this with Config use DummyStarter as InvalidName;",
				new string[] {
					"DummyStarter",
					toId(ProcessConfigurationBuilder.Message.ComponentAssignNotUsed) });
		}

		[TestMethod]
		public void UnknownComponentTests() {
			doTest(string.Join("\n", "configuration Config {",
					"component Unknown typeof UnknownType;",
					" }"),
				"process this with Config;",
				new string[] {
					toId(ProcessConfigurationBuilder.Message.ComponentResolveError) });
		}

		[TestMethod]
		public void UnknownContainerTests() {
			doTest(string.Join("\n", "configuration Config {",
					"container Unknown typeof Unknown default DummyContaineredBoy;",
					" }"),
				"process this with Config;",
				new string[] {
					toId(ProcessConfigurationBuilder.Message.ContainerResolveError) });
		}

		[TestMethod]
		public void NoStarterComponentTests() {
			doTest(string.Join("\n", "configuration Config {",
					"component Processor typeof DummySymbolProcessor;",
					" }"),
				"process this with Config;",
				new string[] {
					"DummySymbolProcessor",
					toId(ProcessConfigurationBuilder.Message.NoStartComponent) });
		}

		[TestMethod]
		public void MoreStarterComponentTests() {
			doTest(string.Join("\n", "configuration Config {",
					"component Starter1 typeof DummyStarter;",
					"component Starter2 typeof DummyStarter;",
					" }"),
				"process this with Config;",
				new string[] {
					"DummyStarter",
					"DummyStarter",
					toId(ProcessConfigurationBuilder.Message.MoreStartComponents) });
		}

		[TestMethod]
		public void OptionalConnectionTests() {
			doTest(string.Join("\n", "configuration Config {",
					"component GirlNoConn typeof DummyContaineredGirl;",
					"component Starter typeof DummyStarter;",
					" }"),
				"process this with Config;",
				new string[] {
					"DummyStarter",
					"DummyContaineredGirl:"});
		}

		[TestMethod]
		public void MandatoryConnectionTests() {
			doTest(string.Join("\n", "configuration Config {",
					"component BoyNoConn typeof DummyContaineredBoy;",
					"component Starter typeof DummyStarter;",
					" }"),
				"process this with Config;",
				new string[] {
					toId(ProcessConfigurationBuilder.Message.UnsetMandatoryConnection) });
		}

		[TestMethod]
		public void ExceptionInCtorTests() {
			doTest(string.Join("\n", "configuration Config {",
					"component Devil typeof ExceptionInCtor;",
					"component Starter typeof DummyStarter;",
					" }"),
				"process this with Config;",
				new string[] {
					toId(ProcessConfigurationBuilder.Message.ComponentCtorException) });
		}

		[TestMethod]
		public void ExceptionInInitTests() {
			doTest(string.Join("\n", "configuration Config {",
					"component GoodDevil typeof GoodExceptionInInit;",
					"component Starter typeof DummyStarter;",
					" }"),
				"process this with Config;",
				new string[] {
					"DummyStarter",
					toId(ProcessConfigurationBuilder.Message.ComponentInitializationError) });

			doTest(string.Join("\n", "configuration Config {",
					"component BadDevil typeof BadExceptionInInit;",
					"component Starter typeof DummyStarter;",
					" }"),
				"process this with Config;",
				new string[] {
					"DummyStarter",
					toId(ProcessConfigurationBuilder.Message.ComponentInitializationException) });
		}



		private string toId<TEnum>(TEnum enumValue) {
			return IMessageLoggerExtensions.EnumToMessageId(enumValue);
		}



		private void doTest(string configDefinition, string configStatement, string[] exceptedMessagesIds) {

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
			resolver.RegisterComponent(typeof(DummyStarter));
			resolver.RegisterComponent(typeof(ExceptionInCtor));
			resolver.RegisterComponent(typeof(GoodExceptionInInit));
			resolver.RegisterComponent(typeof(BadExceptionInInit));

			var logger = new MessageLogger();
			var ctxt = new ProcessContext(new LsystemEvaled("EmptyLsystem"), new FileOutputProvider("./"), input, null, logger);
			var processConfig = new ProcessConfigurationBuilder().BuildConfiguration(procConfig, procStat.ComponentAssignments, resolver, ctxt, logger);

			Console.WriteLine(logger.ToString());

			var actualMsgsIds = logger.Select(msg => msg.Id).ToList();

			CollectionAssert.AreEquivalent(exceptedMessagesIds, actualMsgsIds);
		}

		#region Components for tests

		private interface IDummyContainer {

			[UserConnectable]
			IComponent Component { set; }

		}

		private class DummyContaineredBoy : IDummyContainer, IComponent {

			[UserConnectable]
			public IComponent Component { get; set; }


			public void Initialize(ProcessContext context) {
				context.Logger.LogInfo(typeof(DummyContaineredBoy).Name + ":" + Component.GetType().Name, "");
			}

			public void Cleanup() { }

		}

		private class DummyContaineredGirl : IDummyContainer, IComponent {

			[UserConnectable(IsOptional = true)]
			public IComponent Component { get; set; }


			public void Initialize(ProcessContext context) {
				context.Logger.LogInfo(typeof(DummyContaineredGirl).Name + ":" + (Component != null ? Component.GetType().Name : ""), "");
			}

			public void Cleanup() { }

		}

		private class DummyRewriter : IComponent {

			[UserConnectable]
			public IComponent SymbolProcessor { get; set; }


			public void Initialize(ProcessContext context) {
				context.Logger.LogInfo(typeof(DummyRewriter).Name + ":" + SymbolProcessor.GetType().Name, "");
			}

			public void Cleanup() { }

		}

		private class DummySymbolProcessor : IComponent {


			public void Initialize(ProcessContext context) {
				context.Logger.LogInfo(typeof(DummySymbolProcessor).Name, "");
			}

			public void Cleanup() { }

		}

		private class DummyStarter : IProcessStarter {

			public void Initialize(ProcessContext context) {
				context.Logger.LogInfo(typeof(DummyStarter).Name, "");
			}

			public void Cleanup() { }


			#region IProcessStarter Members

			public void Start(bool doMeasure, TimeSpan timeout) {
				throw new NotImplementedException();
			}

			public void Abort() {
				throw new NotImplementedException();
			}

			#endregion
		}

		private class ExceptionInCtor : IComponent {

			public ExceptionInCtor() {
				throw new Exception("Something went wrong.");
			}


			public void Initialize(ProcessContext context) { }

			public void Cleanup() { }

		}

		private class GoodExceptionInInit : IComponent {

			public void Initialize(ProcessContext context) {
				throw new ComponentInitializationException("Something went wrong.");
			}

			public void Cleanup() { }

		}

		private class BadExceptionInInit : IComponent {

			public void Initialize(ProcessContext context) {
				throw new Exception("Something went wrong.");
			}

			public void Cleanup() { }

		}

		#endregion


	}


}
