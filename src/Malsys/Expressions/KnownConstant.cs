using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Malsys.Expressions {
	public class KnownConstant {
		#region Static members

		#region Known constants definitions

		public static readonly KnownConstant True = new KnownConstant("true", 1.0);
		public static readonly KnownConstant False = new KnownConstant("false", 0.0);

		public static readonly KnownConstant Nan = new KnownConstant("nan", double.NaN);
		public static readonly KnownConstant Infty = new KnownConstant("infty", double.PositiveInfinity);
		public static readonly KnownConstant Infinity = new KnownConstant(CharHelper.Infinity.ToString(), double.PositiveInfinity);
		public static readonly KnownConstant InfinityStr = new KnownConstant("infinity", double.PositiveInfinity);

		public static readonly KnownConstant Pi = new KnownConstant(CharHelper.Pi.ToString(), Math.PI);
		public static readonly KnownConstant PiStr = new KnownConstant("pi", Math.PI);

		public static readonly KnownConstant Tau = new KnownConstant(CharHelper.Tau.ToString(), 2 * Math.PI);
		public static readonly KnownConstant TauStr = new KnownConstant("tau", 2 * Math.PI);

		public static readonly KnownConstant E = new KnownConstant("e", Math.E);

		public static readonly KnownConstant Phi = new KnownConstant(CharHelper.Phi.ToString(), (1 + Math.Sqrt(5)) / 2);
		public static readonly KnownConstant GoldenRatio = new KnownConstant("goldenRatio", (1 + Math.Sqrt(5)) / 2);

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
		/// Tries to parse given string as known constant.
		/// </summary>
		public static bool TryParse(string name, out KnownConstant result) {
			return constCache.TryGetValue(name, out result);
		}

		#endregion

		public readonly string Name;
		public readonly double Value;

		private KnownConstant(string name, double value) {
			Name = name;
			Value = value;
		}
	}
}
