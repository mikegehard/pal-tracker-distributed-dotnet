using System;
using System.Data.Common;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

namespace DatabaseSupport
{
    public class VcapDataSourceConfig : IDataSourceConfig
    {
        public DbConnection GetConnection()
        {
            var json = Environment.GetEnvironmentVariable("VCAP_SERVICES");
            var connection = GetConnectionString(json);
            return new MySqlConnection(connection); // todo - use db pool lib?
        }

        private static string GetConnectionString(string json)
        {
            var services = JObject.Parse(json);
            var credentials = services["p-mysql"].First["credentials"];
            var host = credentials["hostname"];
            var port = credentials["port"];
            var database = credentials["name"];
            var user = credentials["username"];
            var password = credentials["password"];

            return $"Server={host};User id={user};password={password};Port={port};Database={database};";
        }
    }
}