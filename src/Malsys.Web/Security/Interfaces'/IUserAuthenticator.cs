using Malsys.Web.Entities;

namespace Malsys.Web.Security {
	public interface IUserAuthenticator {

		OperationResult<User> ValidateUser(string userName, string password);

		string[] GetRolesForUser(string userName);

		OperationResult<User> ChangePassword(string userName, string oldPassword, string newPassword);

	}
}
