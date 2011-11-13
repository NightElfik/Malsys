
namespace Malsys.Ast {
	public class LsystemSymbolList : ImmutableListPos<LsystemSymbol>, IBindable {


		public LsystemSymbolList(ImmutableListPos<LsystemSymbol> symbols)
			: base(symbols) {
		}


		#region IBindableVisitable Members

		public void Accept(IBindableVisitor visitor) {
			visitor.Visit(this);
		}

		#endregion
	}
}
