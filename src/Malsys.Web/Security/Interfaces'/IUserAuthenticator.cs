
namespace Malsys.Web.Security {
	public interface IUserAuthenticator {

		bool ValidateUser(string userName, string password);

		string[] GetRolesForUser(string userName);

		void ChangePassword(string userName, string oldPassword, string newPassword);

	}
}
