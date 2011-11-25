// Altairis Web Security Toolkit
// Copyright © Michal A. Valasek - Altairis, 2006-2011 | www.altairis.cz 
// Licensed under terms of Microsoft Permissive License (MS-PL)

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web.Hosting;
using System.Web.Security;

namespace Altairis.Web.Security {
    public class TableRoleProvider : RoleProvider {
        private const string DEFAULT_ROLES_TABLE_NAME = "Roles";
        private const string DEFAULT_ROLE_MEMBERSHIPS_TABLE_NAME = "RoleMemberships";
        private ConnectionStringSettings connectionString;

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config) {
            // Perform basic initialization
            base.Initialize(name, config);

            // Read default configuration
            this.ApplicationName = config.GetConfigValue("applicationName", null);

            // Read connection string
            this.ConnectionStringName = config.GetConfigValue("connectionStringName", null);
            if (string.IsNullOrWhiteSpace(this.ConnectionStringName)) throw new ConfigurationErrorsException("Required \"connectionStringName\" attribute not specified.");
            this.connectionString = ConfigurationManager.ConnectionStrings[this.ConnectionStringName];
            if (this.connectionString == null) throw new ConfigurationErrorsException(string.Format("Connection string \"{0}\" was not found.", this.ConnectionStringName));
            if (string.IsNullOrEmpty(this.connectionString.ProviderName)) throw new ConfigurationErrorsException(string.Format("Connection string \"{0}\" does not have specified the \"providerName\" attribute.", this.ConnectionStringName));

            // Read table names
            this.RolesTableName = config.GetConfigValue("rolesTableName", DEFAULT_ROLES_TABLE_NAME);
            this.RoleMembershipsTableName = config.GetConfigValue("roleMembershipsTableName", DEFAULT_ROLE_MEMBERSHIPS_TABLE_NAME);
            if (this.RolesTableName.IndexOf('[') == -1) this.RolesTableName = string.Join(".", this.RolesTableName.Split('.').Select(x => "[" + x + "]").ToArray());
            if (this.RoleMembershipsTableName.IndexOf('[') == -1) this.RoleMembershipsTableName = string.Join(".", this.RoleMembershipsTableName.Split('.').Select(x => "[" + x + "]").ToArray());

            // Throw error on excess attributes
            if (config.Count != 0) throw new ConfigurationErrorsException("Unrecognized configuration attributes found: " + string.Join(", ", config.AllKeys));
        }

        #region Configuration properties

        public override string ApplicationName { get; set; }

        public string ConnectionStringName { get; private set; }

        public string RolesTableName { get; private set; }

        public string RoleMembershipsTableName { get; private set; }

        #endregion

        // Role provider methods

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
                using (var db = this.connectionString.CreateDbConnection())
                using (var cmd = this.CreateDbCommand("INSERT INTO $Roles (RoleName) VALUES (@RoleName)", db)) {
                    cmd.AddParameterWithValue("@RoleName", rolename);
                    db.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex) { throw new ProviderException("Error while performing database query.", ex); }

        }

        public override bool DeleteRole(string rolename, bool throwOnPopulatedRole) {
            // Validate arguments
            if (string.IsNullOrEmpty(rolename)) throw new ArgumentNullException("rolename");
            if (!this.RoleExists(rolename)) throw new ProviderException("Role does not exist");
            if (throwOnPopulatedRole && this.GetUsersInRole(rolename).Length > 0) throw new ProviderException("Cannot delete a populated role");
            rolename = rolename.ToLower();

            // Delete role
            try {
                using (HostingEnvironment.Impersonate())
                using (var db = this.connectionString.CreateDbConnection())
                using (var cmd = this.CreateDbCommand("DELETE FROM $Roles WHERE RoleName = @RoleName", db)) {
                    cmd.AddParameterWithValue("@RoleName", rolename);
                    db.Open();
                    return cmd.ExecuteNonQuery() != 0;
                }
            }
            catch (Exception ex) { throw new ProviderException("Error while performing database query.", ex); }
        }

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
                using (var db = this.connectionString.CreateDbConnection())
                using (var cmd = this.CreateDbCommand("INSERT INTO $RoleMemberships (UserName, RoleName) VALUES (@UserName, @RoleName)", db)) {
                    cmd.AddParameterWithValue("@UserName", string.Empty);
                    cmd.AddParameterWithValue("@RoleName", string.Empty);
                    db.Open();
                    using (var tran = db.BeginTransaction()) {
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
            catch (Exception ex) {
                throw new ProviderException("Error while performing database query.", ex);
            }
        }

        public override string[] GetAllRoles() {
            // Get data from database
            try {
                using (HostingEnvironment.Impersonate())
                using (var db = this.connectionString.CreateDbConnection())
                using (var cmd = this.CreateDbCommand("SELECT RoleName FROM $Roles", db)) {
                    db.Open();
                    using (var r = cmd.ExecuteReader()) {
                        return EnumerateReader(r).ToArray();
                    }
                }
            }
            catch (Exception ex) {
                throw new ProviderException("Error while performing database query.", ex);
            }
        }

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
                using (var db = this.connectionString.CreateDbConnection())
                using (var cmd = this.CreateDbCommand("SELECT RoleName FROM $RoleMemberships WHERE UserName = @UserName", db)) {
                    cmd.AddParameterWithValue("@UserName", username);
                    db.Open();
                    using (var r = cmd.ExecuteReader()) {
                        return EnumerateReader(r).ToArray();
                    }
                }
            }
            catch (Exception ex) {
                throw new ProviderException("Error while performing database query.", ex);
            }
        }

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
                using (var db = this.connectionString.CreateDbConnection())
                using (var cmd = this.CreateDbCommand("SELECT UserName FROM $RoleMemberships WHERE RoleName = @RoleName", db)) {
                    cmd.AddParameterWithValue("@RoleName", rolename);
                    db.Open();
                    using (var r = cmd.ExecuteReader()) {
                        return EnumerateReader(r).ToArray();
                    }
                }
            }
            catch (Exception ex) {
                throw new ProviderException("Error while performing database query.", ex);
            }
        }

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
                using (var db = this.connectionString.CreateDbConnection())
                using (var cmd = this.CreateDbCommand("SELECT COUNT(*) FROM $RoleMemberships WHERE RoleName = @RoleName AND UserName = @UserName", db)) {
                    cmd.AddParameterWithValue("@RoleName", rolename);
                    cmd.AddParameterWithValue("@UserName", username);
                    db.Open();
                    return (int)cmd.ExecuteScalar() != 0;
                }
            }
            catch (Exception ex) {
                throw new ProviderException("Error while performing database query.", ex);
            }
        }

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
                using (var db = this.connectionString.CreateDbConnection())
                using (var cmd = this.CreateDbCommand("DELETE FROM $RoleMemberships WHERE UserName = @UserName AND RoleName = @RoleName", db)) {
                    cmd.AddParameterWithValue("@UserName", string.Empty);
                    cmd.AddParameterWithValue("@RoleName", string.Empty);
                    db.Open();
                    using (var tran = db.BeginTransaction()) {
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
            catch (Exception ex) {
                throw new ProviderException("Error while performing database query.", ex);
            }
        }

        public override bool RoleExists(string rolename) {
            // Validate arguments
            if (string.IsNullOrEmpty(rolename)) return false;
            rolename = rolename.ToLower();

            // Check if role exists
            try {
                using (HostingEnvironment.Impersonate())
                using (var db = this.connectionString.CreateDbConnection())
                using (var cmd = this.CreateDbCommand("SELECT COUNT(*) FROM $Roles WHERE RoleName = @RoleName", db)) {
                    cmd.AddParameterWithValue("@RoleName", rolename);
                    db.Open();
                    return (int)cmd.ExecuteScalar() != 0;
                }
            }
            catch (Exception ex) {
                throw new ProviderException("Error while performing database query.", ex);
            }
        }

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
                using (var db = this.connectionString.CreateDbConnection())
                using (var cmd = this.CreateDbCommand("SELECT UserName FROM $RoleMemberships WHERE RoleName = @RoleName AND UserName LIKE @UserName", db)) {
                    cmd.AddParameterWithValue("@RoleName", rolename);
                    cmd.AddParameterWithValue("@UserName", usernameToMatch);
                    db.Open();
                    using (var r = cmd.ExecuteReader()) {
                        return EnumerateReader(r).ToArray();
                    }
                }
            }
            catch (Exception ex) {
                throw new ProviderException("Error while performing database query.", ex);
            }
        }

        // Helper methods

        private IEnumerable<string> EnumerateReader(DbDataReader reader) {
            // Validate arguments
            if (reader == null) throw new ArgumentNullException("reader");

            while (reader.Read()) {
                yield return reader.GetString(0);
            }
        }

        // Database helper methods

        private DbCommand CreateDbCommand(string commandText, DbConnection db) {
            commandText = commandText.Replace("$Roles", this.RolesTableName);
            commandText = commandText.Replace("$RoleMemberships", this.RoleMembershipsTableName);

            var cmd = db.CreateCommand();
            cmd.CommandText = commandText;
            return cmd;
        }

    }
}
