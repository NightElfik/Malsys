using System;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Malsys.Common")]
[assembly: AssemblyDescription("Common library for Malsys")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Marek Fišer")]
[assembly: AssemblyProduct("Malsys.Common")]
[assembly: AssemblyCopyright("Copyright © Marek Fišer 2012")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]
[assembly: Guid("36f3dbd3-623f-44ec-a10c-4297ea86a9e6")]

[assembly: AssemblyVersion(Malsys.Common.MalsysCommonVersion.CompiledWith)]
[assembly: AssemblyFileVersion(Malsys.Common.MalsysCommonVersion.CompiledWith)]


namespace Malsys.Common {
	public static class MalsysCommonVersion {

		public const string CompiledWith = "1.0.0.0";
		public static readonly string LinkedWith = CompiledWith;

		public static Version CompiledWithVersion { get { return new Version(CompiledWith); } }
		public static Version LinkedWithVersion { get { return new Version(LinkedWith); } }

	}
}