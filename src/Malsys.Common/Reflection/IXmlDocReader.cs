using System.Reflection;

namespace Malsys.Reflection {
	public interface IXmlDocReader {

		string GetXmlDocumentationAsString(MemberInfo member, string tagPath = "summary");

	}
}
