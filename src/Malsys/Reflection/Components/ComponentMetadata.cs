using System;
using System.Collections.Generic;
using System.Reflection;

namespace Malsys.Reflection.Components {
	/// <remarks>
	/// Nearly immutable (only documentation strings can be set later).
	/// </remarks>
	public class ComponentMetadata {

		public readonly Type ComponentType;

		public readonly bool IsContainer;

		public readonly ImmutableList<ComponentGettablePropertyMetadata> GettableProperties;

		public readonly ImmutableList<ComponentSettablePropertyMetadata> SettableProperties;

		public readonly ImmutableList<ComponentSettableSybolsPropertyMetadata> SettableSymbolsProperties;

		public readonly ImmutableList<ComponentConnectablePropertyMetadata> ConnectableProperties;

		public readonly ImmutableList<ComponentCallableFunctionMetadata> CallableFunctions;

		public readonly ImmutableList<ComponentInterpretationMethodMetadata> InterpretationMethods;

		public readonly bool HasCtorWithMessageLogger;
		public readonly ConstructorInfo ComponentConstructor;

		public readonly bool CaseSensitiveLookup;


		public bool IsDocumentationLoaded { get; private set; }

		public string NameDoc { get; private set; }
		public string GroupDoc { get; private set; }
		public string SummaryDoc { get; private set; }

		public string Name { get { return string.IsNullOrEmpty(NameDoc) ? ComponentType.Name : NameDoc; } }



		private readonly Dictionary<string, ComponentGettablePropertyMetadata> gettablePropertiesDictionary;
		private readonly Dictionary<string, ComponentSettablePropertyMetadata> settablePropertiesDictionary;
		private readonly Dictionary<string, ComponentSettableSybolsPropertyMetadata> settableSymbolsPropertiesDictionary;
		private readonly Dictionary<string, ComponentConnectablePropertyMetadata> connectablePropertiesDictionary;
		private readonly Dictionary<string, ComponentCallableFunctionMetadata> callableFunctionsDictionary;
		private readonly Dictionary<string, ComponentInterpretationMethodMetadata> interpretationMethodsDictionary;


		public ComponentMetadata() {

		}

		public ComponentMetadata(Type componentType, ImmutableList<ComponentGettablePropertyMetadata> gettableProperties,
				ImmutableList<ComponentSettablePropertyMetadata> settableProperties, ImmutableList<ComponentSettableSybolsPropertyMetadata> settableSymbolsProperties,
				ImmutableList<ComponentConnectablePropertyMetadata> connectableProperties, ImmutableList<ComponentCallableFunctionMetadata> callableFunctions,
				ImmutableList<ComponentInterpretationMethodMetadata> interpretationMethods, ConstructorInfo componentConstructor,
				bool hasCtorWithMessageLogger, bool isContainer = false, bool caseSensitiveLookup = false) {

			ComponentType = componentType;
			GettableProperties = gettableProperties;
			SettableProperties = settableProperties;
			SettableSymbolsProperties = settableSymbolsProperties;
			ConnectableProperties = connectableProperties;
			CallableFunctions = callableFunctions;
			InterpretationMethods = interpretationMethods;
			ComponentConstructor = componentConstructor;
			HasCtorWithMessageLogger = hasCtorWithMessageLogger;
			CaseSensitiveLookup = caseSensitiveLookup;
			IsContainer = isContainer;

			gettablePropertiesDictionary = new Dictionary<string, ComponentGettablePropertyMetadata>();
			settablePropertiesDictionary = new Dictionary<string, ComponentSettablePropertyMetadata>();
			settableSymbolsPropertiesDictionary = new Dictionary<string, ComponentSettableSybolsPropertyMetadata>();
			connectablePropertiesDictionary = new Dictionary<string, ComponentConnectablePropertyMetadata>();
			callableFunctionsDictionary = new Dictionary<string, ComponentCallableFunctionMetadata>();
			interpretationMethodsDictionary = new Dictionary<string, ComponentInterpretationMethodMetadata>();

			buildLookupCache();

		}


		private void buildLookupCache() {

			// note: indexer works like AddOrUpdate method

			foreach (var gettProp in GettableProperties) {
				foreach (string name in gettProp.Names) {
					gettablePropertiesDictionary[CaseSensitiveLookup ? name : name.ToLower()] = gettProp;
				}
			}

			foreach (var settProp in SettableProperties) {
				foreach (string name in settProp.Names) {
					settablePropertiesDictionary[CaseSensitiveLookup ? name : name.ToLower()] = settProp;
				}
			}

			foreach (var settProp in SettableSymbolsProperties) {
				foreach (string name in settProp.Names) {
					settableSymbolsPropertiesDictionary[CaseSensitiveLookup ? name : name.ToLower()] = settProp;
				}
			}

			foreach (var connProp in ConnectableProperties) {
				foreach (string name in connProp.Names) {
					connectablePropertiesDictionary[CaseSensitiveLookup ? name : name.ToLower()] = connProp;
				}
			}

			foreach (var callfun in CallableFunctions) {
				foreach (string name in callfun.Names) {
					callableFunctionsDictionary[CaseSensitiveLookup ? name : name.ToLower()] = callfun;
				}
			}

			foreach (var intMeth in InterpretationMethods) {
				foreach (string name in intMeth.Names) {
					interpretationMethodsDictionary[CaseSensitiveLookup ? name : name.ToLower()] = intMeth;
				}
			}

		}


		public bool TryGetGettableProperty(string name, out ComponentGettablePropertyMetadata gettPropMetadata) {
			return gettablePropertiesDictionary.TryGetValue(CaseSensitiveLookup ? name : name.ToLower(), out gettPropMetadata);
		}

		public bool TryGetSettableProperty(string name, out ComponentSettablePropertyMetadata settPropMetadata) {
			return settablePropertiesDictionary.TryGetValue(CaseSensitiveLookup ? name : name.ToLower(), out settPropMetadata);
		}

		public bool TryGetSettableSymbolsProperty(string name, out ComponentSettableSybolsPropertyMetadata settSymPropMetadata) {
			return settableSymbolsPropertiesDictionary.TryGetValue(CaseSensitiveLookup ? name : name.ToLower(), out settSymPropMetadata);
		}

		public bool TryGetConnectableProperty(string name, out ComponentConnectablePropertyMetadata connPropMetadata) {
			return connectablePropertiesDictionary.TryGetValue(CaseSensitiveLookup ? name : name.ToLower(), out connPropMetadata);
		}

		public bool TryGetCallableFunction(string name, out ComponentCallableFunctionMetadata callFunMetadata) {
			return callableFunctionsDictionary.TryGetValue(CaseSensitiveLookup ? name : name.ToLower(), out callFunMetadata);
		}

		public bool TryGetInterpretationMethod(string name, out ComponentInterpretationMethodMetadata intMethodMetadata) {
			return interpretationMethodsDictionary.TryGetValue(CaseSensitiveLookup ? name : name.ToLower(), out intMethodMetadata);
		}


		public void SetDocumentation(string name, string group, string summary) {

			NameDoc = name;
			GroupDoc = group;
			SummaryDoc = summary;

			IsDocumentationLoaded = true;

		}


	}
}
