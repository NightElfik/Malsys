using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Malsys {
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class UserSettableAttribute : Attribute {

		public UserSettableAttribute() {

		}

	}
}
