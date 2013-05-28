// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.Diagnostics.Contracts;
using System.Linq;
using Malsys.Web.Entities;

namespace Malsys.Web.Security {
	internal class UserAuthenticator : IUserAuthenticator {

		private readonly IUsersDb usersDb;
		private readonly IPasswordHasher pwdHasher;
		private readonly IDateTimeProvider dateTimeProvider;


		public UserAuthenticator(IUsersDb usersDb, IPasswordHasher pwdHasher, IDateTimeProvider dtProvider) {

			Contract.Requires<ArgumentNullException>(usersDb != null);
			Contract.Requires<ArgumentNullException>(pwdHasher != null);

			this.usersDb = usersDb;
			this.pwdHasher = pwdHasher;
			dateTimeProvider = dtProvider;
		}


		public OperationResult<User> ValidateUser(string userName, string password) {

			userName = userName.Trim().ToLower();

			var user = usersDb.Users.SingleOrDefault(u => u.NameLowercase == userName);
			if (user == null) {
				return new OperationResult<User>("Unknown user name or invalid password.");  // unknown user
			}

			if (!pwdHasher.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) {
				return new OperationResult<User>("Unknown user name or invalid password.");  // wrong password
			}

			// password ok, update last login date
			user.LastLoginDate = dateTimeProvider.Now;
			usersDb.SaveChanges();

			return new OperationResult<User>(user);
		}

		public string[] GetRolesForUser(string userName) {

			userName = userName.Trim().ToLower();

			var user = usersDb.Users.SingleOrDefault(u => u.NameLowercase == userName);
			if (user == null) {
				return new string[0];  // unknown user
			}

			return user.Roles.Select(r => r.Name).ToArray();
		}

		public OperationResult<User> ChangePassword(string userName, string oldPassword, string newPassword) {

			var userResult = ValidateUser(userName, oldPassword);
			if (!userResult) {
				return userResult;
			}

			var user = userResult.Data;

			DateTime now = dateTimeProvider.Now;
			byte[] pwdHash, salt;

			pwdHasher.CreatePasswordHash(newPassword, out pwdHash, out salt);

			user.PasswordHash = pwdHash;
			user.PasswordSalt = salt;
			usersDb.SaveChanges();

			return new OperationResult<User>(user);
		}

	}
}