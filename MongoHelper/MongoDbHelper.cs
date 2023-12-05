using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Linq;

namespace GHelpers
{
    interface IMongoDbClass
    {

    }

    public static class MongoDbHelper
    {
        public static IServiceCollection UseMongoDbHelper(this IServiceCollection serviceCollection, Type? assemblyToScan = null)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
#pragma warning restore CS0618 // Type or member is obsolete
            BsonSerializer.RegisterSerializer(typeof(decimal), new DecimalSerializer(BsonType.Decimal128));
            BsonSerializer.RegisterSerializer(typeof(decimal?), new NullableSerializer<decimal>(new DecimalSerializer(BsonType.Decimal128)));
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            if (assemblyToScan != null)
            {

                Type baseType = typeof(IMongoDbClass);
                var assembly = assemblyToScan.Assembly;
                var allRepositories = assembly.GetTypes().Where(t => !t.IsAbstract && baseType.IsAssignableFrom(t));
                foreach (var t in allRepositories)
                    RegisterClassWithMongo(t);
            }

            return serviceCollection;
        }

        static void RegisterClassWithMongo(Type t)
        {
            var map = new BsonClassMap(t);
            map.AutoMap();
            map.SetDiscriminator(t.ToString());
            BsonClassMap.RegisterClassMap(map);
        }
    }
}
