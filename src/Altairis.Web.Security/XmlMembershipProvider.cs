// Altairis Web Security Toolkit
// Copyright © Michal A. Valasek - Altairis, 2006-2011 | www.altairis.cz 
// Licensed under terms of Microsoft Permissive License (MS-PL)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Xml.Serialization;

namespace Altairis.Web.Security {
    public class XmlMembershipProvider : MembershipProvider {
        private const string STORE_NAMESPACE_URI = "http://schemas.altairis.cz/AltairisWebSecurity/XmlMembershipProvider/1.0";
        private const string DEFAULT_FILE_NAME = "~/App_Data/Users.config";
        private const string EMAIL_VALIDATION_PATTERN = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

        private XmlUserCollection userDb;

        // Initialization

        public override void Initialize(string name, NameValueCollection config) {
            // Perform basic initialization
            base.Initialize(name, config);

            // Read default configuration
            this.ApplicationName = config.GetConfigValue("applicationName", null);
            this.minRequiredNonAlphanumericCharacters = config.GetConfigValue("minRequiredNonAlphanumericCharacters", 1);
            this.minRequiredPasswordLength = config.GetConfigValue("minRequiredPasswordLength", 7);
            this.passwordStrengthRegularExpression = config.GetConfigValue("passwordStrengthRegularExpression", null);
            this.requiresUniqueEmail = config.GetConfigValue("requiresUniqueEmail", false);

            // Read data file name
            this.DataFileName = config.GetConfigValue("dataFileName", DEFAULT_FILE_NAME);
            if (this.DataFileName.StartsWith("~/")) {
                this.DataFileName = HttpContext.Current.Server.MapPath(this.DataFileName);
            }

            // Read users
            if (System.IO.File.Exists(this.DataFileName)) {
                userDb = XmlUserCollection.LoadFromFile(this.DataFileName);
            }
            else {
                userDb = new XmlUserCollection();
                // userDb.SaveToFile(this.DataFileName);
            }

            // Throw error on excess attributes
            if (config.Count != 0) throw new ConfigurationErrorsException("Unrecognized configuration attributes found: " + string.Join(", ", config.AllKeys));
        }

        public string DataFileName { get; private set; }

        // Membership provider implementation

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status) {
            // Validate arguments
            if (username == null) throw new ArgumentNullException("username");
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "username");
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (providerUserKey != null) {
                // Do not allow arbitrary user keys
                status = MembershipCreateStatus.InvalidProviderUserKey;
                return null;
            }

            // Validate and normalize username
            username = username.ToLower().Trim();

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

            // Validate e-mail address
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

            // Check if we don't already have such user
            if (this.GetUser(username, false) != null) {
                status = MembershipCreateStatus.DuplicateUserName;
                return null;
            }

            // Create and store new user
            var newUser = new XmlUser {
                DateCreated = DateTime.Now,
                DateLastPasswordChange = DateTime.Now,
                Email = email,
                IsApproved = isApproved,
                UserName = username
            };
            newUser.SetPassword(password);
            this.userDb.Add(newUser);
            this.userDb.SaveToFile(this.DataFileName);

            // Return newly created user
            status = MembershipCreateStatus.Success;
            return this.GetUser(username, false);
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline) {
            return this.GetUser(providerUserKey as string, userIsOnline);
        }

        public override MembershipUser GetUser(string username, bool userIsOnline) {
            // Validate arguments
            if (username == null) throw new ArgumentNullException("username");
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "username");
            username = username.ToLower().Trim();

            // Try to find user
            var xmlUser = this.userDb.FirstOrDefault(x => x.UserName.Equals(username));
            if (xmlUser == null) return null;

            // Update last activity date when requested
            if (userIsOnline) {
                xmlUser.DateLastActivity = DateTime.Now;
                this.userDb.SaveToFile(this.DataFileName);
            }

            // Return found user
            return this.XmlToMembershipUser(xmlUser);
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords) {
            return this.PageSearchResults(this.userDb, pageIndex, pageSize, out totalRecords);
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords) {
            var searchResults = this.userDb.Where(x => x.Email.Contains(emailToMatch));
            return this.PageSearchResults(searchResults, pageIndex, pageSize, out totalRecords);
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords) {
            var searchResults = this.userDb.Where(x => x.UserName.Contains(usernameToMatch));
            return this.PageSearchResults(searchResults, pageIndex, pageSize, out totalRecords);
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

            // Find user
            var xmlUser = this.userDb.FirstOrDefault(x => x.UserName.Equals(user.UserName));
            if (xmlUser == null) throw new ProviderException("User not found");

            // Change user
            xmlUser.Email = email;
            xmlUser.Comment = user.Comment;
            xmlUser.IsApproved = user.IsApproved;
            this.userDb.SaveToFile(this.DataFileName);
        }

        public override bool ValidateUser(string username, string password) {
            // Validate arguments
            if (string.IsNullOrWhiteSpace(username)) return false;
            if (string.IsNullOrWhiteSpace(password)) return false;

            // Find user
            username = username.ToLower().Trim();
            var xmlUser = this.userDb.FirstOrDefault(x => x.UserName.Equals(username));
            if (xmlUser == null) return false;
            if (!xmlUser.VerifyPassword(password)) return false;

            // Update last login and activity date
            xmlUser.DateLastActivity = DateTime.Now;
            xmlUser.DateLastLogin = DateTime.Now;
            this.userDb.SaveToFile(this.DataFileName);
            return true;
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData) {
            if (username == null) throw new ArgumentNullException("username");
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "username");

            username = username.ToLower().Trim();
            var xmlUser = this.userDb.FirstOrDefault(x => x.UserName.Equals(username));
            if (xmlUser == null) return false;

            this.userDb.Remove(xmlUser);
            this.userDb.SaveToFile(this.DataFileName);
            return true;
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword) {
            // Validate arguments
            if (username == null) throw new ArgumentNullException("username");
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "username");
            if (oldPassword == null) throw new ArgumentNullException("oldPassword");
            if (string.IsNullOrWhiteSpace(oldPassword)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "oldPassword");
            if (newPassword == null) throw new ArgumentNullException("newPassword");
            if (string.IsNullOrWhiteSpace(newPassword)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "newPassword");

            // Find user
            username = username.ToLower().Trim();
            var xmlUser = this.userDb.FirstOrDefault(x => x.UserName.Equals(username));
            if (xmlUser == null) return false;

            // Check old password
            if (!xmlUser.VerifyPassword(oldPassword)) return false;

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
            xmlUser.SetPassword(newPassword);
            this.userDb.SaveToFile(this.DataFileName);
            return true;
        }

        public override string ResetPassword(string username, string answer) {
            // Validate arguments
            if (username == null) throw new ArgumentNullException("username");
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "username");
            if (answer != null) throw new ArgumentException("Answers are not supported", "answer");

            // Find user
            username = username.ToLower().Trim();
            var xmlUser = this.userDb.FirstOrDefault(x => x.UserName.Equals(username));
            if (xmlUser == null) throw new ProviderException("User not found");

            // Create new password
            var newPasswordLength = Math.Max(this.MinRequiredPasswordLength, 8);
            var newPassword = Membership.GeneratePassword(newPasswordLength, this.minRequiredNonAlphanumericCharacters);

            // Set and return new password
            xmlUser.SetPassword(newPassword);
            this.userDb.SaveToFile(this.DataFileName);

            return newPassword;
        }

        public override string GetUserNameByEmail(string email) {
            if (email == null) throw new ArgumentNullException("email");
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "email");

            email = email.ToLower().Trim();
            var xmlUser = this.userDb.FirstOrDefault(x => x.Email.Equals(email));
            if (xmlUser == null) return null;
            return xmlUser.UserName;
        }

        public override int GetNumberOfUsersOnline() {
            var decisiveTime = DateTime.Now.AddMinutes(-Membership.UserIsOnlineTimeWindow);
            return this.userDb.Count(x => x.DateLastActivity >= decisiveTime);
        }

        #region Inherited properties

        private string passwordStrengthRegularExpression;
        private int minRequiredNonAlphanumericCharacters, minRequiredPasswordLength;
        private bool requiresUniqueEmail;

        public override string ApplicationName { get; set; }

        public override bool EnablePasswordReset {
            get { return true; }
        }

        public override bool EnablePasswordRetrieval {
            get { return false; }
        }

        public override int MinRequiredNonAlphanumericCharacters {
            get { return this.minRequiredNonAlphanumericCharacters; }
        }

        public override int MinRequiredPasswordLength {
            get { return this.minRequiredPasswordLength; }
        }

        public override MembershipPasswordFormat PasswordFormat {
            get { return MembershipPasswordFormat.Hashed; }
        }

        public override string PasswordStrengthRegularExpression {
            get { return this.passwordStrengthRegularExpression; }
        }

        public override bool RequiresQuestionAndAnswer {
            get { return false; }
        }

        public override bool RequiresUniqueEmail {
            get { return this.requiresUniqueEmail; }
        }

        #endregion

        #region Unsupported

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer) {
            throw new NotSupportedException();
        }

        public override string GetPassword(string username, string answer) {
            throw new NotSupportedException();
        }

        public override int MaxInvalidPasswordAttempts {
            get { throw new NotSupportedException(); }
        }

        public override int PasswordAttemptWindow {
            get { throw new NotSupportedException(); }
        }

        public override bool UnlockUser(string userName) {
            throw new NotSupportedException();
        }

        #endregion

        // Helper methods

        private MembershipUser XmlToMembershipUser(XmlUser user) {
            return new MembershipUser(
                this.Name,          // provider name
                user.UserName,
                user.UserName,      // provider key
                user.Email,
                null,               // password question
                user.Comment,
                user.IsApproved,
                false,              // is locked out
                user.DateCreated,
                user.DateLastLogin.HasValue ? user.DateLastLogin.Value : new DateTime(),
                user.DateLastActivity.HasValue ? user.DateLastActivity.Value : new DateTime(),
                user.DateLastPasswordChange,
                new DateTime());    // last lockout date
        }

        private MembershipUserCollection XmlToMembershipUserCollection(IEnumerable<XmlUser> users) {
            var r = new MembershipUserCollection();
            foreach (var xmlUser in users) {
                r.Add(this.XmlToMembershipUser(xmlUser));
            }
            return r;
        }

        private MembershipUserCollection PageSearchResults(IEnumerable<XmlUser> users, int pageIndex, int pageSize, out int totalRecords) {
            // Validate arguments
            if (pageIndex < 1) throw new ArgumentOutOfRangeException("pageIndex");
            if (pageSize < 1) throw new ArgumentOutOfRangeException("pageSize");

            // Get total number of users
            totalRecords = users.Count();

            // How many records to skip?
            var skip = (pageIndex - 1) * pageSize;
            if (skip >= totalRecords) return new MembershipUserCollection();

            // Return paged data
            return this.XmlToMembershipUserCollection(users.Skip(skip).Take(totalRecords));
        }

        #region Storage classes

        [XmlType(TypeName = "user", Namespace = XmlMembershipProvider.STORE_NAMESPACE_URI)]
        [XmlRoot(ElementName = "user", Namespace = XmlMembershipProvider.STORE_NAMESPACE_URI)]
        public class XmlUser {
            [XmlAttribute(AttributeName = "name")]
            public string UserName { get; set; }

            [XmlAttribute(AttributeName = "email")]
            public string Email { get; set; }

            [XmlAttribute(AttributeName = "approved")]
            public bool IsApproved { get; set; }

            [XmlAttribute(AttributeName = "dateCreated")]
            public DateTime DateCreated { get; set; }

            [XmlAttribute(AttributeName = "dateLastPasswordChange")]
            public DateTime DateLastPasswordChange { get; set; }

            [XmlElement(ElementName = "Comment", IsNullable = false)]
            public string Comment { get; set; }

            [XmlElement(ElementName = "dateLastLogin")]
            public DateTime? DateLastLogin { get; set; }

            [XmlElement(ElementName = "dateLastActivity")]
            public DateTime? DateLastActivity { get; set; }

            [XmlElement(ElementName = "passwordHash")]
            public byte[] PasswordHash { get; set; }

            [XmlElement(ElementName = "passwordSalt")]
            public byte[] PasswordSalt { get; set; }

            public void SetPassword(string newPassword) {
                if (newPassword == null) throw new ArgumentNullException("newPassword");
                if (string.IsNullOrWhiteSpace(newPassword)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "newPassword");

                var data = System.Text.Encoding.UTF8.GetBytes(newPassword);
                using (var hmac = new System.Security.Cryptography.HMACSHA512()) {
                    this.PasswordSalt = hmac.Key;
                    this.PasswordHash = hmac.ComputeHash(data);
                }
            }

            public bool VerifyPassword(string password) {
                if (string.IsNullOrEmpty(password)) return false;

                var data = System.Text.Encoding.UTF8.GetBytes(password);
                using (var hmac = new System.Security.Cryptography.HMACSHA512(this.PasswordSalt)) {
                    var hash = hmac.ComputeHash(data);
                    if (hash.Length != this.PasswordHash.Length) return false;
                    for (int i = 0; i < hash.Length; i++) {
                        if (hash[i] != this.PasswordHash[i]) return false;
                    }
                }

                return true;
            }

        }

        [XmlType(TypeName = "users", Namespace = XmlMembershipProvider.STORE_NAMESPACE_URI)]
        [XmlRoot(ElementName = "users", Namespace = XmlMembershipProvider.STORE_NAMESPACE_URI)]
        public class XmlUserCollection : KeyedCollection<string, XmlUser> {

            protected override string GetKeyForItem(XmlUser item) {
                return item.UserName;
            }

            private static object saveLock = new object();

            public void SaveToFile(string fileName) {
                lock (saveLock) {
                    using (var f = System.IO.File.Create(fileName)) {
                        var xs = new XmlSerializer(this.GetType());
                        xs.Serialize(f, this);
                    }
                }
            }

            public static XmlUserCollection LoadFromFile(string fileName) {
                using (var f = System.IO.File.OpenRead(fileName)) {
                    var xs = new XmlSerializer(typeof(XmlUserCollection));
                    return xs.Deserialize(f) as XmlUserCollection;
                }
            }

        }

        #endregion

    }
}
