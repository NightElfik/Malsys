using System.Web.Security;

namespace Malsys.Web.Security {
	public class FormsAuthenticationProvider : IAuthenticationProvider {

		public void LogOn(string userName, bool persistent) {
			FormsAuthentication.SetAuthCookie(userName, persistent);
		}

		public void LogOff() {
			FormsAuthentication.SignOut();
		}

	}
}