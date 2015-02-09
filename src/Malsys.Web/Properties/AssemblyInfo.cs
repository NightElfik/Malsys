using System;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Malsys.Web")]
[assembly: AssemblyDescription("Web interface of Malsys")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Marek Fišer")]
[assembly: AssemblyProduct("Malsys.Web")]
[assembly: AssemblyCopyright("Copyright © Marek Fišer 2012 All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]
[assembly: Guid("79bd5bc3-6fd7-4f26-9eb3-372745f9aa31")]

[assembly: AssemblyVersion(Malsys.Web.MalsysWebVersion.CompiledWith)]
[assembly: AssemblyFileVersion(Malsys.Web.MalsysWebVersion.CompiledWith)]


namespace Malsys.Web {
	public static class MalsysWebVersion {

		public const string CompiledWith = "1.5.1";
		public static readonly string LinkedWith = CompiledWith;

		public static Version CompiledWithVersion { get { return new Version(CompiledWith); } }
		public static Version LinkedWithVersion { get { return new Version(LinkedWith); } }

	}
}
