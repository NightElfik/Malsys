// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.Reflection;
using System.Runtime.InteropServices;


[assembly: AssemblyTitle("Malsys.BitmapRenderers")]
[assembly: AssemblyDescription("Malsys plugin adding some bitmap renderers.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Marek Fišer")]
[assembly: AssemblyProduct("Malsys.BitmapRenderers")]
[assembly: AssemblyCopyright("Copyright © Marek Fišer 2012 All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: Guid("716bf11f-5894-42b9-b5dd-37146797b185")]

[assembly: AssemblyVersion(Malsys.BitmapRenderers.MalsysBitmapRenderersVersion.CompiledWith)]
[assembly: AssemblyFileVersion(Malsys.BitmapRenderers.MalsysBitmapRenderersVersion.CompiledWith)]


namespace Malsys.BitmapRenderers {
	public static class MalsysBitmapRenderersVersion {

		public const string CompiledWith = "0.1.0";
		public static readonly string LinkedWith = CompiledWith;

		public static Version CompiledWithVersion { get { return new Version(CompiledWith); } }
		public static Version LinkedWithVersion { get { return new Version(LinkedWith); } }

	}
}