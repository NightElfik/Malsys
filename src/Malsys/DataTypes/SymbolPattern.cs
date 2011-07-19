
namespace Malsys {
	public class SymbolPattern {
		public string Symbol { get; set; }
		public string[] ParamsNames { get; set; }


		public SymbolPattern() {

		}

		public SymbolPattern(string symbol, string[] paramsNames) {
			Symbol = symbol;
			ParamsNames = paramsNames;
		}
	}
}
