using System;
using Malsys.Reflection.Components;

namespace Malsys.Web.Areas.Documentation.Models {
	public class ComponentModel {

		public string[] AccessNames { get; set; }

		public ComponentMetadata Metadata { get; set; }

		public Type[] BaseTypes { get; set; }

		public Type[] DerivedTypes { get; set; }


	}
}