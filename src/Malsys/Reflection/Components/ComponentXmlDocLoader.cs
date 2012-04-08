
namespace Malsys.Reflection.Components {
	public class ComponentXmlDocLoader : IComponentXmlDocLoader {


		private readonly XmlDocReader docReader;


		public ComponentXmlDocLoader(XmlDocReader docReader) {
			this.docReader = docReader;
		}


		public void LoadXmlDoc(ComponentMetadata component) {

			if (component.IsDocumentationLoaded) {
				return;
			}

			loadXmlDocForSetProp(component.SettableProperties);
			loadXmlDocForSetSymProp(component.SettableSymbolsProperties);
			loadXmlDocForCallFuns(component.CallableFunctions);
			loadXmlDocForIntMethods(component.InterpretationMethods);

			string name = docReader.GetXmlDocumentation(component.ComponentType, "name");
			string group = docReader.GetXmlDocumentation(component.ComponentType, "group");
			string summary = docReader.GetXmlDocumentation(component.ComponentType);

			component.SetDocumentation(name, group, summary);

		}



		private void loadXmlDocForSetProp(ImmutableList<ComponentSettablePropertyMetadata> setProps) {
			foreach (var setProp in setProps) {
				string summary = docReader.GetXmlDocumentation(setProp.PropertyInfo);
				string excpected = docReader.GetXmlDocumentation(setProp.PropertyInfo, "expected");
				string def = docReader.GetXmlDocumentation(setProp.PropertyInfo, "default");
				setProp.SetDocumentation(summary, excpected, def);
			}
		}

		private void loadXmlDocForSetSymProp(ImmutableList<ComponentSettableSybolsPropertyMetadata> setProps) {
			foreach (var setProp in setProps) {
				string summary = docReader.GetXmlDocumentation(setProp.PropertyInfo);
				setProp.SetDocumentation(summary);
			}
		}

		private void loadXmlDocForCallFuns(ImmutableList<ComponentCallableFunctionMetadata> callFuns) {
			foreach (var callFun in callFuns) {
				string summary = docReader.GetXmlDocumentation(callFun.MethodInfo);
				string paramsDoc = docReader.GetXmlDocumentation(callFun.MethodInfo, "parameters");
				callFun.SetDocumentation(summary, paramsDoc);
			}
		}

		private void loadXmlDocForIntMethods(ImmutableList<ComponentInterpretationMethodMetadata> intMethods) {
			foreach (var intMethod in intMethods) {
				string summary = docReader.GetXmlDocumentation(intMethod.MethodInfo);
				string paramsDoc = docReader.GetXmlDocumentation(intMethod.MethodInfo, "parameters");
				intMethod.SetDocumentation(summary, paramsDoc);
			}
		}


	}
}
