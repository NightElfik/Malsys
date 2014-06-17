using Malsys.Web.ArticleTools;

namespace Malsys.Web.Models {
	public class ArticleModelBase {

		public SectionsManager SectionsManager { get; set; }


		private LayoutManager layoutManager;
		public LayoutManager LayoutManager {
			get {
				if (layoutManager == null) {
					layoutManager = new LayoutManager();
				}
				return layoutManager;
			}
		}

		//private FiguresManager figuresManager;
		//public FiguresManager FiguresManager {
		//	get {
		//		if (figuresManager == null) {
		//			figuresManager = new FiguresManager();
		//		}
		//		return figuresManager;
		//	}
		//}

		//private CodeListingsManager codeListingsManager;
		//public CodeListingsManager CodeListingsManager {
		//	get {
		//		if (codeListingsManager == null) {
		//			codeListingsManager = new CodeListingsManager();
		//		}
		//		return codeListingsManager;
		//	}
		//}

		//private EquationManager equationManager;
		//public EquationManager EquationManager {
		//	get {
		//		if (equationManager == null) {
		//			equationManager = new EquationManager();
		//		}
		//		return equationManager;
		//	}
		//}

	}
}