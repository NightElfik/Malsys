
namespace Malsys {
	public class LsystemDefinition {
		public string Name { get; set; }
		public string AncestorName { get; set; }

		public OptionalParameter[] Parameters { get; set; }

		public VariableDefinition[] Variables { get; set; }
		public RewriteRule[] RewriteRules { get; set; }
	}
}
