/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.Linq;
using Malsys.Web.Entities;

namespace Malsys.Web.Models {
	public interface IUsersRepository {

		IQueryable<User> Users { get; }
		IQueryable<Role> Roles { get; }


		void LogUserActivity(string userName);

		User CreateUser(NewUserModel user);

		Role CreateRole(NewRoleModel role);

		void AddUserToRole(int userId, int roleId);

		void RemoveUserFromRole(int userId, int roleId);

		int SaveChanges();

	}
}
