
namespace Malsys.Processing.Components {
	public interface IComponent {

		ProcessContext Context { set; }


		void BeginProcessing(bool measuring);

		void EndProcessing();

	}
}
