using System;
using System.Linq;
using Malsys.Web.Entities;
using Malsys.Web.Security;

namespace Malsys.Web.Models.Repositories {
	class UsersRepository : IUsersRepository {

		private readonly IUsersDb usersDb;
		private readonly IDateTimeProvider dateTimeProvider;
		private readonly IPasswordHasher pwdHasher;


		public UsersRepository(IUsersDb usersDb, IPasswordHasher pwdHasher, IDateTimeProvider dtProvider) {

			this.usersDb = usersDb;
			this.pwdHasher = pwdHasher;
			dateTimeProvider = dtProvider;
		}


		public IQueryable<User> Users {
			get { return usersDb.Users; }
		}

		public IQueryable<Role> Roles {
			get { return usersDb.Roles; }
		}


		public void LogUserActivity(string userName) {

			string userNameLower = userName.Trim().ToLower();

			var user = usersDb.Users.Where(u => u.NameLowercase == userNameLower).SingleOrDefault();
			if (user != null) {
				user.LastActivityDate = dateTimeProvider.Now;
				usersDb.SaveChanges();
			}

		}

		public User CreateUser(NewUserModel newUser) {

			// TODO: validate model somehow

			string userName = newUser.UserName.Trim();
			string userNameLower = userName.ToLower();

			var user = usersDb.Users.SingleOrDefault(u => u.NameLowercase == userNameLower);
			if (user != null) {
				throw new ApplicationException("User name `{0}` already exists.".Fmt(userName));
			}

			DateTime now = dateTimeProvider.Now;
			byte[] pwdHash, salt;

			pwdHasher.CreatePasswordHash(newUser.Password, out pwdHash, out salt);

			var newDbUser = new User() {
				Name = userName,
				NameLowercase = userNameLower,
				PasswordHash = pwdHash,
				PasswordSalt = salt,
				Email = newUser.Email.Trim().ToLower(),
				RegistrationDate = now,
				LastLoginDate = now,
				LastActivityDate = now,
				LastPwdChangeDate = now
			};

			usersDb.AddUser(newDbUser);
			usersDb.SaveChanges();

			return newDbUser;
		}

		public Role CreateRole(NewRoleModel newRole) {

			// TODO: validate model somehow

			string roleName = newRole.RoleName.Trim();
			string roleNameLower = roleName.ToLower();

			var role = usersDb.Roles.SingleOrDefault(r => r.NameLowercase == roleNameLower);
			if (role != null) {
				throw new ApplicationException("Role `{0}` already exists.".Fmt(roleName));
			}

			var newDbRole = new Role() {
				Name = roleName,
				NameLowercase = roleNameLower
			};

			usersDb.AddRole(newDbRole);
			usersDb.SaveChanges();

			return newDbRole;
		}

		public void AddUserToRole(int userId, int roleId) {

			var user = usersDb.Users.SingleOrDefault(u => u.UserId == userId);
			if (user == null) {
				throw new ApplicationException("Unknown user.");
			}

			var role = usersDb.Roles.SingleOrDefault(r => r.RoleId == roleId);
			if (role == null) {
				throw new ApplicationException("Unknown role.");
			}

			if (!user.Roles.Contains(role)) {
				user.Roles.Add(role);
				usersDb.SaveChanges();
			}

		}

		public void RemoveUserFromRole(int userId, int roleId) {

			var user = usersDb.Users.SingleOrDefault(u => u.UserId == userId);
			if (user == null) {
				throw new ApplicationException("Unknown user.");
			}

			var role = usersDb.Roles.SingleOrDefault(r => r.RoleId == roleId);
			if (role == null) {
				throw new ApplicationException("Unknown role.");
			}

			if (user.Roles.Contains(role)) {
				user.Roles.Remove(role);
				usersDb.SaveChanges();
			}
		}


		public int SaveChanges() {
			return usersDb.SaveChanges();
		}

	}
}