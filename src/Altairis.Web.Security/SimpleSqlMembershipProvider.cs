// Altairis Web Security Toolkit
// Copyright © Michal A. Valasek - Altairis, 2006-2010 | www.altairis.cz 
// Licensed under terms of Microsoft Permissive License (MS-PL)

using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web.Hosting;
using System.Web.Security;

namespace Altairis.Web.Security {

    [Obsolete("Use TableMembershipProvider class instead")]
    public class SimpleSqlMembershipProvider : MembershipProvider {
        private const int DefaultMinRequiredPasswordLength = 8;
        private const string EmailPattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

        #region Initialization and configuration

        private string applicationName, connectionString;
        private NameValueCollection configuration;

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="name">The friendly name of the provider.</param>
        /// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
        /// <exception cref="T:System.ArgumentNullException">The name of the provider is null.</exception>
        /// <exception cref="T:System.ArgumentException">The name of the provider has a length of zero.</exception>
        /// <exception cref="T:System.InvalidOperationException">An attempt is made to call <see cref="M:System.Configuration.Provider.ProviderBase.Initialize(System.String,NameValueCollection)"/> on a provider after the provider has already been initialized.</exception>
        public override void Initialize(string name, NameValueCollection config) {
            // Validate arguments
            if (config == null) throw new ArgumentNullException("config");
            if (string.IsNullOrEmpty(name)) name = "SimpleSqlMembershipProvider";
            if (String.IsNullOrEmpty(config["description"])) {
                config.Remove("description");
                config.Add("description", "Altairis simple SQL membership provider");
            }

            // Initialize base class
            base.Initialize(name, config);

            // Initialize current class
            this.configuration = config;
            System.Configuration.ConnectionStringSettings ConnectionStringSettings = System.Configuration.ConfigurationManager.ConnectionStrings[config["connectionStringName"]];
            if (ConnectionStringSettings == null || ConnectionStringSettings.ConnectionString.Trim() == "") throw new ProviderException("Connection string cannot be blank.");
            this.connectionString = ConnectionStringSettings.ConnectionString;
            this.applicationName = GetConfig("applicationName", "");
        }

        /// <summary>
        /// The name of the application using the custom membership provider. This provider ignores this value.
        /// </summary>
        /// <value></value>
        /// <returns>The name of the application using the custom membership provider.</returns>
        public override string ApplicationName {
            get { return this.applicationName; }
            set { this.applicationName = value; }
        }

        /// <summary>
        /// Indicates that this membership provider allows users to reset their passwords.
        /// </summary>
        /// <value></value>
        /// <returns>true</returns>
        public override bool EnablePasswordReset {
            get { return true; }
        }

        /// <summary>
        /// Indicates that this the membership provider does not allow users to retrieve their passwords.
        /// </summary>
        /// <value></value>
        /// <returns>false</returns>
        public override bool EnablePasswordRetrieval {
            get { return false; }
        }

        /// <summary>
        /// Account lockout is not supported by this provider.
        /// </summary>
        /// <value></value>
        /// <returns>0</returns>
        public override int MaxInvalidPasswordAttempts {
            get { return 0; }
        }

        /// <summary>
        /// Gets the minimum number of special characters that must be present in a valid password.
        /// </summary>
        /// <value></value>
        /// <returns>The minimum number of special characters that must be present in a valid password.</returns>
        public override int MinRequiredNonAlphanumericCharacters {
            get { return System.Convert.ToInt32(this.GetConfig("minRequiredNonAlphanumericCharacters", "0")); }
        }

        /// <summary>
        /// Gets the minimum length required for a password.
        /// </summary>
        /// <value></value>
        /// <returns>The minimum length required for a password. </returns>
        public override int MinRequiredPasswordLength {
            get { return System.Convert.ToInt32(this.GetConfig("minRequiredPasswordLength", DefaultMinRequiredPasswordLength.ToString())); }
        }

        /// <summary>
        /// Account lockout is not supported by this provider.
        /// </summary>
        /// <value></value>
        /// <returns>0</returns>
        public override int PasswordAttemptWindow {
            get { return 0; }
        }

        /// <summary>
        /// Passwor reset questions are not supported by this provider.
        /// </summary>
        /// <value></value>
        /// <returns>false</returns>
        public override bool RequiresQuestionAndAnswer {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require a unique e-mail address for each user name.
        /// </summary>
        /// <value></value>
        /// <returns>true if the membership provider requires a unique e-mail address; otherwise, false. The default is true.</returns>
        public override bool RequiresUniqueEmail {
            get { return System.Convert.ToBoolean(this.GetConfig("requiresUniqueEmail", "true")); }
        }

        /// <summary>
        /// Gets the regular expression used to evaluate a password.
        /// </summary>
        /// <value></value>
        /// <returns>A regular expression used to evaluate a password.</returns>
        public override string PasswordStrengthRegularExpression {
            get { return this.GetConfig("passwordStrengthRegularExpression", ""); }
        }

        /// <summary>
        /// Gets a value indicating the format for storing passwords in the membership data store.
        /// </summary>
        /// <value></value>
        /// <returns>MembershipPasswordFormat.Hashed</returns>
        public override MembershipPasswordFormat PasswordFormat {
            get { return MembershipPasswordFormat.Hashed; }
        }

        #endregion

        /// <summary>
        /// Adds a new membership user to the data source.
        /// </summary>
        /// <param name="username">The user name for the new user.</param>
        /// <param name="password">The password for the new user.</param>
        /// <param name="email">The e-mail address for the new user.</param>
        /// <param name="passwordQuestion">The password question for the new user.</param>
        /// <param name="passwordAnswer">The password answer for the new user</param>
        /// <param name="isApproved">Whether or not the new user is approved to be validated.</param>
        /// <param name="providerUserKey">The unique identifier from the membership data source for the user.</param>
        /// <param name="status">A <see cref="T:System.Web.Security.MembershipCreateStatus"/> enumeration value indicating whether the user was created successfully.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the information for the newly created user.
        /// </returns>
        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status) {

            // Check username
            if (string.IsNullOrEmpty(username) || username.Length > 100) {
                status = MembershipCreateStatus.InvalidUserName;
                return null;
            }
            username = username.ToLower();
            if (this.CheckUserExists(username)) {
                status = MembershipCreateStatus.DuplicateUserName;
                return null;
            }

            // Check if password meets complexity requirements
            ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, password, true);
            OnValidatingPassword(args);
            if (args.Cancel) {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            // Check e-mail
            if (!IsEmail(email)) {
                status = MembershipCreateStatus.InvalidEmail;
                return null;
            }
            email = email.ToLower();
            if (this.RequiresUniqueEmail && !string.IsNullOrEmpty(this.GetUserNameByEmail(email))) {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            // Check Password
            if (password.Length < MinRequiredPasswordLength) {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            int count = 0;

            for (int i = 0; i < password.Length; i++) {
                if (!char.IsLetterOrDigit(password, i)) {
                    count++;
                }
            }

            if (count < MinRequiredNonAlphanumericCharacters) {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (PasswordStrengthRegularExpression.Length > 0) {
                if (!Regex.IsMatch(password, PasswordStrengthRegularExpression)) {
                    status = MembershipCreateStatus.InvalidPassword;
                    return null;
                }
            }

            // Generate hash from password
            string passwordSalt = Membership.GeneratePassword(5, 1);
            string passwordHash = ComputeSHA512(password + passwordSalt);

            // Insert to database
            try {
                using (HostingEnvironment.Impersonate())
                using (SqlConnection db = this.OpenDatabase())
                using (SqlCommand cmd = new SqlCommand("INSERT INTO Users (UserName, PasswordHash, PasswordSalt, Email, Comment, Enabled, DateCreated, DateLastLogin, DateLastActivity, DateLastPasswordChange) VALUES (@UserName, @PasswordHash, @PasswordSalt, @Email, NULL, @Enabled, GETDATE(), NULL, NULL, GETDATE())", db)) {
                    cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 100).Value = username;
                    cmd.Parameters.Add("@PasswordHash", SqlDbType.Char, 86).Value = passwordHash;
                    cmd.Parameters.Add("@PasswordSalt", SqlDbType.Char, 5).Value = passwordSalt;
                    cmd.Parameters.Add("@Email", SqlDbType.VarChar, 100).Value = email;
                    cmd.Parameters.Add("@Enabled", SqlDbType.Bit).Value = isApproved;
                    int rowCount = cmd.ExecuteNonQuery();
                    if (rowCount == 0) status = MembershipCreateStatus.UserRejected;
                    else status = MembershipCreateStatus.Success;
                }
            }
            catch { throw; } // Security context hack for HostingEnvironment.Impersonate

            if (status == MembershipCreateStatus.Success) return this.GetUser(username, false);
            return null;
        }

        /// <summary>
        /// Gets information from the data source for a user. Provides an option to update the last-activity date/time stamp for the user.
        /// </summary>
        /// <param name="username">The name of the user to get information for.</param>
        /// <param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the specified user's information from the data source.
        /// </returns>
        public override MembershipUser GetUser(string username, bool userIsOnline) {
            // Validate arguments
            if (string.IsNullOrEmpty(username) || username.Length > 100) return null;
            username = username.ToLower();

            // Read user information
            try {
                using (HostingEnvironment.Impersonate())
                using (SqlConnection db = this.OpenDatabase())
                using (SqlCommand cmd = new SqlCommand("SELECT UserId, UserName, Email, Comment, Enabled, DateCreated, DateLastLogin, DateLastActivity, DateLastPasswordChange FROM Users WHERE UserName=@UserName", db)) {
                    cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 100).Value = username;
                    using (SqlDataReader r = cmd.ExecuteReader(CommandBehavior.SingleRow)) {
                        if (!r.Read()) return null;
                        MembershipUser u = this.GetUserFromReader(r);
                        if (userIsOnline) this.UpdateLastActivityDate(u.UserName);
                        return u;
                    }
                }
            }
            catch { throw; } // Security context hack for HostingEnvironment.Impersonate
        }

        /// <summary>
        /// Gets user information from the data source based on the unique identifier for the membership user. Provides an option to update the last-activity date/time stamp for the user.
        /// </summary>
        /// <param name="providerUserKey">The unique identifier for the membership user to get information for.</param>
        /// <param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the specified user's information from the data source.
        /// </returns>
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline) {
            // Get userId
            if (providerUserKey == null) return null;
            int userId = System.Convert.ToInt32(providerUserKey);

            // Read user information
            try {
                using (HostingEnvironment.Impersonate())
                using (SqlConnection db = this.OpenDatabase())
                using (SqlCommand cmd = new SqlCommand("SELECT UserId, UserName, Email, Comment, Enabled, DateCreated, DateLastLogin, DateLastActivity, DateLastPasswordChange FROM Users WHERE UserId=@UserId", db)) {
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    using (SqlDataReader r = cmd.ExecuteReader(CommandBehavior.SingleRow)) {
                        if (!r.Read()) return null;
                        MembershipUser u = this.GetUserFromReader(r);
                        if (userIsOnline) this.UpdateLastActivityDate(u.UserName);
                        return u;
                    }
                }
            }
            catch { throw; } // Security context hack for HostingEnvironment.Impersonate
        }

        /// <summary>
        /// Processes a request to update the password question and answer for a membership user.
        /// </summary>
        /// <param name="username">The user to change the password question and answer for.</param>
        /// <param name="password">The password for the specified user.</param>
        /// <param name="newPasswordQuestion">The new password question for the specified user.</param>
        /// <param name="newPasswordAnswer">The new password answer for the specified user.</param>
        /// <returns>
        /// true if the password question and answer are updated successfully; otherwise, false.
        /// </returns>
        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer) {
            throw new ProviderException("Password questions are not implemented in this provider.");
        }

        /// <summary>
        /// Gets the password for the specified user name from the data source.
        /// </summary>
        /// <param name="username">The user to retrieve the password for.</param>
        /// <param name="answer">The password answer for the user.</param>
        /// <returns>
        /// The password for the specified user name.
        /// </returns>
        public override string GetPassword(string username, string answer) {
            throw new ProviderException("Password retrieval is not possible for hashed passwords.");
        }

        /// <summary>
        /// Gets the user name associated with the specified e-mail address.
        /// </summary>
        /// <param name="email">The e-mail address to search for.</param>
        /// <returns>
        /// The user name associated with the specified e-mail address. If no match is found, return null.
        /// </returns>
        public override string GetUserNameByEmail(string email) {
            if (string.IsNullOrEmpty(email) || email.Length > 100) return null;

            try {
                using (HostingEnvironment.Impersonate())
                using (SqlConnection db = this.OpenDatabase())
                using (SqlCommand cmd = new SqlCommand("SELECT UserName FROM Users WHERE Email=@Email", db)) {
                    cmd.Parameters.Add("@Email", SqlDbType.VarChar, 100).Value = email.ToLower();
                    return (string)cmd.ExecuteScalar() ?? null;
                }
            }
            catch { throw; } // Security context hack for HostingEnvironment.Impersonate
        }

        /// <summary>
        /// Verifies that the specified user name and password exist in the data source.
        /// </summary>
        /// <param name="username">The name of the user to validate.</param>
        /// <param name="password">The password for the specified user.</param>
        /// <returns>
        /// true if the specified username and password are valid; otherwise, false.
        /// </returns>
        public override bool ValidateUser(string username, string password) {
            // Validate arguments
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || username.Length > 100) return false;
            username = username.ToLower();

            // Get password hash and salt for username
            string passwordHash, passwordSalt;
            try {
                using (HostingEnvironment.Impersonate())
                using (SqlConnection db = this.OpenDatabase())
                using (SqlCommand cmd = new SqlCommand("SELECT PasswordHash, PasswordSalt FROM Users WHERE UserName=@UserName AND Enabled=1", db)) {
                    cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 100).Value = username;
                    using (SqlDataReader r = cmd.ExecuteReader(CommandBehavior.SingleRow)) {
                        if (!r.Read()) return false; // User not found
                        passwordHash = r["PasswordHash"] as string;
                        passwordSalt = r["PasswordSalt"] as string;
                    }
                }
            }
            catch { throw; } // Security context hack for HostingEnvironment.Impersonate

            // Validate password
            if (!ComputeSHA512(password + passwordSalt).Equals(passwordHash, StringComparison.Ordinal)) return false; // invalid password

            // Password is valid
            this.UpdateLastLoginDate(username);
            return true;
        }

        /// <summary>
        /// Processes a request to update the password for a membership user.
        /// </summary>
        /// <param name="username">The user to update the password for.</param>
        /// <param name="oldPassword">The current password for the specified user.</param>
        /// <param name="newPassword">The new password for the specified user.</param>
        /// <returns>
        /// true if the password was updated successfully; otherwise, false.
        /// </returns>
        public override bool ChangePassword(string username, string oldPassword, string newPassword) {
            // Validate user
            if (!ValidateUser(username, oldPassword)) return false;
            username = username.ToLower();

            // Check if newPassword meets complexivity requirements
            if (newPassword.Length < MinRequiredPasswordLength) throw new ArgumentException(String.Format("The length of parameter 'newPassword' needs to be greater or equal to '{0}'.", MinRequiredPasswordLength.ToString(CultureInfo.InvariantCulture)));
            int count = 0;
            for (int i = 0; i < newPassword.Length; i++) {
                if (!char.IsLetterOrDigit(newPassword, i)) count++;
            }
            if (count < MinRequiredNonAlphanumericCharacters) throw new ArgumentException(String.Format("Non alpha numeric characters in 'newPassword' needs to be greater than or equal to '{0}'.", MinRequiredNonAlphanumericCharacters.ToString(CultureInfo.InvariantCulture)));
            if (!string.IsNullOrEmpty(PasswordStrengthRegularExpression) && !Regex.IsMatch(newPassword, PasswordStrengthRegularExpression)) throw new ArgumentException("The parameter 'newPassword' does not match the regular expression specified in config file.");
            ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, newPassword, true);
            OnValidatingPassword(args);
            if (args.Cancel) {
                if (args.FailureInformation != null) throw args.FailureInformation;
                else throw new MembershipPasswordException("Change password canceled due to new password validation failure.");
            }

            // Update password in database
            return this.SetPassword(username, newPassword);
        }

        /// <summary>
        /// Resets a user's password to a new, automatically generated password.
        /// </summary>
        /// <param name="username">The user to reset the password for.</param>
        /// <param name="answer">The password answer for the specified user.</param>
        /// <returns>The new password for the specified user.</returns>
        public override string ResetPassword(string username, string answer) {
            // Check if user exists
            if (!this.CheckUserExists(username)) throw new MembershipPasswordException("User not found");
            username = username.ToLower();

            // Generate new password
            string newPassword = Membership.GeneratePassword(this.MinRequiredPasswordLength, this.MinRequiredNonAlphanumericCharacters);

            // Reset password
            this.SetPassword(username, newPassword);
            return newPassword;
        }

        /// <summary>
        /// Unlocks the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns><c>true</c> if user was now unlocked, <c>false</c> otherwise.</returns>
        public override bool UnlockUser(string username) {
            // Check if user exists
            if (!this.CheckUserExists(username)) return false;
            username = username.ToLower();

            // Update password in database
            try {
                using (HostingEnvironment.Impersonate())
                using (SqlConnection db = this.OpenDatabase())
                using (SqlCommand cmd = new SqlCommand("UPDATE Users SET Enabled=1 WHERE UserName=@UserName AND Enabled=0", db)) {
                    cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 100).Value = username;
                    return cmd.ExecuteNonQuery() != 0;
                }
            }
            catch { throw; } // Security context hack for HostingEnvironment.Impersonate
        }

        /// <summary>
        /// Removes a user from the membership data source.
        /// </summary>
        /// <param name="username">The name of the user to delete.</param>
        /// <param name="deleteAllRelatedData">true to delete data related to the user from the database; false to leave data related to the user in the database.</param>
        /// <returns>
        /// <c>true</c> if the user was successfully deleted; otherwise, <c>false</c>.
        /// </returns>
        public override bool DeleteUser(string username, bool deleteAllRelatedData) {
            // Check if user exists
            if (!this.CheckUserExists(username)) return false;
            username = username.ToLower();

            // Delete user data
            try {
                using (HostingEnvironment.Impersonate())
                using (SqlConnection db = this.OpenDatabase())
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Users WHERE UserName=@UserName", db)) {
                    cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 100).Value = username;
                    return cmd.ExecuteNonQuery() != 0;
                }
            }
            catch { throw; } // Security context hack for HostingEnvironment.Impersonate
        }

        /// <summary>
        /// Gets a collection of membership users where the e-mail address contains the specified e-mail address to match.
        /// </summary>
        /// <param name="emailToMatch">The e-mail address to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.MembershipUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
        /// </returns>
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords) {
            // Validate arguments
            if (emailToMatch == null) emailToMatch = string.Empty;
            if (pageIndex < 0) throw new ArgumentOutOfRangeException("pageIndex");
            if (pageSize < 1) throw new ArgumentOutOfRangeException("pageSize");
            emailToMatch = emailToMatch.ToLower().Trim();

            // Get table with users
            using (DataTable userTable = new DataTable()) {
                try {
                    using (HostingEnvironment.Impersonate())
                    using (SqlConnection db = this.OpenDatabase())
                    using (SqlCommand cmd = new SqlCommand("SELECT UserId, UserName, Email, Comment, Enabled, DateCreated, DateLastLogin, DateLastActivity, DateLastPasswordChange FROM Users WHERE Email LIKE @Email", db)) {
                        cmd.Parameters.Add("@Email", SqlDbType.VarChar, 100).Value = emailToMatch;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) da.Fill(userTable);
                    }
                }
                catch { throw; } // Security context hack for HostingEnvironment.Impersonate

                // Get bounds
                totalRecords = userTable.Rows.Count;
                int startIndex = pageIndex * pageSize;
                int endIndex = startIndex + pageSize - 1;

                // Get number of users
                if (totalRecords == 0 || startIndex > totalRecords) return new MembershipUserCollection(); // no users
                return this.GetUsersFromDataTable(userTable, startIndex, endIndex);
            }
        }

        /// <summary>
        /// Gets a collection of membership users where the user name contains the specified user name to match.
        /// </summary>
        /// <param name="usernameToMatch">The user name to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.MembershipUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
        /// </returns>
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords) {
            // Validate arguments
            if (string.IsNullOrEmpty(usernameToMatch)) throw new ArgumentNullException("usernameToMatch");
            if (pageIndex < 0) throw new ArgumentOutOfRangeException("pageIndex");
            if (pageSize < 1) throw new ArgumentOutOfRangeException("pageSize");
            usernameToMatch = usernameToMatch.ToLower().Trim();

            // Get table with users
            using (DataTable userTable = new DataTable()) {
                try {
                    using (HostingEnvironment.Impersonate())
                    using (SqlConnection db = this.OpenDatabase())
                    using (SqlCommand cmd = new SqlCommand("SELECT UserId, UserName, Email, Comment, Enabled, DateCreated, DateLastLogin, DateLastActivity, DateLastPasswordChange FROM Users WHERE UserName LIKE @UserName", db)) {
                        cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 100).Value = usernameToMatch;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) da.Fill(userTable);
                    }
                }
                catch { throw; } // Security context hack for HostingEnvironment.Impersonate

                // Get bounds
                totalRecords = userTable.Rows.Count;
                int startIndex = pageIndex * pageSize;
                int endIndex = startIndex + pageSize - 1;

                // Get number of users
                if (totalRecords == 0 || startIndex > totalRecords) return new MembershipUserCollection(); // no users
                return this.GetUsersFromDataTable(userTable, startIndex, endIndex);
            }
        }

        /// <summary>
        /// Gets a collection of all the users in the data source in pages of data.
        /// </summary>
        /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.MembershipUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
        /// </returns>
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords) {
            // Validate arguments
            if (pageIndex < 0) throw new ArgumentOutOfRangeException("pageIndex");
            if (pageSize < 1) throw new ArgumentOutOfRangeException("pageSize");

            // Get table with users
            using (DataTable userTable = new DataTable()) {
                try {
                    using (HostingEnvironment.Impersonate())
                    using (SqlConnection db = this.OpenDatabase())
                    using (SqlDataAdapter da = new SqlDataAdapter("SELECT UserId, UserName, Email, Comment, Enabled, DateCreated, DateLastLogin, DateLastActivity, DateLastPasswordChange FROM Users", db)) {
                        da.Fill(userTable);
                    }
                }
                catch { throw; } // Security context hack for HostingEnvironment.Impersonate

                // Get bounds
                totalRecords = userTable.Rows.Count;
                int startIndex = pageIndex * pageSize;
                int endIndex = startIndex + pageSize;

                // Get number of users
                if (totalRecords == 0 || startIndex > totalRecords) return new MembershipUserCollection(); // no users
                return this.GetUsersFromDataTable(userTable, startIndex, endIndex);
            }
        }

        /// <summary>
        /// Gets the number of users currently accessing the application.
        /// </summary>
        /// <returns>
        /// The number of users currently accessing the application.
        /// </returns>
        public override int GetNumberOfUsersOnline() {
            try {
                using (HostingEnvironment.Impersonate())
                using (SqlConnection db = this.OpenDatabase())
                using (SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM Users WHERE LastActivityDate > @LastActivityDate", db)) {
                    cmd.Parameters.Add("@LastActivityDate", SqlDbType.DateTime).Value = DateTime.Now.AddMinutes(-Membership.UserIsOnlineTimeWindow);
                    return (int)cmd.ExecuteScalar();
                }
            }
            catch { throw; } // Security context hack for HostingEnvironment.Impersonate
        }

        /// <summary>
        /// Updates information about a user in the data source.
        /// </summary>
        /// <param name="user">A <see cref="T:System.Web.Security.MembershipUser"/> object that represents the user to update and the updated information for the user.</param>
        public override void UpdateUser(MembershipUser user) {
            // Validate arguments
            if (user == null) throw new ArgumentNullException("user");
            if (!IsEmail(user.Email)) throw new ArgumentException("E-mail is invalid", "user");
            if (this.RequiresUniqueEmail) {
                string username = this.GetUserNameByEmail(user.Email);
                if (!string.IsNullOrEmpty(username) && username != user.UserName)
                    throw new ArgumentException("E-mail is not unique", "user");
            }
            if (!this.CheckUserExists(user.UserName)) throw new ArgumentException("User not found", "user");

            // Update database
            try {
                using (HostingEnvironment.Impersonate())
                using (SqlConnection db = this.OpenDatabase())
                using (SqlCommand cmd = new SqlCommand("UPDATE Users SET Email=@Email, Comment=@Comment, Enabled=@Enabled WHERE UserName=@UserName", db)) {
                    cmd.Parameters.Add("@Email", SqlDbType.VarChar, 100).Value = user.Email.ToLower();
                    cmd.Parameters.Add("@Comment", SqlDbType.Text).Value = user.Comment;
                    cmd.Parameters.Add("@Enabled", SqlDbType.Bit).Value = user.IsApproved;
                    cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 100).Value = user.UserName.ToLower();
                    cmd.ExecuteNonQuery();
                }
            }
            catch { throw; } // Security context hack for HostingEnvironment.Impersonate
        }

        // Public extension functions

        /// <summary>
        /// Checks if the user exists.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns><c>true</c> if the user with given name exists; otherwise returns <c>false</c>.</returns>
        public bool CheckUserExists(string username) {
            if (string.IsNullOrEmpty(username)) return false;
            if (username.Length > 100) throw new ArgumentOutOfRangeException("username", "Maximum length of 100 characters exceeded");

            try {
                using (HostingEnvironment.Impersonate())
                using (SqlConnection db = this.OpenDatabase())
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE UserName=@UserName", db)) {
                    cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 100).Value = username.ToLower();
                    return (int)cmd.ExecuteScalar() == 1;
                }
            }
            catch { throw; } // Security context hack for HostingEnvironment.Impersonate
        }

        // Private support functions

        private MembershipUserCollection GetUsersFromDataTable(DataTable userTable, int startIndex, int endIndex) {
            // Validate arguments
            if (userTable == null) throw new ArgumentNullException("userTable");
            if (startIndex < 0) throw new ArgumentOutOfRangeException("startIndex");
            if (endIndex < startIndex) throw new ArgumentOutOfRangeException("endIndex");
            if (endIndex > userTable.Rows.Count) endIndex = userTable.Rows.Count;

            // Read selected users
            MembershipUserCollection uc = new MembershipUserCollection();
            for (int i = startIndex; i < endIndex; i++) {
                string username = userTable.Rows[i]["UserName"] as string;
                object providerUserKey = userTable.Rows[i]["UserId"];
                string email = userTable.Rows[i]["Email"] as string;
                string comment = ""; if (userTable.Rows[i]["Comment"] != DBNull.Value) comment = userTable.Rows[i]["Comment"] as string;
                bool isApproved = System.Convert.ToBoolean(userTable.Rows[i]["Enabled"]);
                bool isLockedOut = !System.Convert.ToBoolean(userTable.Rows[i]["Enabled"]);
                DateTime creationDate = System.Convert.ToDateTime(userTable.Rows[i]["DateCreated"]);
                DateTime lastLoginDate = new DateTime(); if (userTable.Rows[i]["DateLastLogin"] != DBNull.Value) lastLoginDate = System.Convert.ToDateTime(userTable.Rows[i]["DateLastLogin"]);
                DateTime lastActivityDate = new DateTime(); if (userTable.Rows[i]["DateLastActivity"] != DBNull.Value) lastActivityDate = System.Convert.ToDateTime(userTable.Rows[i]["DateLastActivity"]);
                DateTime lastPasswordChangedDate = System.Convert.ToDateTime(userTable.Rows[i]["DateLastPasswordChange"]);
                uc.Add(new MembershipUser(this.Name, username, providerUserKey, email, string.Empty, comment, isApproved, isLockedOut, creationDate, lastLoginDate, lastActivityDate, lastPasswordChangedDate, new DateTime()));
            }

            return uc;
        }

        private MembershipUser GetUserFromReader(SqlDataReader reader) {
            string username = System.Convert.ToString(reader["UserName"]);
            object providerUserKey = reader["UserId"];
            string email = System.Convert.ToString(reader["Email"]);
            string comment = ""; if (reader["Comment"] != DBNull.Value) comment = System.Convert.ToString(reader["Comment"]);
            bool isApproved = System.Convert.ToBoolean(reader["Enabled"]);
            bool isLockedOut = !System.Convert.ToBoolean(reader["Enabled"]);
            DateTime creationDate = System.Convert.ToDateTime(reader["DateCreated"]);
            DateTime lastLoginDate = new DateTime(); if (reader["DateLastLogin"] != DBNull.Value) lastLoginDate = System.Convert.ToDateTime(reader["DateLastLogin"]);
            DateTime lastActivityDate = new DateTime(); if (reader["DateLastActivity"] != DBNull.Value) lastActivityDate = System.Convert.ToDateTime(reader["DateLastActivity"]);
            DateTime lastPasswordChangedDate = System.Convert.ToDateTime(reader["DateLastPasswordChange"]);
            return new MembershipUser(this.Name, username, providerUserKey, email, string.Empty, comment, isApproved, isLockedOut, creationDate, lastLoginDate, lastActivityDate, lastPasswordChangedDate, new DateTime());
        }

        private bool SetPassword(string username, string password) {
            // Generate new password hash and salt
            string passwordSalt = Membership.GeneratePassword(5, 1);
            string passwordHash = ComputeSHA512(password + passwordSalt);

            // Update password in database
            try {
                using (HostingEnvironment.Impersonate())
                using (SqlConnection db = this.OpenDatabase())
                using (SqlCommand cmd = new SqlCommand("UPDATE Users SET PasswordHash=@PasswordHash, PasswordSalt=@PasswordSalt, DateLastActivity=GETDATE(), DateLastPasswordChange=GETDATE() WHERE UserName=@UserName", db)) {
                    cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 100).Value = username;
                    cmd.Parameters.Add("@PasswordHash", SqlDbType.Char, 86).Value = passwordHash;
                    cmd.Parameters.Add("@PasswordSalt", SqlDbType.Char, 5).Value = passwordSalt;
                    return cmd.ExecuteNonQuery() != 0;
                }
            }
            catch { throw; } // Security context hack for HostingEnvironment.Impersonate
        }

        private void UpdateLastActivityDate(string username) {
            if (string.IsNullOrEmpty(username)) throw new ArgumentNullException("username");
            if (username.Length > 100) throw new ArgumentOutOfRangeException("username", "Maximum length of 100 characters exceeded");

            try {
                using (HostingEnvironment.Impersonate())
                using (SqlConnection db = this.OpenDatabase())
                using (SqlCommand cmd = new SqlCommand("UPDATE Users SET DateLastActivity=GETDATE() WHERE UserName=@UserName", db)) {
                    cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 100).Value = username.ToLower();
                    cmd.ExecuteNonQuery();
                }
            }
            catch { throw; } // Security context hack for HostingEnvironment.Impersonate
        }

        private void UpdateLastLoginDate(string username) {
            if (string.IsNullOrEmpty(username)) throw new ArgumentNullException("username");
            if (username.Length > 100) throw new ArgumentOutOfRangeException("username", "Maximum length of 100 characters exceeded");

            try {
                using (HostingEnvironment.Impersonate())
                using (SqlConnection db = this.OpenDatabase())
                using (SqlCommand cmd = new SqlCommand("UPDATE Users SET DateLastLogin=GETDATE(), DateLastActivity=GETDATE() WHERE UserName=@UserName", db)) {
                    cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 100).Value = username.ToLower();
                    cmd.ExecuteNonQuery();
                }
            }
            catch { throw; } // Security context hack for HostingEnvironment.Impersonate
        }

        private static bool IsEmail(string email) {
            if (string.IsNullOrEmpty(email) || email.Length > 100) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(email, EmailPattern);
        }

        private SqlConnection OpenDatabase() {
            SqlConnection db = new SqlConnection(this.connectionString);
            db.Open();
            return db;
        }

        private string GetConfig(string name, string defaultValue) {
            // Validate input arguments
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("Name");

            // Get value from configuration
            string Value = this.configuration[name];
            if (string.IsNullOrEmpty(Value)) Value = defaultValue;
            return Value;
        }

        private static string ComputeSHA512(string s) {
            if (string.IsNullOrEmpty(s)) throw new ArgumentNullException();
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(s);
            buffer = System.Security.Cryptography.SHA512Managed.Create().ComputeHash(buffer);
            return System.Convert.ToBase64String(buffer).Substring(0, 86); // strip padding
        }

    }

}