
namespace Malsys.Processing.Components {
	public interface IRandomGeneratorProvider : IComponent {

		IRandomGenerator GetRandomGenerator();

	}
}
