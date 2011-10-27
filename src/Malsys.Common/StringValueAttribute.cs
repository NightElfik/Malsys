using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Malsys {

	[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	public sealed class StringValueAttribute : Attribute {
		public readonly string String;

		public StringValueAttribute(string str) {
			String = str;
		}
	}



}
