// Altairis Web Security Toolkit
// Copyright © Michal A. Valasek - Altairis, 2006-2011 | www.altairis.cz 
// Licensed under terms of Microsoft Permissive License (MS-PL)

using System;
using System.Collections.Specialized;

namespace Altairis.Web.Security {
    internal static class ConfigurationExtensionMethods {

        public static string GetConfigValue(this NameValueCollection config, string name, string defaultValue) {
            // Validate arguments
            if (config == null) throw new ArgumentNullException("config");
            if (name == null) throw new ArgumentNullException("name");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "name");

            // Check if we have value in collection
            if (Array.IndexOf(config.AllKeys, name) > -1) {
                var r = config[name];
                config.Remove(name);
                return r;
            }
            else {
                return defaultValue;
            }
        }

        public static int GetConfigValue(this NameValueCollection config, string name, int defaultValue) {
            // Validate arguments
            if (config == null) throw new ArgumentNullException("config");
            if (name == null) throw new ArgumentNullException("name");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "name");

            // Check if we have value in collection
            if (Array.IndexOf(config.AllKeys, name) > -1) {
                int r;
                var parsed = int.TryParse(config[name], out r);
                if (!parsed) throw new System.Configuration.ConfigurationErrorsException(string.Format("Value of the \"{0}\" attribute is not valid Int32.", name));
                config.Remove(name);
                return r;
            }
            else {
                return defaultValue;
            }

        }

        public static bool GetConfigValue(this NameValueCollection config, string name, bool defaultValue) {
            // Validate arguments
            if (config == null) throw new ArgumentNullException("config");
            if (name == null) throw new ArgumentNullException("name");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "name");

            // Check if we have value in collection
            if (Array.IndexOf(config.AllKeys, name) > -1) {
                bool r;
                var parsed = bool.TryParse(config[name], out r);
                if (!parsed) throw new System.Configuration.ConfigurationErrorsException(string.Format("Value of the \"{0}\" attribute is not valid Boolean.", name));
                config.Remove(name);
                return r;
            }
            else {
                return defaultValue;
            }

        }

        public static T GetConfigValue<T>(this NameValueCollection config, string name, T defaultValue) where T : struct {
            if (config == null) throw new ArgumentNullException("config");
            if (name == null) throw new ArgumentNullException("name");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "name");
            if (!typeof(T).IsEnum) throw new ArgumentException("Generic type must be enum");

            if (Array.IndexOf(config.AllKeys, name) > -1) {
                T r;
                var parsed = Enum.TryParse<T>(config[name], out r);
                if (!parsed) throw new System.Configuration.ConfigurationErrorsException(string.Format("Value of the \"{0}\" attribute is not valid {1}.", name, typeof(T)));
                config.Remove(name);
                return r;
            }
            else {
                return defaultValue;
            }

        }

    }
}
