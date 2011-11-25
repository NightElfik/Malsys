// Altairis Web Security Toolkit
// Copyright © Michal A. Valasek - Altairis, 2006-2011 | www.altairis.cz 
// Licensed under terms of Microsoft Permissive License (MS-PL)

using System;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using Altairis.Web.Security.Configuration;

namespace Altairis.Web.Security {
    /// <summary>
    /// This class is authentication module, implementing the Basic authentication,
    /// as specified in the RFC2616 (Hypertext Transfer Protocol -- HTTP/1.1) and
    /// RFC2617 (HTTP Authentication: Basic and Digest Access Authentication).
    /// It uses standard membership database for verifying user names and passwords.
    /// </summary>
    public class BasicAuthenticationModule : IHttpModule {
        // These constants are pretty much all given by RFC2616 and RFC2617
        private const int AccessDeniedStatusCode = 401;
        private const string AuthenticationMethodName = "Basic";
        private const string AccessDeniedStatusText = "Access Denied";
        private const string ChallengeHeaderName = "WWW-Authenticate";
        private const string ChallengeHeaderValue = "Basic realm=\"{0}\"";
        private const string ResponseHeaderName = "Authorization";

        // Configuration
        SecuritySection config;

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
        /// </summary>
        public void Dispose() {
            // Nothing to dispose
        }

        /// <summary>
        /// Inits the specified module.
        /// </summary>
        /// <param name="application">The HTTP application containing the module.</param>
        public void Init(HttpApplication application) {
            // Get configuration
            this.config = SecuritySection.GetCurrentOrDefault();

            // Attach event handlers
            application.AuthenticateRequest += new EventHandler(application_AuthenticateRequest);
            application.EndRequest += new EventHandler(application_EndRequest);
        }

        /// <summary>
        /// Handles the AuthenticateRequest event of the application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// The main work is done here. We parse incoming headers, get user name and password from them.
        /// Then we verify the user name and password against configured membership provider.
        /// </remarks>
        void application_AuthenticateRequest(object sender, EventArgs e) {
            HttpApplication app = sender as HttpApplication;

            // Get authentication data
            string authString = app.Request.Headers[ResponseHeaderName];
            if (String.IsNullOrEmpty(authString)) return; // anonymous request
            if (!authString.StartsWith(AuthenticationMethodName, StringComparison.OrdinalIgnoreCase)) return; // not basic auth

            // Get username and password
            string userName, password;
            try {
                authString = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(authString.Substring(6)));
                string[] authParts = authString.Split(new char[] { ':' }, 2);
                userName = authParts[0];
                password = authParts[1];
            }
            catch (Exception ex) {
                throw new System.Security.SecurityException("Invalid format of '" + ResponseHeaderName + "' HTTP header.", ex);
            }

            // Validate user against currently configured membership provider
            if (Membership.ValidateUser(userName, password)) {
                // Success - set user
                app.Context.User = new GenericPrincipal(new GenericIdentity(userName, AuthenticationMethodName), new string[0]);
            }
            else {
                // Failure
                app.Response.StatusCode = AccessDeniedStatusCode;
                app.Response.StatusDescription = AccessDeniedStatusText;
                app.Response.Write(AccessDeniedStatusText);
                app.CompleteRequest();
                return;
            }
        }

        /// <summary>
        /// Handles the EndRequest event of the application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// This methods adds the "WWW-Authenticate" challenge header to all requests ending with the
        /// HTTP status code 401 (Access Denied), set either in this module's application_AuthenticateRequest
        /// or in any other place in application (most likely the authorization module).
        /// </remarks>
        void application_EndRequest(object sender, EventArgs e) {
            // Add WWW-Authenticate header if access denied
            HttpApplication app = sender as HttpApplication;
            if (app.Response.StatusCode == AccessDeniedStatusCode) {
                app.Response.AppendHeader(ChallengeHeaderName, 
                    string.Format(ChallengeHeaderValue, this.config.BasicAuthentication.Realm));
            }
        }

    }
}