
namespace Malsys.Ast {
	/// <summary>
	/// Immutable.
	/// </summary>
	public class OptionalParameter : IToken {

		public readonly Identificator NameId;
		public readonly Expression DefaultValue;


		public OptionalParameter(Identificator name, Expression defValue, Position pos) {
			NameId = name;
			DefaultValue = defValue;
			Position = pos;
		}


		public bool IsOptional { get { return !DefaultValue.IsEmpty; } }


		#region IToken Members

		public Position Position { get; private set; }

		#endregion
	}
}
