/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System;
using System.Data.Objects.DataClasses;
using System.Linq;

namespace Malsys.Web.Entities {
	public interface IUsersDb : IActionLogDb {

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

		/// <summary>
		///	Returns user entity which corresponds to given user name.
		///	Throws exception if given user name do not exists.
		/// </summary>
		/// <param name="db">User database for query.</param>
		/// <param name="name">Case insensitive name of desired user.</param>
		public static User GetUserByName(this IUsersDb db, string name) {
			string userNameLower = name.Trim().ToLowerInvariant();
			return db.Users.Single(u => u.NameLowercase == userNameLower);
		}

		/// <summary>
		///	Returns user entity which corresponds to given user name.
		/// Returns null if given user name do not exist of if null is supplied.
		/// </summary>
		/// <param name="db">User database for query.</param>
		/// <param name="name">Case insensitive name of desired user.</param>
		public static User TryGetUserByName(this IUsersDb db, string name) {

			if (name == null) {
				return null;
			}

			string userNameLower = name.Trim().ToLowerInvariant();

			if (userNameLower.Length == 0) {
				return null;
			}

			return db.Users.SingleOrDefault(u => u.NameLowercase == userNameLower);
		}

	}
}
