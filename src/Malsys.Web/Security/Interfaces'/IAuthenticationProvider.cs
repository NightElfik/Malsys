
namespace Malsys.Web.Security {
	public interface IAuthenticationProvider {

		void LogOn(string userName, bool persistent);

		void LogOff();

	}
}
