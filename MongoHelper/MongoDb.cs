using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace GHelpers
{
    public struct MongoDb
    {
        IMongoDatabase _database;

        public static MongoDb FromConfig(IConfiguration config, string dbServer = "mongoServer", string dbName = "mongoDb")
        {
            string? mongoServer = config.GetConnectionString(dbServer);
            string? mongoDb = config.GetConnectionString(dbName);
            mongoServer.ThrowExceptionIfNull("Missing connection string {0}", dbServer);
            mongoDb.ThrowExceptionIfNull("Missing connection string {0}", dbName);
            var dbClient = new MongoClient(mongoServer);
            var database = dbClient.GetDatabase(mongoDb);
            return new MongoDb()
            {
                _database = database,
            };

        }

        public readonly IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }
    }
}
