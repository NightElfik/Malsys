using System;
using System.Collections.Generic;
using System.Reflection;
using StringInt = System.Tuple<string, int>;
using Malsys.Compilers.Expressions;

namespace Malsys.Expressions {
	public class KnownConstFunOpProvider : IKnownConstantsProvider, IKnownFunctionsProvider, IKnownOperatorsProvider {

		private Dictionary<string, KnownConstant> constCache = new Dictionary<string, KnownConstant>();
		private Dictionary<StringInt, FunctionCore> funCache = new Dictionary<StringInt,FunctionCore>();
		private Dictionary<StringInt, OperatorCore> opCache = new Dictionary<StringInt,OperatorCore>();

		/// <summary>
		/// Loads all constants, functions and operators from public static fields of given type.
		/// </summary>
		public void LoadFromType(Type t, bool ignoreDuplicities = false) {

			foreach (FieldInfo fi in t.GetFields(BindingFlags.Public | BindingFlags.Static)) {
				if (fi.FieldType.Equals(typeof(KnownConstant))) {
					load((KnownConstant)fi.GetValue(null), ignoreDuplicities);
				}
				else if (fi.FieldType.Equals(typeof(FunctionCore))) {
					load((FunctionCore)fi.GetValue(null), ignoreDuplicities);
				}
				else if (fi.FieldType.Equals(typeof(OperatorCore))) {
					load((OperatorCore)fi.GetValue(null), ignoreDuplicities);
				}
			}
		}

		#region IKnownConstantsProvider Members

		public bool TryGet(string name, out KnownConstant result) {
			return constCache.TryGetValue(name.ToLowerInvariant(), out result);
		}

		public IEnumerable<KnownConstant> GetAllConstants() {
			return constCache.Values;
		}

		#endregion

		#region IKnownFunctionsProvider Members

		public bool TryGet(string syntax, int paramsCount, out FunctionCore result) {

			syntax = syntax.ToLowerInvariant();

			if (funCache.TryGetValue(new StringInt(syntax, paramsCount), out result)) {
				return true;
			}
			else if (funCache.TryGetValue(new StringInt(syntax, FunctionCore.AnyParamsCount), out result)) {
				// set arity to desired number
				result = new FunctionCore(result.Name, paramsCount, result.ParamsTypes, result.EvalFunction);
				return true;
			}
			else {
				return false;
			}
		}

		public IEnumerable<FunctionCore> GetAllFunctions() {
			return funCache.Values;
		}

		#endregion

		#region IKnownOperatorsProvider Members

		public bool TryGet(string syntax, byte arity, out OperatorCore result) {
			return opCache.TryGetValue(new StringInt(syntax, arity), out result);
		}

		public IEnumerable<OperatorCore> GetAllOperators() {
			return opCache.Values;
		}

		#endregion


		private void load(KnownConstant cnst, bool ignoreDuplicities) {

			string key = cnst.Name.ToLowerInvariant();

			if (constCache.ContainsKey(key)) {
				if (ignoreDuplicities) {
					constCache[key] = cnst;
				}
				else {
					throw new Exception("Known constant `{0}` alredy exists in cache.".Fmt(key));
				}
			}
			else {
				constCache.Add(key, cnst);
			}
		}

		private void load(FunctionCore fun, bool ignoreDuplicities) {

			StringInt key = new StringInt(fun.Name.ToLowerInvariant(), fun.ParametersCount);

			if (funCache.ContainsKey(key)) {
				if (ignoreDuplicities) {
					funCache[key] = fun;
				}
				else {
					throw new Exception("Known function `{0}` alredy exists in cache.".Fmt(key));
				}
			}
			else {
				funCache.Add(key, fun);
			}
		}

		private void load(OperatorCore op, bool ignoreDuplicities) {

			StringInt key = new StringInt(op.Syntax, op.Arity);

			if (opCache.ContainsKey(key)) {
				if (ignoreDuplicities) {
					opCache[key] = op;
				}
				else {
					throw new Exception("Known operator `{0}` alredy exists in cache.".Fmt(key));
				}
			}
			else {
				opCache.Add(key, op);
			}
		}
	}
}
