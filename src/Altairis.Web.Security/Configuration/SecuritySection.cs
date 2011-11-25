using System.Configuration;

namespace Altairis.Web.Security.Configuration {

    public class SecuritySection : ConfigurationSection {

        private const string ConfigurationPath = "altairis.web/security";

        public static SecuritySection GetCurrentOrDefault() {
            return (ConfigurationManager.GetSection(ConfigurationPath) as SecuritySection) ?? new SecuritySection();
        }

        [ConfigurationProperty("basicAuthentication")]
        public BasicAuthenticationElement BasicAuthentication {
            get { return (BasicAuthenticationElement)this["basicAuthentication"]; }
            set { this["basicAuthentication"] = value; }
        }

    }

}
