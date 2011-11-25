// Altairis Web Security Toolkit
// Copyright © Michal A. Valasek - Altairis, 2006-2011 | www.altairis.cz 
// Licensed under terms of Microsoft Permissive License (MS-PL)

using System;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace Altairis.Web.Security {
    internal static class DatabaseExtensionMethods {

        public static DbConnection CreateDbConnection(this ConnectionStringSettings settings) {
            // Validate arguments
            if (settings == null) throw new ArgumentNullException("settings");
            if (string.IsNullOrEmpty(settings.ProviderName)) throw new ArgumentException("The ProviderName property cannot be empty.", "settings");
            if (string.IsNullOrEmpty(settings.ConnectionString)) throw new ArgumentException("The ConnectionString property cannot be empty.", "settings");

            var factory = DbProviderFactories.GetFactory(settings.ProviderName);
            var conn = factory.CreateConnection();
            conn.ConnectionString = settings.ConnectionString;
            return conn;
        }

        public static void AddParameterWithValue(this DbCommand cmd, string name, string value) {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.DbType = DbType.String;
            if (!string.IsNullOrEmpty(value)) p.Size = value.Length;
            p.Value = value;
            cmd.Parameters.Add(p);
        }

        public static void AddParameterWithValue(this DbCommand cmd, string name, int value) {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.DbType = DbType.Int32;
            p.Value = value;
            cmd.Parameters.Add(p);
        }

        public static void AddParameterWithValue(this DbCommand cmd, string name, Guid value) {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.DbType = DbType.Guid;
            p.Value = value;
            cmd.Parameters.Add(p);
        }

        public static void AddParameterWithValue(this DbCommand cmd, string name, DateTime value) {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.DbType = DbType.DateTime;
            p.Value = value;
            cmd.Parameters.Add(p);
        }

        public static void AddParameterWithValue(this DbCommand cmd, string name, bool value) {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.DbType = DbType.Boolean;
            p.Value = value;
            cmd.Parameters.Add(p);
        }

        public static void AddParameterWithValue(this DbCommand cmd, string name, byte[] value) {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.DbType = DbType.Binary;
            p.Size = value.Length;
            p.Value = value;
            cmd.Parameters.Add(p);
        }

    }
}
