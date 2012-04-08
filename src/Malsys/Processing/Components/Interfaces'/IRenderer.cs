﻿
namespace Malsys.Processing.Components {
	/// <name>Renderer container</name>
	/// <group>Renderers</group>
	public interface IRenderer : IProcessComponent {

		/// <summary>
		/// Adds data entry which will be associated with all following outputs.
		/// </summary>
		void AddGlobalOutputData(string key, object value);

		/// <summary>
		/// Adds data entry which will be associated with all outputs between BeginProcess and EndProcess methods.
		/// Should be called between BeginProcess and EndProcess methods.
		/// </summary>
		void AddCurrentOutputData(string key, object value);

	}
}
