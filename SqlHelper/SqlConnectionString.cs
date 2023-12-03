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
            connectionString.ThrowExceptionIfNull("Missing section {0} for connection string", sectionName);
            return new SqlConnectionString()
            {
                connectionString = connectionString,
            };
        }
    }
}
