using System.Linq;
using Malsys.Web.Entities;

namespace Malsys.Web.Models {
	public interface IUsersRepository {

		IUsersDb UsersDb { get; }


		void LogUserActivity(string userName);

		OperationResult<User> CreateUser(NewUserModel user);

		OperationResult<Role> CreateRole(NewRoleModel role);

		void AddUserToRole(int userId, int roleId);

		void RemoveUserFromRole(int userId, int roleId);

	}
}
