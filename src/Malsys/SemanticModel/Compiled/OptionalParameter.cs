﻿using Malsys.Expressions;

namespace Malsys.SemanticModel.Compiled {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class OptionalParameter {

		public readonly string Name;
		public readonly IExpression DefaultValue;


		public OptionalParameter() {
			Name = null;
			DefaultValue = null;
		}

		public OptionalParameter(string name) {
			Name = name;
			DefaultValue = null;
		}

		public OptionalParameter(string name, IExpression defaultValue) {
			Name = name;
			DefaultValue = defaultValue;
		}


		public bool IsOptional { get { return DefaultValue != null; } }

	}
}