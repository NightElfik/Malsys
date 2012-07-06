/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


[assembly: AssemblyTitle("Malsys")]
[assembly: AssemblyDescription("Library for L-system processing.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Marek Fišer")]
[assembly: AssemblyProduct("Malsys")]
[assembly: AssemblyCopyright("Copyright © Marek Fišer 2012 All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]
[assembly: Guid("2f3751da-c10c-4a4c-81d4-0e9a8b93bd08")]

[assembly: AssemblyVersion(Malsys.MalsysVersion.CompiledWith)]
[assembly: AssemblyFileVersion(Malsys.MalsysVersion.CompiledWith)]

[assembly: InternalsVisibleTo("Malsys.Tests")]


namespace Malsys {
	public static class MalsysVersion {

		public const string CompiledWith = "1.2.1";
		public static readonly string LinkedWith = CompiledWith;

		public static Version CompiledWithVersion { get { return new Version(CompiledWith); } }
		public static Version LinkedWithVersion { get { return new Version(LinkedWith); } }

	}
}