﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Malsys.Expressions {
	public class KnownConstant {
		#region Static members

		#region Known constants definitions

		public static readonly KnownConstant True = new KnownConstant("true", 1.0);
		public static readonly KnownConstant False = new KnownConstant("false", 0.0);

		public static readonly KnownConstant Nan = new KnownConstant("nan", double.NaN);

		public static readonly KnownConstant Infinity = new KnownConstant("infinity", double.PositiveInfinity);
		public static readonly KnownConstant Infty = Infinity.changeName("infty");
		public static readonly KnownConstant InfinityUnicode = Infinity.changeName(CharHelper.Infinity.ToString());

		public static readonly KnownConstant Pi = new KnownConstant("pi", Math.PI);
		public static readonly KnownConstant PiUnicode = Pi.changeName(CharHelper.Pi.ToString());

		public static readonly KnownConstant Tau = new KnownConstant("tau", 2 * Math.PI);
		public static readonly KnownConstant TauUnicode = Tau.changeName(CharHelper.Tau.ToString());

		public static readonly KnownConstant E = new KnownConstant("e", Math.E);

		public static readonly KnownConstant GoldenRatio = new KnownConstant("golden_ratio", (1 + Math.Sqrt(5)) / 2);
		public static readonly KnownConstant GoldenRatioCamel = GoldenRatio.changeName("goldenRatio");
		public static readonly KnownConstant GoldenRatioPhi = GoldenRatio.changeName(CharHelper.Phi.ToString());
		public static readonly KnownConstant GoldenRatioVarPhi = GoldenRatio.changeName(CharHelper.VarPhi.ToString());

		#endregion


		static Dictionary<string, KnownConstant> constCache;

		/// <summary>
		/// Builds constants cache from definitions in this class.
		/// </summary>
		static KnownConstant() {
			constCache = new Dictionary<string, KnownConstant>();

			foreach (FieldInfo fi in typeof(KnownConstant).GetFields(BindingFlags.Public | BindingFlags.Static)) {
				if (!fi.FieldType.Equals(typeof(KnownConstant))) {
					continue;
				}

				KnownConstant cnst = (KnownConstant)fi.GetValue(null);
				string key = cnst.Name.ToLowerInvariant();

				Debug.Assert(!constCache.ContainsKey(key), "Name `{0}` of known constant is not unique.".Fmt(key));

				constCache.Add(key, cnst);
			}
		}

		/// <summary>
		/// Tries to get constant with name equal to given string.
		/// </summary>
		public static bool TryGet(string name, out KnownConstant result) {
			return constCache.TryGetValue(name.ToLowerInvariant(), out result);
		}

		public static KnownConstant[] GetAllDefinedConstants() {
			return constCache.Values.ToArray();
		}

		#endregion

		public readonly string Name;
		public readonly double Value;


		private KnownConstant(string name, double value) {
			Name = name;
			Value = value;
		}


		private KnownConstant changeName(string newName) {
			return new KnownConstant(newName, Value);
		}
	}
}
