using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Serializers;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Interfaces;
using Play.Catalog.Service.Repositories;
using Play.Catalog.Service.Settings;

namespace Play.Catalog.Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServiceDependancies(this IServiceCollection services)
        {
            services.AddRepositories()
                .AddMongoDb();

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddSingleton<IItemsRepository, ItemsRepository>();
            services.AddSingleton<IRepository<Item>>(serviceProvider =>
            {
                var database = serviceProvider.GetService<MongoDB.Driver.IMongoDatabase>();
                return new Repository<Item>(database, "items");
            });

            return services;
        }

        private static IServiceCollection AddMongoDb(this IServiceCollection services)
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