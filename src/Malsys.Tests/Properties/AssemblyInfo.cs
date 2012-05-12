/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Malsys.Tests")]
[assembly: AssemblyDescription("Unit test for Malsys")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Marek Fišer")]
[assembly: AssemblyProduct("Malsys.Tests")]
[assembly: AssemblyCopyright("Copyright © Marek Fišer 2012 All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]
[assembly: Guid("91b51565-4a20-47e1-9dd8-fe3fe3c1dfd0")]

[assembly: AssemblyVersion(Malsys.Tests.MalsysTestsVersion.CompiledWith)]
[assembly: AssemblyFileVersion(Malsys.Tests.MalsysTestsVersion.CompiledWith)]


namespace Malsys.Tests {
	public static class MalsysTestsVersion {

		public const string CompiledWith = "1.0.0.0";
		public static readonly string LinkedWith = CompiledWith;

		public static Version CompiledWithVersion { get { return new Version(CompiledWith); } }
		public static Version LinkedWithVersion { get { return new Version(LinkedWith); } }

	}
}