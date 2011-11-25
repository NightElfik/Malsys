using System.Configuration;

namespace Altairis.Web.Security.Configuration {
    public class BasicAuthenticationElement : ConfigurationElement {

        [ConfigurationProperty("realm", DefaultValue = "", IsRequired = false)]
        [StringValidator(MinLength = 0, MaxLength = 100)]
        public string Realm {
            get { return (string)this["realm"]; }
            set { this["realm"] = value; }
        }

    }
}
