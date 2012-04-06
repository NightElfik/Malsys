
namespace Malsys.Reflection.Components {
	public class ComponentXmlDocLoader {


		private readonly XmlDocReader docReader;


		public ComponentXmlDocLoader(XmlDocReader docReader) {
			this.docReader = docReader;
		}


		public void LoadXmlDocFor(ComponentMetadata component) {


			loadXmlDocForIntMethods(component.InterpretationMethods);

			component.SetDocumentation();

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
