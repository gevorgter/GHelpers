using GHelpers;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SqlHelper
{
    public struct SqlConnectionString
    {
        string connectionString { get; set; }

        public readonly SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        public static SqlConnectionString FromConfig(IConfiguration config, string sectionName = "db")
        {
            var connectionString = config.GetConnectionString(sectionName);
            connectionString.ThrowExceptionIfNull("Missing connection string {0}", sectionName);
            return new SqlConnectionString()
            {
                connectionString = connectionString,
            };
        }
    }
}