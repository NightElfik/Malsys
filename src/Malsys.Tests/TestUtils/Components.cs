using System;
using Malsys.Processing;
using Malsys.Processing.Components;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Tests {
	internal class Components {

		public static void RegisterAllComponents(IComponentContainer container) {

			container.RegisterComponent(typeof(EmptyComponent));
			container.RegisterComponent(typeof(ConnectablePropertyComponent));

			container.RegisterComponent(typeof(StarterComponent));

			container.RegisterComponent(typeof(IContainer));

			container.RegisterComponent(typeof(ContaineredAlphaComponent));
			container.RegisterComponent(typeof(ContaineredBetaComponent));

			container.RegisterComponent(typeof(NoParamlessCtorComponent));
			container.RegisterComponent(typeof(ExceptionInCtorComponent));
			container.RegisterComponent(typeof(ErrorInInitComponent));
			container.RegisterComponent(typeof(ExceptionInInitComponent));

			container.RegisterComponent(typeof(SettablePropertiesComponent));
			container.RegisterComponent(typeof(SettablePropertyAliasesComponent));
			container.RegisterComponent(typeof(SettablePropertyInvalidValueComponent));
			container.RegisterComponent(typeof(MandatorySettablePropertyComponent));
			container.RegisterComponent(typeof(SettableSymbolPropertiesComponent));
			container.RegisterComponent(typeof(MndatorySettableSymbolPropertiesComponent));

			container.RegisterComponent(typeof(GettablePropertiesComponent));

		}


		public class EmptyComponent : IComponent {

			public void Initialize(ProcessContext context) {
				context.Logger.LogInfo(this.GetType().Name, "");
			}

			public void Cleanup() { }

		}

		public class ConnectablePropertyComponent : IComponent {

			[UserConnectable]
			public IComponent Component { get; set; }


			public void Initialize(ProcessContext context) {
				context.Logger.LogInfo(this.GetType().Name + ":" + Component.GetType().Name, "");
			}

			public void Cleanup() { }

		}


		public class StarterComponent : IProcessStarter {

			public void Initialize(ProcessContext context) {
				context.Logger.LogInfo(this.GetType().Name, "");
			}

			public void Cleanup() { }


			#region IProcessStarter Members

			public void Start(bool doMeasure, TimeSpan timeout) { }

			public void Abort() { }

			#endregion
		}


		public interface IContainer {

			[UserConnectable]
			IComponent Component { set; }

		}


		public class ContaineredAlphaComponent : IContainer, IComponent {

			[UserConnectable]
			public IComponent Component { get; set; }


			public void Initialize(ProcessContext context) {
				context.Logger.LogInfo(this.GetType().Name + ":" + Component.GetType().Name, "");
			}

			public void Cleanup() { }

		}

		public class ContaineredBetaComponent : IContainer, IComponent {

			[UserConnectable(IsOptional = true)]
			public IComponent Component { get; set; }


			public void Initialize(ProcessContext context) {
				context.Logger.LogInfo(this.GetType().Name + ":" + (Component != null ? Component.GetType().Name : ""), "");
			}

			public void Cleanup() { }

		}


		public class NoParamlessCtorComponent : IComponent, IContainer {


			public NoParamlessCtorComponent(string something, int weird) { }


			public void Initialize(ProcessContext context) {
				context.Logger.LogInfo(this.GetType().Name, "");
			}

			public void Cleanup() { }


			#region IContainer Members

			public IComponent Component { get; set; }

			#endregion
		}

		public class ExceptionInCtorComponent : IComponent {

			public ExceptionInCtorComponent() {
				throw new Exception("Something went wrong.");
			}


			public void Initialize(ProcessContext context) { }

			public void Cleanup() { }

		}

		public class ErrorInInitComponent : IComponent {

			public void Initialize(ProcessContext context) {
				throw new ComponentInitializationException("Something went wrong.");
			}

			public void Cleanup() { }

		}

		public class ExceptionInInitComponent : IComponent {

			public void Initialize(ProcessContext context) {
				throw new Exception("Something went wrong.");
			}

			public void Cleanup() { }

		}


		public class SettablePropertiesComponent : IComponent {

			[UserSettable]
			public Constant Constant { private get; set; }

			[UserSettable]
			public ValuesArray ValuesArray { private get; set; }

			[UserSettable]
			public IValue IValue { private get; set; }


			public void Initialize(ProcessContext context) {
				context.Logger.LogInfo(this.GetType().Name
					+ ":Constant=" + (Constant != null ? TestUtils.Print(Constant) : "")
					+ ",ValuesArray=" + (ValuesArray != null ? TestUtils.Print(ValuesArray) : "")
					+ ",IValue=" + (IValue != null ? TestUtils.Print(IValue) : ""), "");
			}

			public void Cleanup() { }

		}

		public class SettablePropertyAliasesComponent : IComponent {

			[Alias("iValue", "A", "b")]
			[UserSettable]
			public IValue IValue { private get; set; }


			public void Initialize(ProcessContext context) {
				context.Logger.LogInfo(this.GetType().Name
					+ ":" + (IValue != null ? TestUtils.Print(IValue) : ""), "");
			}

			public void Cleanup() { }

		}

		public class SettablePropertyInvalidValueComponent : IComponent {

			[UserSettable]
			public IValue IValue { set { throw new InvalidUserValueException(); } }


			public void Initialize(ProcessContext context) {
				context.Logger.LogInfo(this.GetType().Name, "");
			}

			public void Cleanup() { }

		}

		public class MandatorySettablePropertyComponent : IComponent {

			[UserSettable(IsMandatory = true)]
			public IValue Mandatory { private get; set; }

			public void Initialize(ProcessContext context) {
				context.Logger.LogInfo(this.GetType().Name
					+ ":" + (Mandatory != null ? TestUtils.Print(Mandatory) : ""), "");
			}

			public void Cleanup() { }

		}

		public class SettableSymbolPropertiesComponent : IComponent {

			[UserSettableSybols]
			public ImmutableList<Symbol<IValue>> Symbols { private get; set; }


			public void Initialize(ProcessContext context) {
				context.Logger.LogInfo(this.GetType().Name
					+ ":" + (Symbols != null ? TestUtils.Print(Symbols) : ""), "");
			}

			public void Cleanup() { }

		}

		public class MndatorySettableSymbolPropertiesComponent : IComponent {

			[UserSettableSybols(IsMandatory = true)]
			public ImmutableList<Symbol<IValue>> Mandatory { private get; set; }


			public void Initialize(ProcessContext context) {
				context.Logger.LogInfo(this.GetType().Name
					+ ":" + (Mandatory != null ? TestUtils.Print(Mandatory) : ""), "");
			}

			public void Cleanup() { }

		}

		public class GettablePropertiesComponent : IComponent {

			[UserGettable(GettableBeforeInitialiation=true)]
			public Constant ConstantGet { get { return 8.ToConst(); } }

			[UserGettable(GettableBeforeInitialiation = true)]
			public ValuesArray ValuesArrayGet { get { return new ValuesArray(new ImmutableList<IValue>(1.ToConst(), 2.ToConst(), 3.ToConst())); } }

			[UserGettable(GettableBeforeInitialiation = true)]
			public IValue IValueGet { get { return 42.ToConst(); } }


			public void Initialize(ProcessContext context) {
				context.Logger.LogInfo(this.GetType().Name, "");
			}

			public void Cleanup() { }

		}

	}
}
