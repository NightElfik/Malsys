// Altairis Web Security Toolkit
// Copyright © Michal A. Valasek - Altairis, 2006-2011 | www.altairis.cz 
// Licensed under terms of Microsoft Permissive License (MS-PL)

using System;
using System.Configuration;
using System.Configuration.Provider;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Web.Hosting;
using System.Web.Security;
using System.Linq;

namespace Altairis.Web.Security {
    public class TableMembershipProvider : MembershipProvider {
        private const string DEFAULT_TABLE_NAME = "Users";
        private const string EMAIL_VALIDATION_PATTERN = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

        private string passwordStrengthRegularExpression;
        private int minRequiredNonAlphanumericCharacters, minRequiredPasswordLength;
        private bool requiresUniqueEmail;
        private ConnectionStringSettings connectionString;

        public enum ProviderUserKeyType {
            UserName,
            IntIdentity,
            Guid
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config) {
            // Perform basic initialization
            base.Initialize(name, config);

            // Read default configuration
            this.ApplicationName = config.GetConfigValue("applicationName", null);
            this.minRequiredNonAlphanumericCharacters = config.GetConfigValue("minRequiredNonAlphanumericCharacters", 1);
            this.minRequiredPasswordLength = config.GetConfigValue("minRequiredPasswordLength", 7);
            this.passwordStrengthRegularExpression = config.GetConfigValue("passwordStrengthRegularExpression", null);
            this.requiresUniqueEmail = config.GetConfigValue("requiresUniqueEmail", false);

            // Read connection string
            this.ConnectionStringName = config.GetConfigValue("connectionStringName", null);
            if (string.IsNullOrWhiteSpace(this.ConnectionStringName)) throw new ConfigurationErrorsException("Required \"connectionStringName\" attribute not specified.");
            this.connectionString = ConfigurationManager.ConnectionStrings[this.ConnectionStringName];
            if (this.connectionString == null) throw new ConfigurationErrorsException(string.Format("Connection string \"{0}\" was not found.", this.ConnectionStringName));
            if (string.IsNullOrEmpty(this.connectionString.ProviderName)) throw new ConfigurationErrorsException(string.Format("Connection string \"{0}\" does not have specified the \"providerName\" attribute.", this.ConnectionStringName));

            // Read other configuration
            this.TableName = config.GetConfigValue("tableName", DEFAULT_TABLE_NAME);
            this.UserKeyType = config.GetConfigValue("userKeyType", ProviderUserKeyType.UserName);
            this.UseEmailAddressAsUserName = config.GetConfigValue("useEmailAddressAsUserName", false);
            if (this.UseEmailAddressAsUserName && !this.RequiresUniqueEmail) throw new ConfigurationErrorsException("When setting useEmailAddressAsUserName to true, requiresUniqueEmail must be set to true as well.");

            // Throw error on excess attributes
            if (config.Count != 0) throw new ConfigurationErrorsException("Unrecognized configuration attributes found: " + string.Join(", ", config.AllKeys));

            // Quote table name
            if (this.TableName.IndexOf('[') == -1) this.TableName = string.Join(".", this.TableName.Split('.').Select(x => "[" + x + "]").ToArray());
        }

        #region Configuration properties

        // Custom properties

        public string ConnectionStringName { get; private set; }

        public string TableName { get; private set; }

        public ProviderUserKeyType UserKeyType { get; private set; }

        public bool UseEmailAddressAsUserName { get; private set; }

        // The following properties are inherited from MembershipProvider and fully supported

        public override int MinRequiredNonAlphanumericCharacters {
            get {
                return this.minRequiredNonAlphanumericCharacters;
            }
        }

        public override int MinRequiredPasswordLength {
            get {
                return this.minRequiredPasswordLength;
            }
        }

        public override string PasswordStrengthRegularExpression {
            get {
                return this.passwordStrengthRegularExpression;
            }
        }

        public override bool RequiresUniqueEmail {
            get {
                return this.requiresUniqueEmail;
            }
        }

        public override string ApplicationName { get; set; }

        // The following properties are inherited from MembershipProvider and fixed by provider implementation

        public override bool EnablePasswordReset {
            get { return true; }
        }

        public override bool EnablePasswordRetrieval {
            get { return false; }
        }

        public override MembershipPasswordFormat PasswordFormat {
            get { return MembershipPasswordFormat.Hashed; }
        }

        public override bool RequiresQuestionAndAnswer {
            get { return false; }
        }

        // The following properties are inherited from MembershipProvider and are not implemented

        public override int MaxInvalidPasswordAttempts {
            get { throw new NotImplementedException("Account lockdown is intentionally not implemented due to possibility of DoS attack."); }
        }

        public override int PasswordAttemptWindow {
            get { throw new NotImplementedException("Account lockdown is intentionally not implemented due to possibility of DoS attack."); }
        }

        #endregion

        // Membership provider methods

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status) {
            // Validate arguments
            if (username == null) throw new ArgumentNullException("username");
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "username");
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            // Validate providerUserKey
            if (this.UserKeyType == ProviderUserKeyType.Guid) {
                if (providerUserKey == null) {
                    // Create new random GUID
                    providerUserKey = Guid.NewGuid();
                }
                else if (!(providerUserKey is Guid)) {
                    // Invalid key (not GUID)
                    status = MembershipCreateStatus.InvalidProviderUserKey;
                    return null;
                }
                else if (this.GetUser(providerUserKey, false) != null) {
                    // Duplicate key
                    status = MembershipCreateStatus.DuplicateProviderUserKey;
                    return null;
                }
            }
            else if (providerUserKey != null) {
                // Do not allow arbitrary user key when not using GUIDs
                status = MembershipCreateStatus.InvalidProviderUserKey;
                return null;
            }

            // Validate and normalize username
            username = username.ToLower().Trim();
            if (username.Length > 100) {
                status = MembershipCreateStatus.InvalidUserName;
                return null;
            }

            // Validate password (local)
            if (!this.CheckPasswordPolicy(password)) {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            // Validate password (external)
            var args = new ValidatePasswordEventArgs(username, password, true);
            this.OnValidatingPassword(args);
            if (args.Cancel) {
                if (args.FailureInformation != null) throw args.FailureInformation;
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            // Validate  e-mail address
            if (this.RequiresUniqueEmail && string.IsNullOrWhiteSpace(email)) {
                // E-mail is not specified when provider requires unique e-mail addresses
                status = MembershipCreateStatus.InvalidEmail;
                return null;
            }
            if (!string.IsNullOrWhiteSpace(email)) {
                email = email.ToLower().Trim();
                if (!Regex.IsMatch(email, EMAIL_VALIDATION_PATTERN)) {
                    // Invalid e-mail syntax
                    status = MembershipCreateStatus.InvalidEmail;
                    return null;
                }
                if (this.RequiresUniqueEmail && !string.IsNullOrEmpty(this.GetUserNameByEmail(email))) {
                    // Duplicate e-mail
                    status = MembershipCreateStatus.DuplicateEmail;
                    return null;
                }
            }

            // Check username/e-mail address when UseEmailAddressAsUserName=true
            if (this.UseEmailAddressAsUserName && !username.Equals(email)) {
                status = MembershipCreateStatus.InvalidUserName;
                return null;
            }

            // Check if we don't already have such user
            if (this.GetUser(username, false) != null) {
                status = MembershipCreateStatus.DuplicateUserName;
                return null;
            }

            // Hash password
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            // Store in database
            var sql = "INSERT INTO $Users (UserName, PasswordHash, PasswordSalt, Email, IsApproved, DateCreated, DateLastPasswordChange) VALUES (@UserName, @PasswordHash, @PasswordSalt, @Email, @IsApproved, @DateCreated, @DateLastPasswordChange)";
            if (this.UserKeyType == ProviderUserKeyType.Guid) sql = "INSERT INTO $Users (UserId, UserName, PasswordHash, PasswordSalt, Email, IsApproved, DateCreated, DateLastPasswordChange) VALUES (@UserId, @UserName, @PasswordHash, @PasswordSalt, @Email, @IsApproved, @DateCreated, @DateLastPasswordChange)";
            using (HostingEnvironment.Impersonate())
            using (var db = this.connectionString.CreateDbConnection())
            using (var cmd = this.CreateDbCommand(sql, db)) {
                if (this.UserKeyType == ProviderUserKeyType.Guid) cmd.AddParameterWithValue("@UserId", (Guid)providerUserKey);
                cmd.AddParameterWithValue("@UserName", username);
                cmd.AddParameterWithValue("@PasswordHash", passwordHash);
                cmd.AddParameterWithValue("@PasswordSalt", passwordSalt);
                cmd.AddParameterWithValue("@Email", email);
                cmd.AddParameterWithValue("@IsApproved", isApproved);
                cmd.AddParameterWithValue("@DateCreated", DateTime.Now);
                cmd.AddParameterWithValue("@DateLastPasswordChange", DateTime.Now);

                db.Open();
                if (cmd.ExecuteNonQuery() == 0) {
                    status = MembershipCreateStatus.UserRejected;
                    return null;
                }
            }
            status = MembershipCreateStatus.Success;
            return this.GetUser(username, false);
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword) {
            // Validate arguments
            if (username == null) throw new ArgumentNullException("username");
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "username");
            if (oldPassword == null) throw new ArgumentNullException("oldPassword");
            if (string.IsNullOrWhiteSpace(oldPassword)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "oldPassword");
            if (newPassword == null) throw new ArgumentNullException("newPassword");
            if (string.IsNullOrWhiteSpace(newPassword)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "newPassword");

            // Validate old password
            if (!ValidateUser(username, oldPassword)) return false;

            // Validate new password (local)
            if (!this.CheckPasswordPolicy(newPassword)) return false;

            // Validate new password (external)
            var args = new ValidatePasswordEventArgs(username, newPassword, true);
            this.OnValidatingPassword(args);
            if (args.Cancel) {
                if (args.FailureInformation != null) throw args.FailureInformation;
                return false;
            }

            // Set password
            this.SetPassword(username, newPassword);
            return true;
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData) {
            // Validate arguments
            if (username == null) throw new ArgumentNullException("username");
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "username");

            // Update database
            try {
                using (HostingEnvironment.Impersonate())
                using (var db = this.connectionString.CreateDbConnection())
                using (var cmd = this.CreateDbCommand("DELETE FROM $Users WHERE UserName = @UserName ", db)) {
                    cmd.AddParameterWithValue("@UserName", username);
                    db.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex) {
                throw new ProviderException("Error while performing database query.", ex);
            }
        }

        public override int GetNumberOfUsersOnline() {
            // Query database
            try {
                using (HostingEnvironment.Impersonate())
                using (var db = this.connectionString.CreateDbConnection())
                using (var cmd = this.CreateDbCommand("SELECT COUNT(*) FROM $Users WHERE DateLastActivity > @DateLastActivity", db)) {
                    cmd.AddParameterWithValue("@DateLastActivity", DateTime.Now.AddMinutes(-Membership.UserIsOnlineTimeWindow));
                    db.Open();
                    return (int)cmd.ExecuteScalar();

                }
            }
            catch (Exception ex) {
                throw new ProviderException("Error while performing database query.", ex);
            }
        }

        public override string GetUserNameByEmail(string email) {
            // Validate arguments
            if (email == null) throw new ArgumentNullException("email");
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "email");

            // Update database
            try {
                using (HostingEnvironment.Impersonate())
                using (var db = this.connectionString.CreateDbConnection())
                using (var cmd = this.CreateDbCommand("SELECT UserName FROM $Users WHERE Email = @Email", db)) {
                    cmd.AddParameterWithValue("@Email", email.ToLower().Trim());
                    db.Open();
                    return cmd.ExecuteScalar() as string;
                }
            }
            catch (Exception ex) {
                throw new ProviderException("Error while performing database query.", ex);
            }
        }

        public override string ResetPassword(string username, string answer) {
            if (username == null) throw new ArgumentNullException("username");
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "username");

            // Create new password
            var newPasswordLength = Math.Max(this.MinRequiredPasswordLength, 8);
            var newPassword = Membership.GeneratePassword(newPasswordLength, this.minRequiredNonAlphanumericCharacters);

            // Set password 
            this.SetPassword(username, newPassword);
            return newPassword;
        }

        public override bool ValidateUser(string username, string password) {
            // Validate arguments
            if (username == null) throw new ArgumentNullException("username");
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "username");
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            // Normalize arguments
            username = username.ToLower().Trim();

            // Query database
            try {
                using (HostingEnvironment.Impersonate())
                using (var db = this.connectionString.CreateDbConnection())
                using (var cmd = this.CreateDbCommand("SELECT PasswordHash, PasswordSalt FROM $Users WHERE UserName = @UserName AND IsApproved = 1", db)) {
                    cmd.AddParameterWithValue("@UserName", username);
                    db.Open();

                    // Validate password
                    using (var r = cmd.ExecuteReader()) {
                        if (!r.Read()) return false;   // User not found or disabled
                        var passwordHash = (byte[])r["PasswordHash"];
                        var passwordSalt = (byte[])r["PasswordSalt"];
                        if (!VerifyPasswordHash(password, passwordHash, passwordSalt)) return false;
                    }

                    // Update last login date
                    cmd.CommandText = "UPDATE " + this.TableName + " SET DateLastLogin = @DateLastLogin WHERE UserName = @UserName";
                    cmd.AddParameterWithValue("@DateLastLogin", DateTime.Now);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex) {
                throw new ProviderException("Error while performing database query.", ex);
            }
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline) {
            // Validate arguments
            switch (this.UserKeyType) {
                case ProviderUserKeyType.IntIdentity:
                    if (!(providerUserKey is int)) throw new ArgumentException("Invalid type, expected int.", "providerUserKey");
                    break;
                case ProviderUserKeyType.Guid:
                    if (!(providerUserKey is Guid)) throw new ArgumentException("Invalid type, expected Guid.", "providerUserKey");
                    break;
                case ProviderUserKeyType.UserName:
                default:
                    if (!(providerUserKey is string)) throw new ArgumentException("Invalid type, expected string.", "providerUserKey");
                    return GetUser(providerUserKey as string, userIsOnline);
            }

            // Query database
            try {
                using (HostingEnvironment.Impersonate())
                using (var db = this.connectionString.CreateDbConnection())
                using (var cmd = this.CreateDbCommand("SELECT * FROM $Users WHERE UserId = @UserId", db)) {
                    if (this.UserKeyType == ProviderUserKeyType.IntIdentity) {
                        cmd.AddParameterWithValue("@UserId", (int)providerUserKey);
                    }
                    else {
                        cmd.AddParameterWithValue("@UserId", (Guid)providerUserKey);
                    }
                    db.Open();
                    using (var r = cmd.ExecuteReader()) {
                        if (!r.Read()) return null; // user not found

                        // Get user
                        var u = this.GetUserFromReader(r);
                        r.Close();

                        // Update last activity date
                        if (userIsOnline) {
                            cmd.CommandText = "UPDATE " + this.TableName + " SET DateLastActivity = @DateLastActivity WHERE UserName = @UserName";
                            cmd.AddParameterWithValue("@DateLastActivity", DateTime.Now);
                            cmd.ExecuteNonQuery();
                        }

                        return u;
                    }
                }
            }
            catch (Exception ex) {
                throw new ProviderException("Error while performing database query.", ex);
            }

        }

        public override MembershipUser GetUser(string username, bool userIsOnline) {
            // Validate arguments
            if (username == null) throw new ArgumentNullException("username");
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "username");
            username = username.ToLower().Trim();

            // Query database
            try {
                using (HostingEnvironment.Impersonate())
                using (var db = this.connectionString.CreateDbConnection())
                using (var cmd = this.CreateDbCommand("SELECT * FROM $Users WHERE UserName = @UserName", db)) {
                    cmd.AddParameterWithValue("@UserName", username);
                    db.Open();
                    using (var r = cmd.ExecuteReader()) {
                        if (!r.Read()) return null; // user not found

                        // Get user
                        var u = this.GetUserFromReader(r);
                        r.Close();

                        // Update last activity date
                        if (userIsOnline) {
                            cmd.CommandText = "UPDATE " + this.TableName + " SET DateLastActivity = @DateLastActivity WHERE UserName = @UserName";
                            cmd.AddParameterWithValue("@DateLastActivity", DateTime.Now);
                            cmd.ExecuteNonQuery();
                        }

                        return u;
                    }
                }
            }
            catch (Exception ex) {
                throw new ProviderException("Error while performing database query.", ex);
            }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords) {
            if (emailToMatch == null) throw new ArgumentNullException("emailToMatch");
            if (string.IsNullOrWhiteSpace(emailToMatch)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "emailToMatch");

            return this.FindUsers("WHERE Email LIKE @Match", emailToMatch.Trim().ToLower(), pageIndex, pageSize, out totalRecords);
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords) {
            if (usernameToMatch == null) throw new ArgumentNullException("usernameToMatch");
            if (string.IsNullOrWhiteSpace(usernameToMatch)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "usernameToMatch");

            return this.FindUsers("WHERE UserName LIKE @Match", usernameToMatch.Trim().ToLower(), pageIndex, pageSize, out totalRecords);
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords) {
            return this.FindUsers(null, null, pageIndex, pageSize, out totalRecords);
        }

        public override void UpdateUser(MembershipUser user) {
            // Validate arguments
            if (user == null) throw new ArgumentNullException("user");

            // Validate e-mail
            var email = user.Email.Trim().ToLower();
            if (!Regex.IsMatch(email, EMAIL_VALIDATION_PATTERN)) throw new FormatException("Invalid format of e-mail address.");
            if (this.RequiresUniqueEmail) {
                var userName = this.GetUserNameByEmail(user.Email);
                if (!(string.IsNullOrWhiteSpace(userName) || userName.Equals(user.UserName))) throw new ProviderException("Duplicate e-mail address.");
            }

            // Update database
            var sql = "UPDATE $Users SET Email = @Email, Comment = @Comment, IsApproved = @IsApproved WHERE UserName = @UserName";
            if (this.UseEmailAddressAsUserName) sql = "UPDATE $Users SET UserName = @Email, Email = @Email, Comment = @Comment, IsApproved = @IsApproved WHERE UserName = @UserName";
            try {
                using (HostingEnvironment.Impersonate())
                using (var db = this.connectionString.CreateDbConnection())
                using (var cmd = this.CreateDbCommand(sql, db)) {
                    cmd.AddParameterWithValue("@UserName", user.UserName);
                    cmd.AddParameterWithValue("@Email", email);
                    cmd.AddParameterWithValue("@Comment", user.Comment);
                    cmd.AddParameterWithValue("@IsApproved", user.IsApproved);
                    db.Open();
                    cmd.ExecuteNonQuery();
                    db.Close();
                }
            }
            catch (Exception ex) {
                throw new ProviderException("Error while performing database query.", ex);
            }
        }

        // The following methods are inherited from MembershipProvider and are not implemented 

        public override bool UnlockUser(string userName) {
            throw new NotImplementedException("Account lockdown is intentionally not implemented due to possibility of DoS attack.");
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer) {
            throw new NotImplementedException("Questions are not implemented.");
        }

        public override string GetPassword(string username, string answer) {
            throw new NotImplementedException("Password retrieval is not implemented.");
        }

        // Helper methods

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt) {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512()) {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt) {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt)) {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++) {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

        private void SetPassword(string username, string newPassword) {
            // Validate new password
            var args = new ValidatePasswordEventArgs(username, newPassword, true);
            OnValidatingPassword(args);
            if (args.Cancel) throw args.FailureInformation ?? new MembershipPasswordException("Change password canceled due to new password validation failure.");

            // Hash password
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(newPassword, out passwordHash, out passwordSalt);

            // Update database
            try {
                using (HostingEnvironment.Impersonate())
                using (var db = this.connectionString.CreateDbConnection())
                using (var cmd = this.CreateDbCommand("UPDATE $Users SET PasswordHash = @PasswordHash, PasswordSalt = @PasswordSalt, DateLastPasswordChange = @DateLastPasswordChange WHERE UserName = @UserName ", db)) {
                    cmd.AddParameterWithValue("@UserName", username);
                    cmd.AddParameterWithValue("@PasswordHash", passwordHash);
                    cmd.AddParameterWithValue("@PasswordSalt", passwordSalt);
                    cmd.AddParameterWithValue("@DateLastPasswordChange", DateTime.Now);
                    db.Open();
                    cmd.ExecuteNonQuery();
                    db.Close();
                }
            }
            catch (Exception ex) {
                throw new ProviderException("Error while performing database query.", ex);
            }
        }

        private MembershipUser GetUserFromReader(DbDataReader r) {
            object providerKey;
            switch (this.UserKeyType) {
                case ProviderUserKeyType.IntIdentity:
                    providerKey = (int)r["UserId"];
                    break;
                case ProviderUserKeyType.Guid:
                    providerKey = (Guid)r["UserId"];
                    break;
                case ProviderUserKeyType.UserName:
                default:
                    providerKey = r["UserName"] as string;
                    break;
            }

            return new MembershipUser(this.Name,   // provider name
                r["UserName"] as string,
                providerKey,                        // provider key
                r["Email"] as string,
                null,                               // password question
                r["Comment"] == DBNull.Value ? null : r["Comment"] as string,
                (bool)r["IsApproved"],
                false,                              // locked out
                (DateTime)r["DateCreated"],
                r["DateLastLogin"] == DBNull.Value ? new DateTime() : (DateTime)r["DateLastLogin"],
                r["DateLastActivity"] == DBNull.Value ? new DateTime() : (DateTime)r["DateLastActivity"],
                (DateTime)r["DateLastPasswordChange"],
                new DateTime());                    // last lockout date
        }

        private MembershipUserCollection FindUsers(string whereCondition, string matchString, int pageIndex, int pageSize, out int totalRecords) {
            var uc = new MembershipUserCollection();

            // Query database
            try {
                using (HostingEnvironment.Impersonate())
                using (var db = this.connectionString.CreateDbConnection())
                using (var cmd = this.CreateDbCommand("SELECT COUNT(*) FROM " + this.TableName, db)) {
                    // Add where condition
                    if (!string.IsNullOrEmpty(whereCondition)) {
                        cmd.CommandText += " " + whereCondition;
                        cmd.AddParameterWithValue("@Match", matchString);
                    }
                    db.Open();

                    // Get total records
                    var skipRecords = pageIndex * pageSize;
                    totalRecords = (int)cmd.ExecuteScalar();
                    if (totalRecords <= skipRecords) return uc; // not enough users

                    // Get users
                    cmd.CommandText = "SELECT TOP " + (skipRecords + pageSize) + " * FROM " + this.TableName;
                    if (!string.IsNullOrEmpty(whereCondition)) cmd.CommandText += " " + whereCondition;
                    cmd.CommandText += " ORDER BY UserName";
                    using (var r = cmd.ExecuteReader()) {
                        var i = 0;
                        while (r.Read()) {
                            i++;
                            if (i <= skipRecords) continue;
                            uc.Add(this.GetUserFromReader(r));
                        }
                    }
                    db.Close();
                    return uc;
                }
            }
            catch (Exception ex) { throw new ProviderException("Error while performing database query.", ex); }
        }

        private DbCommand CreateDbCommand(string commandText, DbConnection db) {
            var cmd = db.CreateCommand();
            cmd.CommandText = commandText.Replace("$Users", this.TableName);
            return cmd;
        }



    }
}
