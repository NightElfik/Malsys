// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.Web.Mvc;
using System.Web.Security;

namespace Malsys.Web.Security {
	class LightweightRoleProvider : RoleProvider {

		public override void AddUsersToRoles(string[] usernames, string[] roleNames) {
			throw new NotImplementedException();
		}

		public override string ApplicationName {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public override void CreateRole(string roleName) {
			throw new NotImplementedException();
		}

		public override bool DeleteRole(string roleName, bool throwOnPopulatedRole) {
			throw new NotImplementedException();
		}

		public override string[] FindUsersInRole(string roleName, string usernameToMatch) {
			throw new NotImplementedException();
		}

		public override string[] GetAllRoles() {
			throw new NotImplementedException();
		}

		public override string[] GetRolesForUser(string username) {
			var userAuth = DependencyResolver.Current.GetService<IUserAuthenticator>();
			return userAuth.GetRolesForUser(username);
		}

		public override string[] GetUsersInRole(string roleName) {
			throw new NotImplementedException();
		}

		public override bool IsUserInRole(string username, string roleName) {
			throw new NotImplementedException();
		}

		public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames) {
			throw new NotImplementedException();
		}

		public override bool RoleExists(string roleName) {
			throw new NotImplementedException();
		}
	}
}