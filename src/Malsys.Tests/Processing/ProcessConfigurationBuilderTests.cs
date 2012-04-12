using System;
using System.Linq;
using Malsys.Processing;
using Malsys.Processing.Output;
using Malsys.SemanticModel.Evaluated;
using Microsoft.FSharp.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Malsys.SemanticModel.Compiled;
using Malsys.Reflection.Components;
using Malsys.Evaluators;

namespace Malsys.Tests.Process {
	[TestClass]
	public class ProcessConfigurationBuilderTests {

		[TestMethod]
		public void ComponentsOnlyTests() {
			doTest(string.Join("\n", "configuration Config {",
					"component Starter typeof StarterComponent;",
					"component ConnProp typeof ConnectablePropertyComponent;",
					"component Empty typeof EmptyComponent;",
					"connect Empty to ConnProp.Component;",
					"}"),
				"process all with Config;",
				new string[] {
					"StarterComponent",
					"EmptyComponent",
					"ConnectablePropertyComponent:EmptyComponent" });
		}

		[TestMethod]
		public void ContainerTests() {
			doTest(string.Join("\n", "configuration Config {",
					"component Starter typeof StarterComponent;",
					"container Alpha typeof IContainer default ContaineredAlphaComponent;",
					"container Beta typeof IContainer default ContaineredAlphaComponent;",
					"connect Beta to Alpha.Component;",
					"connect Alpha to Beta.Component;",
					"}"),
				"process all with Config use ContaineredBetaComponent as Beta;",
				new string[] {
					"StarterComponent",
					"ContaineredAlphaComponent:ContaineredBetaComponent",
					"ContaineredBetaComponent:ContaineredAlphaComponent" });
		}

		[TestMethod]
		public void InvalidConnectionSourceTests() {
		    doTest(string.Join("\n", "configuration Config {",
		            "component Starter typeof StarterComponent;",
		            "component ConnProp typeof ConnectablePropertyComponent;",
		            "component Empty typeof EmptyComponent;",
		            "virtual connect XXXX to ConnProp.Component;",
		            "}"),
		        "process all with Config;",
		        new string[] {
		            toId(ProcessConfigurationBuilder.Message.FailedToConnect)});
		}

		[TestMethod]
		public void InvalidConnectionDestinationTests() {
		    doTest(string.Join("\n", "configuration Config {",
		            "component Starter typeof StarterComponent;",
		            "component Empty typeof EmptyComponent;",
					"virtual connect Empty to XXX.Component;",
		            "}"),
		        "process all with Config;",
		        new string[] {
		            toId(ProcessConfigurationBuilder.Message.InvalidConnection) });
		}

		[TestMethod]
		public void InvalidConnectionPropertyTests() {
			doTest(string.Join("\n", "configuration Config {",
					"component Starter typeof StarterComponent;",
					"component ConnProp typeof ContaineredBetaComponent;",
					"component Empty typeof EmptyComponent;",
					"connect Empty to ConnProp.XXXX;",
					"}"),
				"process all with Config;",
				new string[] {
					toId(ProcessConfigurationBuilder.Message.InvalidConnection)});
		}

		[TestMethod]
		public void ComponentDoNotFitContainerTests() {
			doTest(string.Join("\n", "configuration Config {",
					"container Starter typeof IContainer default StarterComponent;",
					"}"),
				"process all with Config;",
				new string[] {
					toId(ProcessConfigurationBuilder.Message.ComponentDontFitContainer) });

			doTest(string.Join("\n", "configuration Config {",
					"container Beta typeof IContainer default ContaineredBetaComponent;",
					"}"),
				"process all with Config use StarterComponent as Beta;",
				new string[] {
					toId(ProcessConfigurationBuilder.Message.ComponentDontFitContainer) });
		}

		[TestMethod]
		public void UnusedContainerAssocNameTests() {
			doTest(string.Join("\n", "configuration Config {",
					"container Starter typeof StarterComponent default StarterComponent;",
					"}"),
				"process all with Config use StarterComponent as InvalidName;",
				new string[] {
					"StarterComponent",
					toId(ProcessConfigurationBuilder.Message.ComponentAssignNotUsed) });
		}

		[TestMethod]
		public void UnknownComponentTests() {
			doTest(string.Join("\n", "configuration Config {",
					"component Unknown typeof UnknownType;",
					"}"),
				"process all with Config;",
				new string[] {
					toId(ProcessConfigurationBuilder.Message.ComponentResolveError) });
		}

		[TestMethod]
		public void UnknownContainerTests() {
			doTest(string.Join("\n", "configuration Config {",
					"container Unknown typeof Unknown default ContaineredAlphaComponent;",
					"}"),
				"process all with Config;",
				new string[] {
					toId(ProcessConfigurationBuilder.Message.ContainerResolveError) });
		}

		[TestMethod]
		public void NoStarterComponentTests() {
			doTest(string.Join("\n", "configuration Config {",
					"component Empty typeof EmptyComponent;",
					"}"),
				"process all with Config;",
				new string[] {
					"EmptyComponent",
					toId(ProcessConfigurationBuilder.Message.NoStartComponent) });
		}

		[TestMethod]
		public void MoreStarterComponentTests() {
			doTest(string.Join("\n", "configuration Config {",
					"component Starter1 typeof StarterComponent;",
					"component Starter2 typeof StarterComponent;",
					"}"),
				"process all with Config;",
				new string[] {
					"StarterComponent",
					"StarterComponent",
					toId(ProcessConfigurationBuilder.Message.MoreStartComponents) });
		}

		[TestMethod]
		public void OptionalConnectionTests() {
			doTest(string.Join("\n", "configuration Config {",
					"component BetaNoConn typeof ContaineredBetaComponent;",
					"component Starter typeof StarterComponent;",
					"}"),
				"process all with Config;",
				new string[] {
					"StarterComponent",
					"ContaineredBetaComponent:" });
		}

		[TestMethod]
		public void MandatoryConnectionTests() {
			doTest(string.Join("\n", "configuration Config {",
					"component AlphaNoConn typeof ContaineredAlphaComponent;",
					"component Starter typeof StarterComponent;",
					"}"),
				"process all with Config;",
				new string[] {
					toId(ProcessConfigurationBuilder.Message.UnsetMandatoryConnection) });
		}

		[TestMethod]
		public void ExceptionInCtorTests() {
			doTest(string.Join("\n", "configuration Config {",
					"component Devil typeof ExceptionInCtorComponent;",
					"component Starter typeof StarterComponent;",
					"}"),
				"process all with Config;",
				new string[] {
					toId(ProcessConfigurationBuilder.Message.ComponentCtorException) });
		}

#if !DEBUG  // this test is testing throwing general exception which is not caught in DEBUG configuration
		[TestMethod]
		public void ExceptionInInitTests() {
			doTest(string.Join("\n", "configuration Config {",
					"component GoodDevil typeof ErrorInInitComponent;",
					"component Starter typeof StarterComponent;",
					"}"),
				"process all with Config;",
				new string[] {
					"StarterComponent",
					toId(ProcessConfigurationBuilder.Message.ComponentInitializationError) });

			doTest(string.Join("\n", "configuration Config {",
					"component BadDevil typeof ExceptionInInitComponent;",
					"component Starter typeof StarterComponent;",
					"}"),
				"process all with Config;",
				new string[] {
					"StarterComponent",
					toId(ProcessConfigurationBuilder.Message.ComponentInitializationException) });
		}
#endif

		[TestMethod]
		public void SettableVariableTests() {

			doTest(string.Join("\n", "lsystem l {",
					"set Constant = 8;",
					"}",
					"configuration Config {",
					"component Sett typeof SettablePropertiesComponent;",
					"component Starter typeof StarterComponent;",
					"}"),
				"process all with Config;",
				new string[] {
					"StarterComponent",
					"SettablePropertiesComponent:Constant=8,ValuesArray=,IValue=" });

			doTest(string.Join("\n", "lsystem l {",
					"set ValuesArray = {0,1,2,3};",
					"}",
					"configuration Config {",
					"component Sett typeof SettablePropertiesComponent;",
					"component Starter typeof StarterComponent;",
					"}"),
				"process all with Config;",
				new string[] {
					"StarterComponent",
					"SettablePropertiesComponent:Constant=,ValuesArray={0, 1, 2, 3},IValue=" });

			doTest(string.Join("\n", "lsystem l {",
					"set IValue = 27;",
					"}",
					"configuration Config {",
					"component Sett typeof SettablePropertiesComponent;",
					"component Starter typeof StarterComponent;",
					"}"),
				"process all with Config;",
				new string[] {
					"StarterComponent",
					"SettablePropertiesComponent:Constant=,ValuesArray=,IValue=27" });

			doTest(string.Join("\n", "lsystem l {",
					"set IValue = {2,7};",
					"}",
					"configuration Config {",
					"component Sett typeof SettablePropertiesComponent;",
					"component Starter typeof StarterComponent;",
					"}"),
				"process all with Config;",
				new string[] {
					"StarterComponent",
					"SettablePropertiesComponent:Constant=,ValuesArray=,IValue={2, 7}" });

		}

		[TestMethod]
		public void SettableVariableWrongTypesTests() {

			doTest(string.Join("\n", "lsystem l {",
					"set Constant = {0};",
					"}",
					"configuration Config {",
					"component Sett typeof SettablePropertiesComponent;",
					"component Starter typeof StarterComponent;",
					"}"),
				"process all with Config;",
				new string[] {
					"StarterComponent",
					"SettablePropertiesComponent:Constant=,ValuesArray=,IValue=",
					toId(ProcessConfigurationBuilder.Message.FailedToSetPropertyValueIncompatibleTypes)});

			doTest(string.Join("\n", "lsystem l {",
					 "set ValuesArray = 0;",
					 "}",
					 "configuration Config {",
					 "component Sett typeof SettablePropertiesComponent;",
					 "component Starter typeof StarterComponent;",
					 "}"),
				 "process all with Config;",
				 new string[] {
					"StarterComponent",
					"SettablePropertiesComponent:Constant=,ValuesArray=,IValue=",
					toId(ProcessConfigurationBuilder.Message.FailedToSetPropertyValueIncompatibleTypes)});

		}

		[TestMethod]
		public void SettableVariableAliasesTests() {

			doTest(string.Join("\n", "lsystem l {",
					"set IValue = 1;",
					"}",
					"configuration Config {",
					"component Sett typeof SettablePropertyAliasesComponent;",
					"component Starter typeof StarterComponent;",
					"}"),
				"process all with Config;",
				new string[] {
					"StarterComponent",
					"SettablePropertyAliasesComponent:1" });

			doTest(string.Join("\n", "lsystem l {",
					"set iValue = 1;",
					"}",
					"configuration Config {",
					"component Sett typeof SettablePropertyAliasesComponent;",
					"component Starter typeof StarterComponent;",
					"}"),
				"process all with Config;",
				new string[] {
					"StarterComponent",
					"SettablePropertyAliasesComponent:1" });

			doTest(string.Join("\n", "lsystem l {",
					"set A = 1;",
					"}",
					"configuration Config {",
					"component Sett typeof SettablePropertyAliasesComponent;",
					"component Starter typeof StarterComponent;",
					"}"),
				"process all with Config;",
				new string[] {
					"StarterComponent",
					"SettablePropertyAliasesComponent:1" });

			doTest(string.Join("\n", "lsystem l {",
					"set b = 1;",
					"}",
					"configuration Config {",
					"component Sett typeof SettablePropertyAliasesComponent;",
					"component Starter typeof StarterComponent;",
					"}"),
				"process all with Config;",
				new string[] {
					"StarterComponent",
					"SettablePropertyAliasesComponent:1" });

		}

		[TestMethod]
		public void MandatorySettableVariableTests() {

			doTest(string.Join("\n", "lsystem l {",
					"set Mandatory = 7;",
					"}",
					"configuration Config {",
					"component Sett typeof MandatorySettablePropertyComponent;",
					"component Starter typeof StarterComponent;",
					"}"),
				"process all with Config;",
				new string[] {
					"StarterComponent",
					"MandatorySettablePropertyComponent:7" });

			doTest(string.Join("\n", "lsystem l {",
					"}",
					"configuration Config {",
					"component Sett typeof MandatorySettablePropertyComponent;",
					"component Starter typeof StarterComponent;",
					"}"),
				"process all with Config;",
				new string[] {
					toId(ProcessConfigurationBuilder.Message.UnsetMandatoryProperty) });

		}

		[TestMethod]
		public void SettableVariableInvalidValueTests() {

			doTest(string.Join("\n", "lsystem l {",
					"set IValue = 7;",
					"}",
					"configuration Config {",
					"component Sett typeof SettablePropertyInvalidValueComponent;",
					"component Starter typeof StarterComponent;",
					"}"),
				"process all with Config;",
				new string[] {
					"StarterComponent",
					"SettablePropertyInvalidValueComponent",
					toId(ProcessConfigurationBuilder.Message.FailedToSetPropertyValue) });

		}

		[TestMethod]
		public void SettableSymbolsTests() {

			doTest(string.Join("\n", "lsystem l {",
				"set symbols Symbols = A B C;",
				"}",
				"configuration Config {",
				"component Sett typeof SettableSymbolPropertiesComponent;",
				"component Starter typeof StarterComponent;",
				"}"),
				"process all with Config;",
				new string[] {
					"StarterComponent",
					"SettableSymbolPropertiesComponent:A B C"} );

			doTest(string.Join("\n", "lsystem l {",
				 "set symbols Symbols = X(8,{1,2}) Y();",
				 "}",
				 "configuration Config {",
				 "component Sett typeof SettableSymbolPropertiesComponent;",
				 "component Starter typeof StarterComponent;",
				 "}"),
				 "process all with Config;",
				 new string[] {
					"StarterComponent",
					"SettableSymbolPropertiesComponent:X(8, {1, 2}) Y"} );

		}

		[TestMethod]
		public void MandatorySettableSymbolsTests() {

			doTest(string.Join("\n", "lsystem l {",
					"set symbols Mandatory = A B;",
					"}",
					"configuration Config {",
					"component Sett typeof MndatorySettableSymbolPropertiesComponent;",
					"component Starter typeof StarterComponent;",
					"}"),
				"process all with Config;",
				new string[] {
					"StarterComponent",
					"MndatorySettableSymbolPropertiesComponent:A B" });

			doTest(string.Join("\n", "lsystem l {",
					"}",
					"configuration Config {",
					"component Sett typeof MndatorySettableSymbolPropertiesComponent;",
					"component Starter typeof StarterComponent;",
					"}"),
				"process all with Config;",
				new string[] {
					toId(ProcessConfigurationBuilder.Message.UnsetMandatoryProperty)});

		}


		private string toId<TEnum>(TEnum enumValue) {
			return IMessageLoggerExtensions.EnumToMessageId(enumValue);
		}



		private void doTest(string configDefinition, string configStatement, string[] exceptedMessagesIds) {

			var input = TestUtils.EvaluateInput(configDefinition + configStatement);

			if (input.ProcessStatements.Length != 1) {
				Assert.Fail("Expected 1 process statement.");
			}

			var procStat = input.ProcessStatements[0];

			ProcessConfigurationStatement procConfig;
			if (!input.ProcessConfigurations.TryGetValue(procStat.ProcessConfiName, out procConfig)) {
				Assert.Fail("Configuration `{0}` not found.".Fmt(procStat.ProcessConfiName));
			}

			LsystemEvaled lsystem;
			if (input.Lsystems.Count == 1) {
				lsystem = TestUtils.EvaluateLsystem(input.Lsystems.First().Value);
			}
			else {
				lsystem = new LsystemEvaled("EmptyLsystem");
			}


			var resolver = new ComponentResolver();
			Components.RegisterAllComponents(resolver);

			var logger = new MessageLogger();

			var procCompBuilder = new ProcessConfigurationBuilder();
			var compGraph = procCompBuilder.BuildConfigurationComponentsGraph(procConfig, procStat.ComponentAssignments, resolver, logger);

			if (logger.ErrorOccurred) {
				goto results;
			}

			procCompBuilder.SetAndCheckUserSettableProperties(compGraph, lsystem.ComponentValuesAssigns, lsystem.ComponentSymbolsAssigns, logger);

			var ctxt = new ProcessContext(lsystem, new InMemoryOutputProvider(), input, new EvaluatorsContainer(lsystem.ExpressionEvaluatorContext),
				lsystem.ExpressionEvaluatorContext, resolver, TimeSpan.MaxValue, compGraph, logger);

			if (logger.ErrorOccurred) {
				goto results;
			}

			// we don't need to check output, components are logging state to logger
			procCompBuilder.InitializeComponents(compGraph, ctxt, logger);
			procCompBuilder.CreateConfiguration(compGraph, logger);


		results:

			Console.WriteLine(logger.ToString());

			var actualMsgsIds = logger.Select(msg => msg.Id).ToList();

			CollectionAssert.AreEquivalent(exceptedMessagesIds, actualMsgsIds);
		}




	}


}
