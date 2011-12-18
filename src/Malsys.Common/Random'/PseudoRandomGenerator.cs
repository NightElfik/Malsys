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
