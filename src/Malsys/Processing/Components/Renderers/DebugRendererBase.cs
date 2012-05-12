/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.IO;
using Microsoft.FSharp.Collections;

namespace Malsys.Processing.Components.Renderers {
	/// <summary>
	/// Provides common functionality for renderer debuggers.
	/// </summary>
	/// <name>Renderer debugger base</name>
	/// <group>Renderers</group>
	public abstract class DebugRendererBase : IRenderer {

		protected FSharpMap<string, object> globalAdditionalData = MapModule.Empty<string, object>();

		protected ProcessContext context;
		protected Stream outputStream;
		protected TextWriter writer;

		public IMessageLogger Logger { get; set; }



		public virtual bool RequiresMeasure {
			get { return false; }
		}

		public virtual void Initialize(ProcessContext ctxt) {
			context = ctxt;
		}

		public virtual void Cleanup() {
			context = null;
		}

		public virtual void BeginProcessing(bool measuring) {
			outputStream = context.OutputProvider.GetOutputStream<DebugRendererBase>("debug renderer output", MimeType.Text.Plain);
			writer = new StreamWriter(outputStream);
		}

		public virtual void EndProcessing() {
			writer.Close();
		}


		public virtual void AddGlobalOutputData(string key, object value) {
			globalAdditionalData = globalAdditionalData.Add(key, value);
		}

		public virtual void AddCurrentOutputData(string key, object value) {
			if (outputStream != null) {
				context.OutputProvider.AddMetadata(outputStream, key, value);
			}
		}



		protected void logState(string format, params object[] args) {
			writer.WriteLine(format.FmtInvariant(args));
		}

	}
}
