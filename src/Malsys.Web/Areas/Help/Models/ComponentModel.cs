using System;
using System.Collections.Generic;
using System.Linq;
using Malsys.Processing.Components;
using Malsys.Reflection;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;
using Malsys.Processing.Components.Interpreters;

namespace Malsys.Web.Areas.Help.Models {
	public class ComponentModel {

		public static ComponentModel FromType(Type type, XmlDocReader xmlDocReader) {
			return FromType(type, xmlDocReader, new string[0], new Type[0]);
		}

		public static ComponentModel FromType(Type type, XmlDocReader xmlDocReader, IEnumerable<string> accessibleNames, IEnumerable<Type> allComponentsTypes) {

			ComponentModel result = new ComponentModel() {
				Type = type,
				AccessibleNames = accessibleNames,
				IsContainer = type.IsInterface,
				BaseTypes = (type.BaseType != null ? type.GetInterfaces().Concat(new Type[] { type.BaseType }) : type.GetInterfaces())
					.Where(x => typeof(IProcessComponent).IsAssignableFrom(x)).OrderBy(x => x.FullName),
				DerivedTypes = allComponentsTypes.Where(x => !type.Equals(x) && type.IsAssignableFrom(x)).OrderBy(x => x.FullName),
				Documentation = xmlDocReader.GetXmlDocumentation(type)
			};


			ComponentAttribute cAttr;
			if (type.TryGetAttribute(out cAttr)) {
				result.Name = cAttr.Name;
				result.Group = cAttr.Group;
			}
			else {
				result.Name = result.Type.Name;
				result.Group = "";
			}

			result.SettableProperties = type.GetPropertiesHavingAttr<UserSettableAttribute>()
				.OrderBy(x => x.Item1.Name)
				.Select(x => {
					return new SettableProperty() {
						Name = x.Item1.Name,
						IsMandatory = x.Item2.IsMandatory,
						ValueType = convertToPropertyType(x.Item1.PropertyType)
					};
				});

			result.ConnectableProperties = type.GetPropertiesHavingAttr<UserConnectableAttribute>()
				.OrderBy(x => x.Item1.Name)
				.Select(x => {
					return new ConnectableProperty() {
						Name = x.Item1.Name,
						IsOptional = x.Item2.IsOptional,
						Type = x.Item1.PropertyType
					};
				});

			result.InterpretationMethods = type.GetMethodsHavingAttr<SymbolInterpretationAttribute>()
				.OrderBy(x => x.Item1.Name)
				.Select(x => {
					return new InterpretationMethod() {
						Name = x.Item1.Name,
						RequiredParametersCount = x.Item2.RequiredParametersCount,
						Documentation = xmlDocReader.GetXmlDocumentation(x.Item1)
					};
				});

			return result;
		}

		private static SettablePropertyType convertToPropertyType(Type t) {

			if (t.Equals(typeof(Constant))) { return SettablePropertyType.Constant; }
			else if (t.Equals(typeof(ValuesArray))) { return SettablePropertyType.Array; }
			else if (t.Equals(typeof(IValue))) { return SettablePropertyType.ConstantOrArray; }
			else if (t.Equals(typeof(ImmutableList<Symbol<IValue>>))) { return SettablePropertyType.Symbols; }
			else { return SettablePropertyType.Unknown; }

		}

		public string Name { get; set; }

		public string Documentation { get; set; }

		public string Group { get; set; }


		public Type Type { get; set; }

		public IEnumerable<string> AccessibleNames { get; set; }

		public bool IsContainer { get; set; }

		public IEnumerable<Type> BaseTypes { get; set; }

		public IEnumerable<Type> DerivedTypes { get; set; }



		public IEnumerable<SettableProperty> SettableProperties { get; set; }

		public IEnumerable<ConnectableProperty> ConnectableProperties { get; set; }

		public IEnumerable<InterpretationMethod> InterpretationMethods { get; set; }

	}

	public class SettableProperty {

		public string Name { get; set; }

		public string Documentation { get; set; }

		public bool IsMandatory { get; set; }

		public SettablePropertyType ValueType { get; set; }

		public string ValueTypeHumanReadableString {
			get {
				switch (ValueType) {
					case SettablePropertyType.Constant: return "constant";
					case SettablePropertyType.Array: return "array";
					case SettablePropertyType.ConstantOrArray: return "constant or array";
					case SettablePropertyType.Symbols: return "symbols";
					default: return "unknown";
				}
			}
		}

	}

	public class ConnectableProperty {

		public string Name { get; set; }

		public string Documentation { get; set; }

		public bool IsOptional { get; set; }

		public Type Type { get; set; }

	}

	public class InterpretationMethod {

		public string Name { get; set; }

		public string Documentation { get; set; }

		public int RequiredParametersCount { get; set; }


	}

	public enum SettablePropertyType {

		Unknown,
		Constant,
		Array,
		ConstantOrArray,
		Symbols,

	}
}