using System.Linq;
using Malsys.Web.Entities;

namespace Malsys.Web.Models {
	public interface IUsersRepository {

		IQueryable<User> Users { get; }
		IQueryable<Role> Roles { get; }


		void LogUserActivity(string userName);

		void CreateUser(NewUserModel user);

		void CreateRole(NewRoleModel role);

		void AddUserToRole(int userId, int roleId);

		void RemoveUserFromRole(int userId, int roleId);

		int SaveChanges();

	}
}
