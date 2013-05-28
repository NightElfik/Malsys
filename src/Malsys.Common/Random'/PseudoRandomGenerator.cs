// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;

namespace Malsys {
	public class PseudoRandomGenerator : Random, IRandomGenerator {

		public PseudoRandomGenerator()
			: base() {

		}

		public PseudoRandomGenerator(int seed)
			: base(seed) {

		}

	}
}
