// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using Malsys.Processing;
using Malsys.Processing.Components;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Tests {
	internal class Components {

		public static void RegisterAllComponents(IComponentMetadataContainer container) {

			var logger = new MessageLogger();

			container.RegisterComponent(typeof(EmptyComponent), logger);
			container.RegisterComponent(typeof(ConnectablePropertyComponent), logger);

			container.RegisterComponent(typeof(StarterComponent), logger);

			container.RegisterComponent(typeof(IContainer), logger);

			container.RegisterComponent(typeof(ContaineredAlphaComponent), logger);
			container.RegisterComponent(typeof(ContaineredBetaComponent), logger);

			//container.RegisterComponent(typeof(NoParamlessCtorComponent), logger);
			container.RegisterComponent(typeof(ExceptionInCtorComponent), logger);
			container.RegisterComponent(typeof(ErrorInInitComponent), logger);
			container.RegisterComponent(typeof(ExceptionInInitComponent), logger);

			container.RegisterComponent(typeof(SettablePropertiesComponent), logger);
			container.RegisterComponent(typeof(SettablePropertyAliasesComponent), logger);
			container.RegisterComponent(typeof(SettablePropertyInvalidValueComponent), logger);
			container.RegisterComponent(typeof(MandatorySettablePropertyComponent), logger);
			container.RegisterComponent(typeof(SettableSymbolPropertiesComponent), logger);
			container.RegisterComponent(typeof(MndatorySettableSymbolPropertiesComponent), logger);

			container.RegisterComponent(typeof(GettablePropertiesComponent), logger);

			if (logger.ErrorOccurred) {
				throw new Exception("Failed to register components. " + logger.AllMessagesToFullString());
			}

		}


		public class EmptyComponent : IComponent {

			public IMessageLogger Logger { get; set; }

			public void Reset() { }

			public void Initialize(ProcessContext context) {
				Logger.LogInfo(this.GetType().Name, "");
			}

			public void Cleanup() { }

			public void Dispose() { }

		}

		public class ConnectablePropertyComponent : IComponent {

			[UserConnectable]
			public IComponent Component { get; set; }


			public IMessageLogger Logger { get; set; }

			public void Reset() { }

			public void Initialize(ProcessContext context) {
				Logger.LogInfo(this.GetType().Name + ":" + Component.GetType().Name, "");
			}

			public void Cleanup() { }

			public void Dispose() { }

		}


		public class StarterComponent : IProcessStarter {

			public IMessageLogger Logger { get; set; }

			public void Reset() { }

			public void Initialize(ProcessContext context) {
				Logger.LogInfo(this.GetType().Name, "");
			}

			public void Cleanup() { }

			public void Dispose() { }


			#region IProcessStarter Members

			public void Start(bool doMeasure) { }

			public void Abort() { }

			#endregion IProcessStarter Members
		}


		public interface IContainer : IComponent {

			[UserConnectable]
			IComponent Component { set; }

		}


		public class ContaineredAlphaComponent : IContainer, IComponent {

			[UserConnectable]
			public IComponent Component { get; set; }


			public IMessageLogger Logger { get; set; }

			public void Reset() { }

			public void Initialize(ProcessContext context) {
				Logger.LogInfo(this.GetType().Name + ":" + Component.GetType().Name, "");
			}

			public void Cleanup() { }

			public void Dispose() { }

		}

		public class ContaineredBetaComponent : IContainer, IComponent {

			[UserConnectable(IsOptional = true)]
			public IComponent Component { get; set; }


			public IMessageLogger Logger { get; set; }

			public void Reset() { }

			public void Initialize(ProcessContext context) {
				Logger.LogInfo(this.GetType().Name + ":" + (Component != null ? Component.GetType().Name : ""), "");
			}

			public void Cleanup() { }

			public void Dispose() { }

		}


		public class NoParamlessCtorComponent : IComponent, IContainer {


			public NoParamlessCtorComponent(string something, int weird) { }


			public IMessageLogger Logger { get; set; }

			public void Reset() { }

			public void Initialize(ProcessContext context) {
				Logger.LogInfo(this.GetType().Name, "");
			}

			public void Cleanup() { }

			public void Dispose() { }


			#region IContainer Members

			public IComponent Component { get; set; }

			#endregion IContainer Members
		}

		public class ExceptionInCtorComponent : IComponent {

			public ExceptionInCtorComponent() {
				throw new Exception("Something went wrong.");
			}


			public IMessageLogger Logger { get; set; }

			public void Reset() { }

			public void Initialize(ProcessContext context) { }

			public void Cleanup() { }

			public void Dispose() { }

		}

		public class ErrorInInitComponent : IComponent {

			public IMessageLogger Logger { get; set; }

			public void Reset() { }

			public void Initialize(ProcessContext context) {
				throw new ComponentException("Something went wrong.");
			}

			public void Cleanup() { }

			public void Dispose() { }

		}

		public class ExceptionInInitComponent : IComponent {

			public IMessageLogger Logger { get; set; }

			public void Reset() { }

			public void Initialize(ProcessContext context) {
				throw new Exception("Something went wrong.");
			}

			public void Cleanup() { }

			public void Dispose() { }

		}


		public class SettablePropertiesComponent : IComponent {

			[UserSettable]
			public Constant Constant { private get; set; }

			[UserSettable]
			public ValuesArray ValuesArray { private get; set; }

			[UserSettable]
			public IValue IValue { private get; set; }


			public IMessageLogger Logger { get; set; }

			public void Reset() { }

			public void Initialize(ProcessContext context) {
				Logger.LogInfo(this.GetType().Name
					+ ":Constant=" + (Constant != null ? TestUtils.Print(Constant) : "")
					+ ",ValuesArray=" + (ValuesArray != null ? TestUtils.Print(ValuesArray) : "")
					+ ",IValue=" + (IValue != null ? TestUtils.Print(IValue) : ""), "");
			}

			public void Cleanup() { }

			public void Dispose() { }

		}

		public class SettablePropertyAliasesComponent : IComponent {

			[AccessName("IValue", "iValue", "A", "b")]
			[UserSettable]
			public IValue IValue { private get; set; }


			public IMessageLogger Logger { get; set; }

			public void Reset() { }

			public void Initialize(ProcessContext context) {
				Logger.LogInfo(this.GetType().Name
					+ ":" + (IValue != null ? TestUtils.Print(IValue) : ""), "");
			}

			public void Cleanup() { }

			public void Dispose() { }

		}

		public class SettablePropertyInvalidValueComponent : IComponent {

			[UserSettable]
			public IValue IValue { set { throw new InvalidUserValueException(); } }


			public IMessageLogger Logger { get; set; }

			public void Reset() { }

			public void Initialize(ProcessContext context) {
				Logger.LogInfo(this.GetType().Name, "");
			}

			public void Cleanup() { }

			public void Dispose() { }

		}

		public class MandatorySettablePropertyComponent : IComponent {

			[UserSettable(IsMandatory = true)]
			public IValue Mandatory { private get; set; }


			public IMessageLogger Logger { get; set; }

			public void Reset() { }

			public void Initialize(ProcessContext context) {
				Logger.LogInfo(this.GetType().Name
					+ ":" + (Mandatory != null ? TestUtils.Print(Mandatory) : ""), "");
			}

			public void Cleanup() { }

			public void Dispose() { }

		}

		public class SettableSymbolPropertiesComponent : IComponent {

			[UserSettableSybols]
			public ImmutableList<Symbol<IValue>> Symbols { private get; set; }


			public IMessageLogger Logger { get; set; }

			public void Reset() { }

			public void Initialize(ProcessContext context) {
				Logger.LogInfo(this.GetType().Name
					+ ":" + (Symbols != null ? TestUtils.Print(Symbols) : ""), "");
			}

			public void Cleanup() { }

			public void Dispose() { }

		}

		public class MndatorySettableSymbolPropertiesComponent : IComponent {

			[UserSettableSybols(IsMandatory = true)]
			public ImmutableList<Symbol<IValue>> Mandatory { private get; set; }


			public IMessageLogger Logger { get; set; }

			public void Reset() { }

			public void Initialize(ProcessContext context) {
				Logger.LogInfo(this.GetType().Name
					+ ":" + (Mandatory != null ? TestUtils.Print(Mandatory) : ""), "");
			}

			public void Cleanup() { }

			public void Dispose() { }

		}

		public class GettablePropertiesComponent : IComponent {

			[UserGettable(IsGettableBeforeInitialiation = true)]
			public Constant ConstantGet { get { return 8.ToConst(); } }

			[UserGettable(IsGettableBeforeInitialiation = true)]
			public ValuesArray ValuesArrayGet { get { return new ValuesArray(new ImmutableList<IValue>(1.ToConst(), 2.ToConst(), 3.ToConst())); } }

			[UserGettable(IsGettableBeforeInitialiation = true)]
			public IValue IValueGet { get { return 42.ToConst(); } }


			public IMessageLogger Logger { get; set; }

			public void Reset() { }

			public void Initialize(ProcessContext context) {
				Logger.LogInfo(this.GetType().Name, "");
			}

			public void Cleanup() { }

			public void Dispose() { }

		}

	}
}
