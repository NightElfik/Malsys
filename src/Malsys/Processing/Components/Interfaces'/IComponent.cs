
namespace Malsys.Processing.Components {
	public interface IComponent {

		/// <summary>
		/// Retrieved after component initialization.
		/// </summary>
		bool RequiresMeasure { get; }


		void Initialize(ProcessContext ctxt);

		void Cleanup();


		void BeginProcessing(bool measuring);

		void EndProcessing();

	}
}
