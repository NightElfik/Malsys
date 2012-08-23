/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.Collections.Generic;
using Malsys.Processing;
using Malsys.Reflection.Components;
using Malsys.SemanticModel.Compiled;

namespace Malsys.Web.Areas.Documentation.Models {
	public class ConfigurationModel {

		public ProcessConfigurationStatement ProcessConfiguration { get; set; }

		public IComponentMetadataResolver ComponentMetadataResolver { get; set; }

		public List<KeyValuePair<ComponentMetadata, ComponentGettablePropertyMetadata>> GettableProperties { get; set; }

		public List<KeyValuePair<ComponentMetadata, ComponentSettablePropertyMetadata>> SettableProperties { get; set; }

		public List<KeyValuePair<ComponentMetadata, ComponentSettableSybolsPropertyMetadata>> SettableSymbolProperties { get; set; }

		public List<KeyValuePair<ComponentMetadata, ComponentConnectablePropertyMetadata>> ConnectableProperties { get; set; }

		public List<KeyValuePair<ComponentMetadata, ComponentCallableFunctionMetadata>> CallableFunctions { get; set; }

		public List<KeyValuePair<ComponentMetadata, ComponentInterpretationMethodMetadata>> InterpretationMethods { get; set; }

	}
}