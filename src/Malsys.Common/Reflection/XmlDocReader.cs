﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Malsys.Reflection {
	/// <summary>
	/// Standard implementation of the IXmlDocReader that reads XmlDoc from files in given base path.
	/// </summary>
	/// <remarks>
	/// All public members are thread safe.
	/// Caches XDocuments for fast access to documentation.
	/// </remarks>
	public class XmlDocReader : IXmlDocReader {

		private Dictionary<string, XDocument> XDocumentCache = new Dictionary<string, XDocument>(StringComparer.OrdinalIgnoreCase);

		private string basePath;


		public XmlDocReader(string basePath = "") {
			this.basePath = basePath;
		}

		/// <summary>
		/// Returns the XML documentation for given member and tag path.
		/// </summary>
		public string GetXmlDocumentationAsString(MemberInfo member, string tagPath = "summary") {
			AssemblyName assemblyName = member.Module.Assembly.GetName();
			return getXmlDocumentation(member, tagPath);
		}

		/// <summary>
		/// Returns the expected name for a member element in the XML documentation file.
		/// </summary>
		private string getMemberElementName(MemberInfo member) {

			string prefixCode;
			string memberName = (member is Type)
				? ((Type)member).FullName  // member is a Type
				: (member.DeclaringType.FullName + "." + member.Name);  // member belongs to a Type

			switch (member.MemberType) {
				case MemberTypes.Constructor:
					// XML documentation uses slightly different constructor names
					memberName = memberName.Replace(".ctor", "#ctor");
					goto case MemberTypes.Method;

				case MemberTypes.Method:
					prefixCode = "M";

					// parameters are listed according to their type, not their name
					string paramTypesList = String.Join(
						",",
						((MethodBase)member).GetParameters()
							.Cast<ParameterInfo>()
							.Select(x => x.ParameterType.FullName)
							.ToArray()
					);
					if (!String.IsNullOrEmpty(paramTypesList)) {
						memberName += "(" + paramTypesList + ")";
					}
					break;

				case MemberTypes.Event:
					prefixCode = "E";
					break;

				case MemberTypes.Field:
					prefixCode = "F";
					break;

				case MemberTypes.NestedType:
					// XML documentation uses slightly different nested type names
					memberName = memberName.Replace('+', '.');
					goto case MemberTypes.TypeInfo;

				case MemberTypes.TypeInfo:
					prefixCode = "T";
					break;

				case MemberTypes.Property:
					prefixCode = "P";
					break;

				default:
					throw new ArgumentException("Unknown member type", "member");
			}

			// elements are of the form "M:Namespace.Class.Method"
			return prefixCode + ":" + memberName;
		}

		/// <summary>
		/// Returns the XML documentation from given xml file path for given member and tag path.
		/// </summary>
		/// <returns>Object that can contain bool, double, string, or IEnumerable&gt;T&lt;.</returns>
		private string getXmlDocumentation(MemberInfo member, string tagPath) {

			string assemblyFullName = member.Module.Assembly.GetName().FullName;
			XDocument xml;

			if (XDocumentCache.ContainsKey(assemblyFullName)) {
				xml = XDocumentCache[assemblyFullName];
			}
			else {
				lock (XDocumentCache) {
					if (XDocumentCache.ContainsKey(assemblyFullName)) {
						xml = XDocumentCache[assemblyFullName];
					}
					else {
						string path = basePath + Path.DirectorySeparatorChar + member.Module.Assembly.GetName().Name + ".xml";
						if (File.Exists(path)) {
							xml = XDocument.Load(path);
						}
						else {
							xml = new XDocument();  // empty document
						}
						XDocumentCache[assemblyFullName] = xml;
					}
				}
			}

			return xml.XPathEvaluate("string(/doc/members/member[@name='{0}']/{1})"
				.Fmt(getMemberElementName(member), tagPath)).ToString().Trim();
		}
	}
}
