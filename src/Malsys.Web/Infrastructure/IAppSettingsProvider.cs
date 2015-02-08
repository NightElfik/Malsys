
namespace Malsys.Web.Infrastructure {
	public interface IAppSettingsProvider {

		string this[string key] { get; }

	}
}
