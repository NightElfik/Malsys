// Altairis Web Security Toolkit
// Copyright © Michal A. Valasek - Altairis, 2006-2011 | www.altairis.cz 
// Licensed under terms of Microsoft Permissive License (MS-PL)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;

namespace Altairis.Web.Security {

    public class PlainTextMembershipProvider : MembershipProvider {
        private const int DefaultMinRequiredPasswordLength = 8;
        private const int DefaultCacheExpirationTime = 60;      // in minutes
        private const string DefaultDataFilePath = "~/App_Data/users.txt";
        private const string EmailPattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
        private const string CachedCollectionKeyName = "Altairis.Web.Security.PlainTextMembershipProvider.Users";

        private object loadUsersLock = new object();

        #region Initialization and configuration

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
            if (string.IsNullOrEmpty(name)) name = "PlainTextMembershipProvider";
            if (String.IsNullOrEmpty(config["description"])) {
                config.Remove("description");
                config.Add("description", "Altairis plain text membership provider");
            }

            // Initialize base class
            base.Initialize(name, config);

            // Initialize current class
            this.configuration = config;
            this.ApplicationName = GetConfig("applicationName", "");
            this.DataFilePath = GetConfig("dataFilePath", DefaultDataFilePath);
            this.IgnoreInvalidLines = Convert.ToBoolean(GetConfig("ignoreInvalidLines", "true"));
            this.CacheExpirationTime = Convert.ToInt32(GetConfig("cacheExpirationTime", DefaultCacheExpirationTime.ToString()));

            // Validate data file name
            if (!File.Exists(this.DataFileFullPath)) throw new ProviderException("Configuration file '" + this.DataFilePath + "' was not found!");
        }

        /// <summary>
        /// The name of the application using the custom membership provider. This provider ignores this value.
        /// </summary>
        /// <value></value>
        /// <returns>The name of the application using the custom membership provider.</returns>
        public override string ApplicationName { get; set; }

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
            get { return true; }
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
        /// <returns>MembershipPasswordFormat.Clear</returns>
        public override MembershipPasswordFormat PasswordFormat {
            get { return MembershipPasswordFormat.Clear; }
        }

        /// <summary>
        /// Gets the data file path.
        /// </summary>
        /// <value>The data file path.</value>
        public string DataFilePath { get; private set; }

        /// <summary>
        /// Gets full physical path to the data file.
        /// </summary>
        /// <value>The full physical path to data file.</value>
        public string DataFileFullPath {
            get { return HttpContext.Current.Server.MapPath(this.DataFilePath); }
        }

        /// <summary>
        /// Gets a value indicating whether invalid lines in user configuration file are to be ignored. 
        /// If set to <c>false</c>, exception is thrown when data file contains line with invalid formatting.
        /// </summary>
        /// <value><c>true</c> if provider should ignore invalid lines; otherwise, <c>false</c>.</value>
        public bool IgnoreInvalidLines { get; private set; }

        /// <summary>
        /// Gets or sets the cache expiration time for cached users data.
        /// </summary>
        /// <value>The cache expiration time.</value>
        public int CacheExpirationTime { get; private set; }

        #endregion

        // Already implemented

        public bool CheckUserExists(string username) {
            // Validate arguments
            if (username == null) throw new ArgumentNullException("username");
            if (string.IsNullOrEmpty(username)) throw new ArgumentException("Value cannot be null or empty string.", "username");

            // Check user
            return this.LoadUsers().Any(x => x.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline) {
            // Unique identifier for this provider is user name, 
            // so we simply call the other overload of this method
            return this.GetUser(providerUserKey as string, userIsOnline);
        }

        public override MembershipUser GetUser(string username, bool userIsOnline) {
            // Validate arguments
            if (username == null) throw new ArgumentNullException("username");
            if (string.IsNullOrEmpty(username)) throw new ArgumentException("Value cannot be null or empty string.", "username");

            // Get user info
            var u = this.LoadUsers().SingleOrDefault(x => x.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (u == null) return null; // user not found
            return this.CreateMembershipUser(u);
        }

        public override bool ValidateUser(string username, string password) {
            // Validate arguments
            if (username == null) throw new ArgumentNullException("username");
            if (string.IsNullOrEmpty(username)) throw new ArgumentException("Value cannot be null or empty string.", "username");
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty string.", "password");

            // Find user by user name and password
            return this.LoadUsers().Any(x => x.UserName.Equals(username, StringComparison.OrdinalIgnoreCase) && x.Password.Equals(password, StringComparison.Ordinal));
        }

        public override string GetPassword(string username, string answer) {
            // Validate arguments
            if (username == null) throw new ArgumentNullException("username");
            if (string.IsNullOrEmpty(username)) throw new ArgumentException("Value cannot be null or empty string.", "username");
            if (!string.IsNullOrEmpty(answer)) throw new NotSupportedException("Question and answer retrieval not supported.");

            // Get user
            var u = this.LoadUsers().SingleOrDefault(x => x.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (u == null) return null; // user not found
            return u.Password;
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword) {
            // Validate user
            if (!ValidateUser(username, oldPassword)) return false;
            username = username.ToLower();

            // Check if newPassword meets complexivity requirements
            if (!this.CheckPasswordPolicy(newPassword)) return false;
            var args = new ValidatePasswordEventArgs(username, newPassword, true);
            OnValidatingPassword(args);
            if (args.Cancel) {
                if (args.FailureInformation != null) throw args.FailureInformation;
                return false;
            }

            // Update password in database
            var users = this.LoadUsers();
            var user = users.SingleOrDefault(x => x.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));
            user.Password = newPassword;
            this.SaveUsers(users);
            return true;
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status) {
            // Check username
            if (string.IsNullOrEmpty(username)) {
                status = MembershipCreateStatus.InvalidUserName;
                return null;
            }
            username = username.ToLower();
            if (this.CheckUserExists(username)) {
                status = MembershipCreateStatus.DuplicateUserName;
                return null;
            }

            // Check if password meets complexivity requirements
            if (!this.CheckPasswordPolicy(password)) {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }
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

            // Create user
            var users = this.LoadUsers();
            users.Add(new PlainTextUserInfo { UserName = username, Password = password, Email = email });
            this.SaveUsers(users);

            // Return user
            status = MembershipCreateStatus.Success;
            return this.GetUser(username, false);
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData) {
            // Validate arguments
            if (username == null) throw new ArgumentNullException("username");
            if (string.IsNullOrEmpty(username)) throw new ArgumentException("Value cannot be null or empty string.", "username");

            // Get user
            var users = this.LoadUsers();
            var user = users.SingleOrDefault(x => x.UserName == username);
            if (user == null) return false;

            // Remove user
            users.Remove(user);
            this.SaveUsers(users);

            return true;
        }

        public override string GetUserNameByEmail(string email) {
            // Validate arguments
            if (email == null) throw new ArgumentNullException("email");
            if (string.IsNullOrEmpty(email)) throw new ArgumentException("Value cannot be null or empty string.", "email");

            // Find user
            var u = this.LoadUsers().FirstOrDefault(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            if (u == null) return null;
            return u.UserName;
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords) {
            // Validate arguments
            if (pageSize < 1) throw new ArgumentOutOfRangeException("pageSize");
            if (pageIndex < 1) throw new ArgumentOutOfRangeException("pageIndex");

            // Get number of users
            var users = this.LoadUsers();
            totalRecords = users.Count;

            return PageUsers(users, pageIndex, pageSize);
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords) {
            // Validate arguments
            if (emailToMatch == null) throw new ArgumentNullException("emailToMatch");
            if (string.IsNullOrEmpty(emailToMatch)) throw new ArgumentException("Value cannot be null or empty string.", "emailToMatch");
            if (pageSize < 1) throw new ArgumentOutOfRangeException("pageSize");
            if (pageIndex < 1) throw new ArgumentOutOfRangeException("pageIndex");

            // Get number of users
            var users = this.LoadUsers().Where(x => x.Email.Equals(emailToMatch, StringComparison.OrdinalIgnoreCase));
            totalRecords = users.Count();

            // Return paged users
            return PageUsers(users, pageIndex, pageSize);
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords) {
            // Validate arguments
            if (usernameToMatch == null) throw new ArgumentNullException("usernameToMatch");
            if (string.IsNullOrEmpty(usernameToMatch)) throw new ArgumentException("Value cannot be null or empty string.", "usernameToMatch");
            if (pageSize < 1) throw new ArgumentOutOfRangeException("pageSize");
            if (pageIndex < 1) throw new ArgumentOutOfRangeException("pageIndex");

            // Get number of users
            var users = this.LoadUsers().Where(x => x.UserName.Equals(usernameToMatch, StringComparison.OrdinalIgnoreCase));
            totalRecords = users.Count();

            // Return paged users
            return PageUsers(users, pageIndex, pageSize);
        }

        public override string ResetPassword(string username, string answer) {
            // Validate arguments
            if (username == null) throw new ArgumentNullException("username");
            if (string.IsNullOrEmpty(username)) throw new ArgumentException("Value cannot be null or empty string.", "username");
            if (!this.CheckUserExists(username)) throw new MembershipPasswordException("User not found");

            // Generate new password
            string newPassword = Membership.GeneratePassword(this.MinRequiredPasswordLength, this.MinRequiredNonAlphanumericCharacters);

            // Update password in database
            var users = this.LoadUsers();
            var user = users.SingleOrDefault(x => x.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));
            user.Password = newPassword;
            this.SaveUsers(users);
            return newPassword;
        }

        public override void UpdateUser(MembershipUser user) {
            if (user == null) throw new ArgumentNullException("user");

            var users = this.LoadUsers();
            var userInfo = users.SingleOrDefault(x => x.UserName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase));
            if (userInfo != null) {
                userInfo.Email = user.Email;
                this.SaveUsers(users);
            }
        }

        // Not supported

        public override int GetNumberOfUsersOnline() {
            throw new NotSupportedException();
        }

        public override bool UnlockUser(string userName) {
            throw new NotSupportedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer) {
            throw new NotSupportedException();
        }

        // Private support functions

        private string GetConfig(string name, string defaultValue) {
            // Validate input arguments
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("Name");

            // Get value from configuration
            string Value = this.configuration[name];
            if (string.IsNullOrEmpty(Value)) Value = defaultValue;
            return Value;
        }

        private PlainTextUserInfoCollection LoadUsers() {
            // Look for cached user database
            var u = HttpContext.Current.Cache[CachedCollectionKeyName] as PlainTextUserInfoCollection;
            if (u == null) {
                lock (this.loadUsersLock) {
                    // Check if we didn't loaded it already in meantime
                    if (u == null) {

                        // Load user database from file
                        u = PlainTextUserInfoCollection.LoadFromFile(this.DataFileFullPath, this.IgnoreInvalidLines);

                        // Store it into cache
                        HttpContext.Current.Cache.Add(CachedCollectionKeyName,              // Key
                            u,                                                              // Object
                            new System.Web.Caching.CacheDependency(this.DataFileFullPath),  // Dependency
                            System.Web.Caching.Cache.NoAbsoluteExpiration,                  // Absolute expiration
                            new TimeSpan(0, this.CacheExpirationTime, 0),                   // Sliding expiration
                            System.Web.Caching.CacheItemPriority.High,                      // Priority
                            null);                                                          // Callback when removed

                    }
                }
            }
            return u;
        }

        private void SaveUsers(PlainTextUserInfoCollection c) {
            c.SaveToFile(this.DataFileFullPath);
        }

        private MembershipUser CreateMembershipUser(PlainTextUserInfo ui) {
            // Validate arguments
            if (ui == null) throw new ArgumentNullException("ui");

            // Create new membership user
            return new MembershipUser(this.Name, ui.UserName, ui.UserName, ui.Email,
                null, null, true, false,
                DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue);
        }

        private MembershipUserCollection PageUsers(IEnumerable<PlainTextUserInfo> users, int pageIndex, int pageSize) {
            // Validate arguments
            if (pageSize < 1) throw new ArgumentOutOfRangeException("pageSize");
            if (pageIndex < 1) throw new ArgumentOutOfRangeException("pageIndex");

            // Page IEnumerable
            int skip = (pageIndex - 1) * pageSize;
            var paged = users.Skip(skip).Take(pageSize);

            // Create collection
            var muc = new MembershipUserCollection();
            foreach (var user in paged) {
                muc.Add(this.CreateMembershipUser(user));
            }

            return muc;
        }

        private static bool IsEmail(string email) {
            if (string.IsNullOrEmpty(email) || email.Length > 100) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(email, EmailPattern);
        }

    }

    /// <summary>
    /// This class represents all users in plain text data file
    /// </summary>
    internal class PlainTextUserInfoCollection : List<PlainTextUserInfo> {
        private const char DefaultDelimiter = '\t';

        private object saveLock = new object();

        /// <summary>
        /// Loads collection of users from file using the default delimiter, which is the TAB character.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="ignoreInvalidLines">If set to <c>true</c> invalid data lines are ignored. Otherwise, FormatException is thrown</param>
        /// <returns><c>PlainTextUserInfoCollection</c> representing all users in given data file.</returns>
        public static PlainTextUserInfoCollection LoadFromFile(string fileName, bool ignoreInvalidLines) {
            return LoadFromFile(fileName, ignoreInvalidLines, DefaultDelimiter);
        }

        /// <summary>
        /// Loads collection of users from file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="ignoreInvalidLines">If set to <c>true</c> invalid data lines are ignored. Otherwise, FormatException is thrown</param>
        /// <param name="delimiter">The item parts delimiter.</param>
        /// <returns>c>PlainTextUserInfoCollection</c> representing all users in given data file.</returns>
        public static PlainTextUserInfoCollection LoadFromFile(string fileName, bool ignoreInvalidLines, char delimiter) {
            // Validate arguments
            if (fileName == null) throw new ArgumentNullException("fileName");
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentException("Value cannot be null or empty string.", "fileName");

            // Create empty collection
            var c = new PlainTextUserInfoCollection();

            // Read lines
            var lines = File.ReadAllLines(fileName);

            // Parse lines
            for (int i = 0; i < lines.Length; i++) {
                var userData = lines[i].Split(new char[] { delimiter }, 3);
                if (userData.Length < 3) {
                    // Invalid line
                    if (ignoreInvalidLines) continue;
                    throw new FormatException(string.Format("Invalid format of line {0} in user configuration file.", i + 1));
                }

                // Add new user
                c.Add(new PlainTextUserInfo {
                    UserName = userData[0],
                    Email = userData[1],
                    Password = userData[2]
                });
            }

            return c;
        }

        /// <summary>
        /// Saves collection of users to file using the default delimiter, which is the TAB character.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void SaveToFile(string fileName) {
            this.SaveToFile(fileName, DefaultDelimiter);
        }

        /// <summary>
        /// Saves collection of users to file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="delimiter">The delimiter.</param>
        public void SaveToFile(string fileName, char delimiter) {
            // Validate arguments
            if (fileName == null) throw new ArgumentNullException("fileName");
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentException("Value cannot be null or empty string.", "fileName");

            lock (this.saveLock) {
                // Prepare lines to be saved
                var lines = new string[this.Count];
                for (int i = 0; i < this.Count; i++) {
                    lines[i] = this[i].UserName + delimiter + this[i].Email + delimiter + this[i].Password;
                }

                // Save lines to data file
                File.WriteAllLines(fileName, lines);
            }
        }

    }

    /// <summary>
    /// This class represents information about single user in plain text data file.
    /// </summary>
    internal class PlainTextUserInfo {

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the e-mail address.
        /// </summary>
        /// <value>The e-mail address.</value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; set; }

    }

}
