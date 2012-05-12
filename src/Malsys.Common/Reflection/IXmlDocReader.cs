/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.Reflection;

namespace Malsys.Reflection {
	public interface IXmlDocReader {

		string GetXmlDocumentationAsString(MemberInfo member, string tagPath = "summary");

	}
}
