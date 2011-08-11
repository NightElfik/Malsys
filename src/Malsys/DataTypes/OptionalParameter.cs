﻿using Malsys.Expressions;

namespace Malsys {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class OptionalParameter {

		public readonly string Name;
		public readonly IValue DefaultValue;


		public OptionalParameter() {
			Name = null;
			DefaultValue = null;
		}

		public OptionalParameter(string name) {
			Name = name;
			DefaultValue = null;
		}

		public OptionalParameter(string name, IValue defaultValue) {
			Name = name;
			DefaultValue = defaultValue;
		}


		public bool IsOptional { get { return DefaultValue != null; } }

	}
}
