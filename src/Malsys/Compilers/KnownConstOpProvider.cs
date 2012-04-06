using System;
using System.Collections.Generic;
using System.Reflection;
using Malsys.Reflection;
using Malsys.Resources;
using StringBool = System.Tuple<string, bool>;

namespace Malsys.Compilers {
	public class KnownConstOpProvider : IKnownConstantsProvider, IKnownOperatorsProvider {

		private Dictionary<string, double> constCache = new Dictionary<string, double>();
		private Dictionary<StringBool, OperatorCore> opCache = new Dictionary<StringBool, OperatorCore>();



		public bool TryGet(string name, out double result) {
			return constCache.TryGetValue(name.ToLowerInvariant(), out result);
		}

		public IEnumerable<double> GetAllConstants() {
			return constCache.Values;
		}


		public bool TryGet(string syntax, bool unary, out OperatorCore result) {
			return opCache.TryGetValue(new StringBool(syntax, unary), out result);
		}

		public IEnumerable<OperatorCore> GetAllOperators() {
			return opCache.Values;
		}


		/// <summary>
		/// Loads all constants from public static fields of given type marked with CompilerConstantAttribute.
		/// </summary>
		public void LoadConstants(Type t) {

			foreach (var fiAttrTuple in t.GetFieldsHavingAttr<CompilerConstantAttribute>()) {

				var fieldInfo = fiAttrTuple.Item1;
				var attr = fiAttrTuple.Item2;

				if (!fieldInfo.IsStatic) {
					continue;
				}

				if (fieldInfo.FieldType != typeof(double)) {
					continue;
				}

				double value = (double)fieldInfo.GetValue(null);

				foreach (var name in fieldInfo.GetAccessNames()) {
					constCache[name] = value;
				}
			}

		}

		/// <summary>
		/// Loads all (unary and binary) operators from public static fields of type OperatorCore.
		/// </summary>
		public void LoadOperators(Type t) {

			foreach (var fieldInfo in t.GetFields(BindingFlags.Public | BindingFlags.Static)) {

				if (fieldInfo.FieldType != typeof(OperatorCore)) {
					continue;
				}

				OperatorCore value = (OperatorCore)fieldInfo.GetValue(null);

				opCache[new StringBool(value.Syntax, value.IsUnary)] = value;

			}

		}


	}
}
