using System.Collections.Generic;

namespace Malsys.Media {
	public static class SubdividableSphereStaticCache {

		private static List<SubdividableSphere> cache = new List<SubdividableSphere>();


		public static SubdividableSphere GetSubdividedSphere(int subdivisionCount) {
			if (cache.Count > subdivisionCount && cache[subdivisionCount] != null) {
				return cache[subdivisionCount];
			}

			var sphere = new SubdividableSphere(true);

			for (int i = 0; i < subdivisionCount; i++) {
				sphere.Subdivide();
				if (cache.Count <= i) {
					cache.Add(null);
				}
			}

			if (cache.Count <= subdivisionCount) {
				cache.Add(sphere);
			}
			else {
				cache[subdivisionCount] = sphere;
			}

			return sphere;
		}

	}
}
