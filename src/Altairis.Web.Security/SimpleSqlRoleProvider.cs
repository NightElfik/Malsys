// Altairis Web Security Toolkit
// Copyright © Michal A. Valasek - Altairis, 2006-2010 | www.altairis.cz 
// Licensed under terms of Microsoft Permissive License (MS-PL)

using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Data;
using System.Data.SqlClient;
using System.Web.Hosting;
using System.Web.Security;

namespace Altairis.Web.Security {

    [Obsolete("This class is no longer supported and developed. Use TableRoleProvider class instead.")]
    public class SimpleSqlRoleProvider : RoleProvider {

        #region Initialization and configuration

        private string applicationName, connectionString;

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="name">The friendly name of the provider.</param>
        /// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
        /// <exception cref="T:System.ArgumentNullException">The name of the provider is null.</exception>
        /// <exception cref="T:System.ArgumentException">The name of the provider has a length of zero.</exception>
        /// <exception cref="T:System.InvalidOperationException">An attempt is made to call <see cref="M:System.Configuration.Provider.ProviderBase.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> on a provider after the provider has already been initialized.</exception>
        public override void Initialize(string name, NameValueCollection config) {
            // Validate arguments
            if (config == null) throw new ArgumentNullException("config");
            if (string.IsNullOrEmpty(name)) name = "SimpleSqlRoleProvider";
            if (String.IsNullOrEmpty(config["description"])) {
                config.Remove("description");
                config.Add("description", "Simple SQL role provider");
            }

            // Initialize base class
            base.Initialize(name, config);

            // Initialize current class
            System.Configuration.ConnectionStringSettings ConnectionStringSettings = System.Configuration.ConfigurationManager.ConnectionStrings[config["connectionStringName"]];
            if (ConnectionStringSettings == null || ConnectionStringSettings.ConnectionString.Trim() == "") throw new ProviderException("Connection string cannot be blank.");
            this.connectionString = ConnectionStringSettings.ConnectionString;
            this.applicationName = config["applicationName"];
        }

        /// <summary>
        /// Gets or sets the name of the application to store and retrieve role information for. This provider ignores this setting.
        /// </summary>
        /// <value></value>
        /// <returns>The name of the application to store and retrieve role information for.</returns>
        public override string ApplicationName {
            get { return applicationName; }
            set { applicationName = value; }
        }

        #endregion

        /// <summary>
        /// Creates the role.
        /// </summary>
        /// <param name="rolename">The name of the role.</param>
        public override void CreateRole(string rolename) {
            // Validate arguments
            if (string.IsNullOrEmpty(rolename)) throw new ArgumentNullException("rolename");
            if (rolename.IndexOf(',') > 0) throw new ArgumentException("Role names cannot contain commas");
            if (rolename.Length > 100) throw new ArgumentException("Maximum role name length is 100 characters");
            if (this.RoleExists(rolename)) throw new ProviderException("Role name already exists");
            rolename = rolename.ToLower();

            // Create role
            try {
                using (HostingEnvironment.Impersonate())
                using (SqlConnection db = this.OpenDatabase())
                using (SqlCommand cmd = new SqlCommand("INSERT INTO Roles (RoleName) VALUES (@RoleName)", db)) {
                    cmd.Parameters.Add("@Rolename", SqlDbType.VarChar, 100).Value = rolename;
                    cmd.ExecuteNonQuery();
                }
            }
            catch { throw; } // Security context hack for HostingEnvironment.Impersonate

        }

        /// <summary>
        /// Deletes the role.
        /// </summary>
        /// <param name="rolename">The rolename.</param>
        /// <param name="throwOnPopulatedRole">if set to <c>true</c> [throw on populated role].</param>
        /// <returns></returns>
        public override bool DeleteRole(string rolename, bool throwOnPopulatedRole) {
            // Validate arguments
            if (string.IsNullOrEmpty(rolename)) throw new ArgumentNullException("rolename");
            if (!this.RoleExists(rolename)) throw new ProviderException("Role does not exist");
            if (throwOnPopulatedRole && this.GetUsersInRole(rolename).Length > 0) throw new ProviderException("Cannot delete a populated role");
            rolename = rolename.ToLower();

            // Delete role
            try {
                using (HostingEnvironment.Impersonate())
                using (SqlConnection db = this.OpenDatabase())
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Roles WHERE RoleName = @RoleName", db)) {
                    cmd.Parameters.Add("@RoleName", SqlDbType.VarChar, 100).Value = rolename;
                    return cmd.ExecuteNonQuery() != 0;
                }
            }
            catch { throw; } // Security context hack for HostingEnvironment.Impersonate
        }

        /// <summary>
        /// Adds the users to roles.
        /// </summary>
        /// <param name="usernames">The usernames.</param>
        /// <param name="rolenames">The rolenames.</param>
        public override void AddUsersToRoles(string[] usernames, string[] rolenames) {
            // Validate arguments
            foreach (string rolename in rolenames) if (!this.RoleExists(rolename)) throw new ProviderException("Role name not found");
            foreach (string username in usernames) {
                if (username.IndexOf(',') > 0) throw new ArgumentException("User names cannot contain commas.");
                foreach (string rolename in rolenames) {
                    if (IsUserInRole(username, rolename)) throw new ProviderException("User is already in role.");
                }
            }

            // Put changes into db
            try {
                using (HostingEnvironment.Impersonate())
                using (SqlConnection db = this.OpenDatabase())
                using (SqlCommand cmd = new SqlCommand("INSERT INTO UsersInRoles (UserName, RoleName) VALUES (@UserName, @RoleName)", db)) {
                    cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 100);
                    cmd.Parameters.Add("@RoleName", SqlDbType.VarChar, 100);
                    using (SqlTransaction tran = db.BeginTransaction()) {
                        try {
                            cmd.Transaction = tran;
                            foreach (string username in usernames) {
                                foreach (string rolename in rolenames) {
                                    cmd.Parameters["@UserName"].Value = username;
                                    cmd.Parameters["@RoleName"].Value = rolename;
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            tran.Commit();
                        }
                        catch {
                            tran.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch { throw; } // Security context hack for HostingEnvironment.Impersonate
        }

        /// <summary>
        /// Gets a list of all the roles for the configured application.
        /// </summary>
        /// <returns>
        /// A string array containing the names of all the roles stored in the data source for the configured application.
        /// </returns>
        public override string[] GetAllRoles() {
            // Get data from database
            try {
                using (HostingEnvironment.Impersonate())
                using (DataTable roleTable = new DataTable())
                using (SqlConnection db = this.OpenDatabase())
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT RoleName FROM Roles", db)) {
                    da.Fill(roleTable);
                    return TableToArray(roleTable);
                }
            }
            catch { throw; } // Security context hack for HostingEnvironment.Impersonate
        }

        /// <summary>
        /// Gets a list of the roles that a specified user is in for the configured applicationName.
        /// </summary>
        /// <param name="username">The user to return a list of roles for.</param>
        /// <returns>
        /// A string array containing the names of all the roles that the specified user is in for the configured applicationName.
        /// </returns>
        public override string[] GetRolesForUser(string username) {
            // Validate arguments
            if (string.IsNullOrEmpty(username)) throw new ArgumentNullException("username");
            if (username.IndexOf(',') > -1) throw new ArgumentException("User name cannot contain comma", "username");
            if (username.Length > 100) throw new ArgumentException("User name cannot be longer than 100 characters", "username");
            username = username.ToLower();

            // Get data from database
            try {
                using (HostingEnvironment.Impersonate())
                using (DataTable roleTable = new DataTable())
                using (SqlConnection db = this.OpenDatabase())
                using (SqlCommand cmd = new SqlCommand("SELECT RoleName FROM UsersInRoles WHERE UserName=@UserName", db)) {
                    cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 100).Value = username;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd)) da.Fill(roleTable);
                    return TableToArray(roleTable);
                }
            }
            catch { throw; } // Security context hack for HostingEnvironment.Impersonate
        }

        /// <summary>
        /// Gets the users in role.
        /// </summary>
        /// <param name="rolename">The rolename.</param>
        /// <returns></returns>
        public override string[] GetUsersInRole(string rolename) {
            // Validate arguments
            if (string.IsNullOrEmpty(rolename)) throw new ArgumentNullException("rolename");
            if (rolename.IndexOf(',') > -1) throw new ArgumentException("Role name cannot contain comma", "rolename");
            if (rolename.Length > 100) throw new ArgumentException("Role name cannot be longer than 100 characters", "rolename");
            rolename = rolename.ToLower();

            // Get data from database
            try {
                using (HostingEnvironment.Impersonate())
                using (DataTable roleTable = new DataTable())
                using (SqlConnection db = this.OpenDatabase())
                using (SqlCommand cmd = new SqlCommand("SELECT UserName FROM UsersInRoles WHERE RoleName=@RoleName", db)) {
                    cmd.Parameters.Add("@RoleName", SqlDbType.VarChar, 100).Value = rolename;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd)) da.Fill(roleTable);
                    return TableToArray(roleTable);
                }
            }
            catch { throw; } // Security context hack for HostingEnvironment.Impersonate
        }

        /// <summary>
        /// Determines whether specified user is in the specified role.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="rolename">The rolename.</param>
        /// <returns>
        /// 	<c>true</c> if is user in role; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsUserInRole(string username, string rolename) {
            // Validate arguments
            if (string.IsNullOrEmpty(rolename)) throw new ArgumentNullException("rolename");
            if (rolename.IndexOf(',') > -1) throw new ArgumentException("Role name cannot contain comma", "rolename");
            if (rolename.Length > 100) throw new ArgumentException("Role name cannot be longer than 100 characters", "rolename");
            rolename = rolename.ToLower();
            if (string.IsNullOrEmpty(username)) throw new ArgumentNullException("username");
            if (username.IndexOf(',') > -1) throw new ArgumentException("User name cannot contain comma", "username");
            if (username.Length > 100) throw new ArgumentException("User name cannot be longer than 100 characters", "username");
            username = username.ToLower();

            // Get data from database
            try {
                using (HostingEnvironment.Impersonate())
                using (SqlConnection db = this.OpenDatabase())
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM UsersInRoles WHERE RoleName=@RoleName AND UserName=@UserName", db)) {
                    cmd.Parameters.Add("@RoleName", SqlDbType.VarChar, 100).Value = rolename;
                    cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 100).Value = username;
                    return (int)cmd.ExecuteScalar() != 0;
                }
            }
            catch { throw; } // Security context hack for HostingEnvironment.Impersonate
        }

        /// <summary>
        /// Removes the users from roles.
        /// </summary>
        /// <param name="usernames">The usernames.</param>
        /// <param name="rolenames">The rolenames.</param>
        public override void RemoveUsersFromRoles(string[] usernames, string[] rolenames) {
            // Validate arguments
            foreach (string rolename in rolenames) if (!RoleExists(rolename)) throw new ProviderException("Role name not found.");
            foreach (string username in usernames) {
                foreach (string rolename in rolenames) {
                    if (!IsUserInRole(username, rolename)) throw new ProviderException("User is not in role.");
                }
            }

            try {
                using (HostingEnvironment.Impersonate())
                using (SqlConnection db = this.OpenDatabase())
                using (SqlCommand cmd = new SqlCommand("DELETE FROM UsersInRoles WHERE UserName=@UserName AND RoleName=@RoleName", db)) {
                    cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 100);
                    cmd.Parameters.Add("@RoleName", SqlDbType.VarChar, 100);
                    using (SqlTransaction tran = db.BeginTransaction()) {
                        try {
                            cmd.Transaction = tran;
                            foreach (string username in usernames) {
                                foreach (string rolename in rolenames) {
                                    cmd.Parameters["@UserName"].Value = username;
                                    cmd.Parameters["@RoleName"].Value = rolename;
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            tran.Commit();
                        }
                        catch {
                            tran.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch { throw; } // Security context hack for HostingEnvironment.Impersonate
        }

        /// <summary>
        /// Checks if the specified role exists.
        /// </summary>
        /// <param name="rolename">The name of the role.</param>
        /// <returns><c>true</c> if the role exists; otherwise <c>false</c>.</returns>
        public override bool RoleExists(string rolename) {
            // Validate arguments
            if (string.IsNullOrEmpty(rolename)) return false;
            rolename = rolename.ToLower();

            // Check if role exists
            try {
                using (HostingEnvironment.Impersonate())
                using (SqlConnection db = this.OpenDatabase())
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Roles WHERE RoleName=@RoleName", db)) {
                    cmd.Parameters.Add("@RoleName", SqlDbType.VarChar, 100).Value = rolename;
                    return (int)cmd.ExecuteScalar() != 0;
                }
            }
            catch { throw; } // Security context hack for HostingEnvironment.Impersonate
        }

        /// <summary>
        /// Finds all users in given role.
        /// </summary>
        /// <param name="rolename">The rolename.</param>
        /// <param name="usernameToMatch">The username to match.</param>
        /// <returns>Array containing all matching user names.</returns>
        public override string[] FindUsersInRole(string rolename, string usernameToMatch) {
            // Validate arguments
            if (string.IsNullOrEmpty(rolename)) throw new ArgumentNullException("rolename");
            if (rolename.IndexOf(',') > -1) throw new ArgumentException("Role name cannot contain comma", "rolename");
            if (rolename.Length > 100) throw new ArgumentException("Role name cannot be longer than 100 characters", "rolename");
            rolename = rolename.ToLower();
            if (string.IsNullOrEmpty(usernameToMatch)) throw new ArgumentNullException("usernameToMatch");
            if (usernameToMatch.Length > 100) throw new ArgumentException("User name cannot be longer than 100 characters", "usernameToMatch");
            usernameToMatch = usernameToMatch.ToLower();

            // Get data from database
            try {
                using (HostingEnvironment.Impersonate())
                using (DataTable roleTable = new DataTable())
                using (SqlConnection db = this.OpenDatabase())
                using (SqlCommand cmd = new SqlCommand("SELECT UserName FROM UsersInRoles WHERE RoleName=@RoleName AND UserName LIKE @UserName", db)) {
                    cmd.Parameters.Add("@RoleName", SqlDbType.VarChar, 100).Value = rolename;
                    cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 100).Value = usernameToMatch;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd)) da.Fill(roleTable);
                    return TableToArray(roleTable);
                }
            }
            catch { throw; } // Security context hack for HostingEnvironment.Impersonate
        }

        // Private support functions

        /// <summary>
        /// Opens the database connection.
        /// </summary>
        /// <returns></returns>
        private SqlConnection OpenDatabase() {
            SqlConnection db = new SqlConnection(this.connectionString);
            db.Open();
            return db;
        }

        /// <summary>
        /// Converts single-column DataTable to array.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        private string[] TableToArray(DataTable table) {
            // Validate arguments
            if (table == null) throw new ArgumentNullException("table");
            if (table.Rows.Count == 0) return new string[0];

            // Convert table to array
            string[] data = new string[table.Rows.Count];
            for (int i = 0; i < table.Rows.Count; i++) data[i] = System.Convert.ToString(table.Rows[i][0]);
            return data;
        }

    }

}