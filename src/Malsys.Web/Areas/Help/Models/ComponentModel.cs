/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using Malsys.Reflection.Components;

namespace Malsys.Web.Areas.Help.Models {
	public class ComponentModel {

		public string[] AccessNames { get; set; }

		public ComponentMetadata Metadata { get; set; }

		public Type[] BaseTypes { get; set; }

		public Type[] DerivedTypes { get; set; }


	}
}