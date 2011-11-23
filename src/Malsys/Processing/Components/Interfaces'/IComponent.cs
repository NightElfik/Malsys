
namespace Malsys.Processing.Components {
	public interface IComponent {

		/// <summary>
		/// Retrieved at the end of component configuration.
		/// </summary>
		bool RequiresMeasure { get; }


		void Initialize(ProcessContext context);

		void Cleanup();


		void BeginProcessing(bool measuring);

		void EndProcessing();

	}
}
