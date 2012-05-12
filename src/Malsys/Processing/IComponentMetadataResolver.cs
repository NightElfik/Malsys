/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using System.Collections.Generic;
using Malsys.Reflection;
using Malsys.Reflection.Components;

namespace Malsys.Processing {

	public interface IComponentMetadataResolver {

		ComponentMetadata ResolveComponentMetadata(string name);

	}

	public interface IComponentMetadataContainer {

		void RegisterComponentMetadata(string name, ComponentMetadata metadata);

		IEnumerable<KeyValuePair<string, ComponentMetadata>> GetAllRegisteredComponents();

	}



	public static class IComponentMetadataContainerExtensions {


		public static readonly ComponentMetadataDumper ComponentMetadataDumper = new ComponentMetadataDumper();


		public static void RegisterComponentMetadata(this IComponentMetadataContainer container, string name, Type t, IMessageLogger logger, IXmlDocReader xmlDocReader = null) {
			container.RegisterComponentMetadata(name, ComponentMetadataDumper.GetMetadata(t, logger, xmlDocReader));
		}

		public static void RegisterComponent(this IComponentMetadataContainer container, Type t, IMessageLogger logger, IXmlDocReader xmlDocReader = null) {
			container.RegisterComponentMetadata(t.Name, ComponentMetadataDumper.GetMetadata(t, logger, xmlDocReader));
		}

		public static void RegisterComponentNameAndFullName(this IComponentMetadataContainer container, Type t, IMessageLogger logger, IXmlDocReader xmlDocReader = null) {
			var meta = ComponentMetadataDumper.GetMetadata(t, logger, xmlDocReader);
			container.RegisterComponentMetadata(t.Name, meta);
			container.RegisterComponentMetadata(t.FullName, meta);
		}

	}

}
