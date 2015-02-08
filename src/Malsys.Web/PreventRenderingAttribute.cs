using System;
using System.Web.Mvc;

namespace Malsys.Web {
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class PreventRenderingAttribute : Attribute, IMetadataAware {

		public void OnMetadataCreated(ModelMetadata metadata) {
			metadata.ShowForDisplay = false;
			metadata.ShowForEdit = false;
		}

	}
}