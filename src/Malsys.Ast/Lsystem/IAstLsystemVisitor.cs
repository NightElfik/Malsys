
namespace Malsys.Ast {
	public interface IAstLsystemVisitor {

		void Visit(Binding binding);
		void Visit(EmptyStatement emptyStat);
		void Visit(InterpretationBinding interpretBinding);
		void Visit(RewriteRule rewriteRule);

	}
}
