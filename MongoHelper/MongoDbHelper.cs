using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using System;

namespace GHelpers
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class MongoDbClassAttribute : Attribute
    {
        public string? name;

        public MongoDbClassAttribute(string? name = null)
        {
            this.name = name;
        }
    }

    public static class MongoDbHelper
    {
        public static AttributeMap mongoDbHelperMapper = new AttributeMap(typeof(MongoDbClassAttribute), RegisterClassWithMongo);

        public static IServiceCollection UseMongoDb(this IServiceCollection serviceCollection)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
#pragma warning restore CS0618 // Type or member is obsolete
            BsonSerializer.RegisterSerializer(typeof(decimal), new DecimalSerializer(BsonType.Decimal128));
            BsonSerializer.RegisterSerializer(typeof(decimal?), new NullableSerializer<decimal>(new DecimalSerializer(BsonType.Decimal128)));
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            ConventionRegistry.Register("IgnoreIfNullConvention", new ConventionPack { new IgnoreIfNullConvention(true) }, t => true);
            return serviceCollection;
        }

        public static void RegisterClassWithMongo(IServiceCollection serviceCollection, Attribute attr, Type t)
        {
            MongoDbClassAttribute attribute = (MongoDbClassAttribute)attr;
            string name = attribute.name ?? t.ToString();
            var map = new BsonClassMap(t);
            map.AutoMap();
            map.SetDiscriminator(name);
            BsonClassMap.RegisterClassMap(map);
        }
    }
}
