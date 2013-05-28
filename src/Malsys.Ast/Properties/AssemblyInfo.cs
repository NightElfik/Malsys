// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Malsys.Ast")]
[assembly: AssemblyDescription("Abstract syntax tree for Malsys.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Marek Fišer")]
[assembly: AssemblyProduct("Malsys.Ast")]
[assembly: AssemblyCopyright("Copyright © Marek Fišer 2012 All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]
[assembly: Guid("be090fdd-fa0a-448b-b492-9918c79cbec0")]

[assembly: AssemblyVersion(Malsys.Ast.MalsysAstVersion.CompiledWith)]
[assembly: AssemblyFileVersion(Malsys.Ast.MalsysAstVersion.CompiledWith)]


namespace Malsys.Ast {
	public static class MalsysAstVersion {

		public const string CompiledWith = "1.0.0";
		public static readonly string LinkedWith = CompiledWith;

		public static Version CompiledWithVersion { get { return new Version(CompiledWith); } }
		public static Version LinkedWithVersion { get { return new Version(LinkedWith); } }

	}
}