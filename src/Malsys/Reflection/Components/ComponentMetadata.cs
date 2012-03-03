using System;
using System.Linq;
using System.Reflection;
using Microsoft.FSharp.Collections;
using System.Collections.Generic;

namespace Malsys.Reflection.Components {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class ComponentMetadata {

		public readonly Type ComponentType;

		public readonly ImmutableList<ComponentGettablePropertyMetadata> GettableProperties;

		public readonly ImmutableList<ComponentSettablePropertyMetadata> SettableProperties;

		public readonly ImmutableList<ComponentSettableSybolsPropertyMetadata> SettableSymbolsProperties;

		public readonly ImmutableList<ComponentConnectablePropertyMetadata> ConnectableProperties;

		public readonly ImmutableList<ComponentCallableFunctionMetadata> CallableFunctions;

		public readonly ConstructorInfo ComponentConstructor;

		public readonly bool CaseSensitiveLookup;


		private readonly Dictionary<string, ComponentGettablePropertyMetadata> GettablePropertiesDictionary;
		private readonly Dictionary<string, ComponentSettablePropertyMetadata> SettablePropertiesDictionary;
		private readonly Dictionary<string, ComponentSettableSybolsPropertyMetadata> SettableSymbolsPropertiesDictionary;
		private readonly Dictionary<string, ComponentConnectablePropertyMetadata> ConnectablePropertiesDictionary;
		private readonly Dictionary<string, ComponentCallableFunctionMetadata> CallableFunctionsDictionary;


		public ComponentMetadata() {

		}

		public ComponentMetadata(Type componentType, ImmutableList<ComponentGettablePropertyMetadata> gettableProperties,
				ImmutableList<ComponentSettablePropertyMetadata> settableProperties, ImmutableList<ComponentSettableSybolsPropertyMetadata> settableSymbolsProperties,
				ImmutableList<ComponentConnectablePropertyMetadata> connectableProperties, ImmutableList<ComponentCallableFunctionMetadata> callableFunctions,
			ConstructorInfo componentConstructor, bool caseSensitiveLookup = false) {

			ComponentType = componentType;
			GettableProperties = gettableProperties;
			SettableProperties = settableProperties;
			SettableSymbolsProperties = settableSymbolsProperties;
			ConnectableProperties = connectableProperties;
			CallableFunctions = callableFunctions;
			ComponentConstructor = componentConstructor;
			CaseSensitiveLookup = caseSensitiveLookup;

			GettablePropertiesDictionary = new Dictionary<string, ComponentGettablePropertyMetadata>();
			SettablePropertiesDictionary = new Dictionary<string, ComponentSettablePropertyMetadata>();
			SettableSymbolsPropertiesDictionary = new Dictionary<string, ComponentSettableSybolsPropertyMetadata>();
			ConnectablePropertiesDictionary = new Dictionary<string, ComponentConnectablePropertyMetadata>();
			CallableFunctionsDictionary = new Dictionary<string, ComponentCallableFunctionMetadata>();

			buildLookupCache();

		}


		private void buildLookupCache() {

			// note: indexer works like AddOrUpdate method

			foreach (var gettProp in GettableProperties) {
				foreach (string name in gettProp.Names) {
					GettablePropertiesDictionary[CaseSensitiveLookup ? name : name.ToLowerInvariant()] = gettProp;
				}
			}

			foreach (var settProp in SettableProperties) {
				foreach (string name in settProp.Names) {
					SettablePropertiesDictionary[CaseSensitiveLookup ? name : name.ToLowerInvariant()] = settProp;
				}
			}

			foreach (var settProp in SettableSymbolsProperties) {
				foreach (string name in settProp.Names) {
					SettableSymbolsPropertiesDictionary[CaseSensitiveLookup ? name : name.ToLowerInvariant()] = settProp;
				}
			}

			foreach (var connProp in ConnectableProperties) {
				foreach (string name in connProp.Names) {
					ConnectablePropertiesDictionary[CaseSensitiveLookup ? name : name.ToLowerInvariant()] = connProp;
				}
			}

			foreach (var callfun in CallableFunctions) {
				foreach (string name in callfun.Names) {
					CallableFunctionsDictionary[CaseSensitiveLookup ? name : name.ToLowerInvariant()] = callfun;
				}
			}

		}


		public bool TryGetGettableProperty(string name, out ComponentGettablePropertyMetadata gettPropMetadata) {
			return GettablePropertiesDictionary.TryGetValue(CaseSensitiveLookup ? name : name.ToLowerInvariant(), out gettPropMetadata);
		}

		public bool TryGetSettableProperty(string name, out ComponentSettablePropertyMetadata settPropMetadata) {
			return SettablePropertiesDictionary.TryGetValue(CaseSensitiveLookup ? name : name.ToLowerInvariant(), out settPropMetadata);
		}

		public bool TryGetSettableSymbolsProperty(string name, out ComponentSettableSybolsPropertyMetadata settSymPropMetadata) {
			return SettableSymbolsPropertiesDictionary.TryGetValue(CaseSensitiveLookup ? name : name.ToLowerInvariant(), out settSymPropMetadata);
		}

		public bool TryGetConnectableProperty(string name, out ComponentConnectablePropertyMetadata connPropMetadata) {
			return ConnectablePropertiesDictionary.TryGetValue(CaseSensitiveLookup ? name : name.ToLowerInvariant(), out connPropMetadata);
		}

		public bool TryGetCallableFunction(string name, out ComponentCallableFunctionMetadata callFunMetadata) {
			return CallableFunctionsDictionary.TryGetValue(CaseSensitiveLookup ? name : name.ToLowerInvariant(), out callFunMetadata);
		}


	}
}
