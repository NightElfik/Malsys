﻿/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;

namespace Malsys {
	[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	public sealed class StringValueAttribute : Attribute {
		public readonly string String;

		public StringValueAttribute(string str) {
			String = str;
		}
	}



}
