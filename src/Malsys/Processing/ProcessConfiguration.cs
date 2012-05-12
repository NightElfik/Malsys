/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using Malsys.Processing.Components;
using Microsoft.FSharp.Collections;

namespace Malsys.Processing {
	public class ProcessConfiguration {

		public readonly FSharpMap<string, ConfigurationComponent> Components;

		/// <summary>
		/// True if at least one component in configuration requires measure.
		/// </summary>
		public readonly bool RequiresMeasure;

		public readonly IProcessStarter StarterComponent;



		public ProcessConfiguration(FSharpMap<string, ConfigurationComponent> components, bool requiresMeasure, IProcessStarter starterComponent) {

			Components = components;
			RequiresMeasure = requiresMeasure;
			StarterComponent = starterComponent;

		}


	}
}
