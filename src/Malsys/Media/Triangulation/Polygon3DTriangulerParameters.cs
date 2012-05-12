/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.Media.Triangulation {
	public class Polygon3DTriangulerParameters {

		/// <summary>
		/// Scoring function for ears.
		/// </summary>
		public TriangleEvaluateDelegate TriangleEvalDelegate { get; set; }

		/// <summary>
		/// Specifies how to order candidates for new triangle by score.
		/// </summary>
		public Trianguler3DScoreOrdering Ordering { get; set; }

		/// <summary>
		/// Specifies which triangles candidate's score will be recount after creating new triangle.
		/// </summary>
		public Trianguler3DRecountMode RecountMode { get; set; }

		/// <summary>
		/// Score bonus for triangles attached to previous winner.
		/// </summary>
		public double AttachedMultiplier { get; set; }

		/// <summary>
		/// If detect polygons that are in plane to triangulate them with better 2D algorithm.
		/// </summary>
		public bool DetectPlanarPolygons { get; set; }

		/// <summary>
		/// Max coefficient of variation of distance from computed plane to points
		/// (ratio of the standard deviation σ to the mean μ) while detecting 2D polygons.
		/// </summary>
		public double MaxVarCoefOfDist { get; set; }

	}
}
