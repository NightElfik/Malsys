using System;
using System.Data.Objects.DataClasses;
using System.Linq;

namespace Malsys.Web.Entities {
	public interface IUsersDb : IDisposable {

		IQueryable<User> Users { get; }

		void DeleteUser(User user);
		void AddUser(User user);


		IQueryable<Role> Roles { get; }

		void DeleteRole(Role role);
		void AddRole(Role role);


		int SaveChanges();

		void Detach(object entity);
		void Attach(IEntityWithKey entity);

	}

	public static class IUsersDbExtensions {

		/// <param name="name">Case insensitive name of desired user.</param>
		public static User GetUserByName(this IUsersDb db, string name) {
			return db.Users.Single(u => u.NameLowercase == name.ToLower());
		}

		/// <summary>
		/// Returns null if user with given name do not exist.
		/// </summary>
		/// <param name="name">Case insensitive name of desired user.</param>
		public static User TryGetUserByName(this IUsersDb db, string name) {

			if (name == null) {
				return null;
			}

			return db.Users.SingleOrDefault(u => u.NameLowercase == name.ToLower());
		}

	}
}
