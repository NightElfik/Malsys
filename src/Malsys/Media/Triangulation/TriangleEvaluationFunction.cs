
namespace Malsys.Media.Triangulation {
	public class TriangleEvaluationFunction {

		public readonly Trianguler3DScoreOrdering Ordering;
		public readonly Trianguler3DRecountMode RecountMode;
		public readonly TriangleEvaluateDelegate EvalDelegate;

		public TriangleEvaluationFunction(Trianguler3DScoreOrdering ordering, Trianguler3DRecountMode recountMode, TriangleEvaluateDelegate evalDelegate) {
			Ordering = ordering;
			RecountMode = recountMode;
			EvalDelegate = evalDelegate;
		}

	}
}
