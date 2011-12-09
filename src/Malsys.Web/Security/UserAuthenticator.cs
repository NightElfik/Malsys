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


		public bool ValidateUser(string userName, string password) {

			userName = userName.Trim().ToLower();

			var user = usersDb.Users.SingleOrDefault(u => u.NameLowercase == userName);
			if (user == null) {
				return false;  // unknown user
			}

			if (!pwdHasher.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) {
				return false;  // wrong password
			}

			// password ok, update last login date

			user.LastLoginDate = dateTimeProvider.Now;
			usersDb.SaveChanges();

			return true;
		}

		public string[] GetRolesForUser(string userName) {

			userName = userName.Trim().ToLower();

			var user = usersDb.Users.SingleOrDefault(u => u.NameLowercase == userName);
			if (user == null) {
				return new string[0];  // unknown user
			}

			return user.Roles.Select(r => r.Name).ToArray();
		}

		public void ChangePassword(string userName, string oldPassword, string newPassword) {

			if (!ValidateUser(userName, oldPassword)) {
				throw new ApplicationException("Wrong current password.");
			}

			DateTime now = dateTimeProvider.Now;
			byte[] pwdHash, salt;

			pwdHasher.CreatePasswordHash(newPassword, out pwdHash, out salt);

			// user should exist if it is valid
			var user = usersDb.Users.FirstOrDefault(u => u.NameLowercase == userName);

			user.PasswordHash = pwdHash;
			user.PasswordSalt = salt;
			usersDb.SaveChanges();
		}

	}
}