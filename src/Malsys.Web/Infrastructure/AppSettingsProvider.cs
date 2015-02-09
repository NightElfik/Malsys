using System.Configuration;

namespace Malsys.Web.Infrastructure {
	public class AppSettingsProvider : IAppSettingsProvider {

		public string this[string key] {
			get { return ConfigurationManager.AppSettings[key]; }
		}

	}
}
