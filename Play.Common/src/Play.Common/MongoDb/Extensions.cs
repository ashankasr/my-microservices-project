using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Serializers;
using Play.Common.Settings;

namespace Play.Common.MongoDb
{
    public static class Extensions
    {
        public static IServiceCollection AddMongoDb(this IServiceCollection services) 
        {
            MongoDB.Bson.Serialization.BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
            MongoDB.Bson.Serialization.BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(MongoDB.Bson.BsonType.String));

            services.AddSingleton<MongoDB.Driver.IMongoDatabase>(serviceProvider =>
            {
                var configuration = serviceProvider.GetService<IConfiguration>();

                var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();

                var mongoClient = new MongoDB.Driver.MongoClient(mongoDbSettings.ConnectionString);
                var database = mongoClient.GetDatabase(serviceSettings.ServiceName);

                return database;
            });
            
            return services;
        }
    }
}