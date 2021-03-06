﻿using Microsoft.FSharp.Core;

namespace Malsys {
	public static class FSharpOptionExtensions {

		public static T ValueOrNull<T>(this FSharpOption<T> option) where T : class {
			if (OptionModule.IsSome(option)) {
				return option.Value;
			}
			else {
				return null;
			}
		}

	}
}
