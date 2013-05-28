// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Malsys.Compilers;
using Malsys.Evaluators;
using Malsys.Processing;
using Malsys.Processing.Components;
using Malsys.Resources;

namespace Malsys.Reflection {
	/// <summary>
	/// Helper class for loading all Malsys stuff.
	/// It can load everything from an assembly or individual members from types.
	/// </summary>
	public class MalsysLoader {

		/// <remarks>
		/// Classes implementing the IComponent interface are loaded as Components.
		///
		/// Compiler constants are loaded from classes marked with the MalsysConstants attribute.
		///
		/// Compiler functions are loaded from classes marked with the MalsysFunctions attribute.
		///
		/// Operators are loaded from classes marked with the MalsysOpertors attribute.
		/// </remarks>
		public void LoadMalsysStuffFromAssembly(Assembly a, ICompilerConstantsContainer constCont, IOperatorsContainer opCont,
				ref IExpressionEvaluatorContext funCont, IComponentMetadataContainer compCont, IMessageLogger logger, IXmlDocReader xmlDocReader = null) {

			foreach (var t in a.GetTypes()) {

				if (!t.IsClass && !t.IsInterface) {
					continue;
				}

				if (IsComponent(t)) {
					compCont.RegisterComponentNameAndFullName(t, logger, xmlDocReader);
				}

				var attrs = t.GetCustomAttributes(false);

				if (attrs.Where(x => x.GetType() == typeof(MalsysConstantsAttribute)).FirstOrDefault() != null) {
					foreach (var c in LoadCompilerConstants(t, xmlDocReader)) {
						constCont.AddCompilerConstant(c);
					}
				}

				if (attrs.Where(x => x.GetType() == typeof(MalsysOpertorsAttribute)).FirstOrDefault() != null) {
					foreach (var o in LoadOeprators(t, xmlDocReader)) {
						opCont.AddOperator(o);
					}
				}

				if (attrs.Where(x => x.GetType() == typeof(MalsysFunctionsAttribute)).FirstOrDefault() != null) {
					foreach (var f in LoadFunctionDefinitions(t, xmlDocReader)) {
						funCont = funCont.AddFunction(f);
					}
				}

			}

		}


		public bool IsComponent(Type t) {
			return (typeof(IComponent)).IsAssignableFrom(t);
		}


		/// <summary>
		/// Loads all compiler constants from given type.
		/// Compiler constants are public static fields of type CompilerConstant.
		/// </summary>
		public IEnumerable<CompilerConstant> LoadCompilerConstants(Type t, IXmlDocReader docReader = null) {
			foreach (var fieldInfo in t.GetFields(BindingFlags.Public | BindingFlags.Static)) {

				if (fieldInfo.FieldType != typeof(double)) {
					continue;
				}

				double value = (double)fieldInfo.GetValue(null);
				string summary = docReader != null ? docReader.GetXmlDocumentationAsString(fieldInfo) : null;
				string group = docReader != null ? docReader.GetXmlDocumentationAsString(fieldInfo, "group") : null;

				foreach (var name in fieldInfo.GetAccessNames()) {
					yield return new CompilerConstant(name, value, summary, group);
				}
			}
		}

		/// <summary>
		/// Loads all operators from given type.
		/// Operators are public static fields of type OperatorCore.
		/// </summary>
		public IEnumerable<OperatorCore> LoadOeprators(Type t, IXmlDocReader docReader = null) {
			foreach (var fieldInfo in t.GetFields(BindingFlags.Public | BindingFlags.Static)) {

				if (fieldInfo.FieldType != typeof(OperatorCore)) {
					continue;
				}


				var opCore = (OperatorCore)fieldInfo.GetValue(null);

				string summary = docReader != null ? docReader.GetXmlDocumentationAsString(fieldInfo) : null;
				opCore.SetDocumentation(summary);

				yield return opCore;

			}
		}


		/// <summary>
		/// Loads all functions from given type.
		/// Functions are public static fields of type FunctionCore.
		/// </summary>
		public IEnumerable<FunctionInfo> LoadFunctionDefinitions(Type t, IXmlDocReader docReader = null) {
			foreach (var fieldInfo in t.GetFields(BindingFlags.Public | BindingFlags.Static)) {

				if (fieldInfo.FieldType != typeof(FunctionCore)) {
					continue;
				}

				var funCore = (FunctionCore)fieldInfo.GetValue(null);
				string summary = docReader != null ? docReader.GetXmlDocumentationAsString(fieldInfo) : null;
				string group = docReader != null ? docReader.GetXmlDocumentationAsString(fieldInfo, "group") : null;

				foreach (var name in fieldInfo.GetAccessNames()) {
					yield return new FunctionInfo(name, funCore.ParametersCount, funCore.EvalFunction, funCore.ParamsTypesCyclicPattern, fieldInfo, summary, group);
				}

			}

		}

	}
}
