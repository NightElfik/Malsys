using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


[assembly: AssemblyTitle("Malsys")]
[assembly: AssemblyDescription("Library for L-system processing.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Marek Fišer")]
[assembly: AssemblyProduct("Malsys")]
[assembly: AssemblyCopyright("Created by Marek Fišer, published under the MIT license")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]
[assembly: Guid("2f3751da-c10c-4a4c-81d4-0e9a8b93bd08")]

[assembly: AssemblyVersion(Malsys.MalsysVersion.CompiledWith)]
[assembly: AssemblyFileVersion(Malsys.MalsysVersion.CompiledWith)]

[assembly: InternalsVisibleTo("Malsys.Tests")]


namespace Malsys {
	public static class MalsysVersion {

		public const string CompiledWith = "1.4.3";
		public static readonly string LinkedWith = CompiledWith;

		public static Version CompiledWithVersion { get { return new Version(CompiledWith); } }
		public static Version LinkedWithVersion { get { return new Version(LinkedWith); } }

	}
}
