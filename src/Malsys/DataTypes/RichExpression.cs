﻿using System.Collections.Generic;
using Malsys.Expressions;

namespace Malsys {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class RichExpression {

		public static readonly RichExpression True = new RichExpression(Constant.True);
		public static readonly RichExpression One = new RichExpression(Constant.One);


		public readonly IExpression Expression;
		public readonly ImmutableList<VariableDefinition<IExpression>> VariableDefinitions;


		public RichExpression(IExpression expr) {
			VariableDefinitions = ImmutableList<VariableDefinition<IExpression>>.Empty;
			Expression = expr;
		}

		public RichExpression(ImmutableList<VariableDefinition<IExpression>> varDefs, IExpression expr) {
			VariableDefinitions = varDefs;
			Expression = expr;
		}
	}
}