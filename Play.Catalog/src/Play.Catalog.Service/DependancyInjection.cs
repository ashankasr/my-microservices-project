using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Interfaces;
using Play.Catalog.Service.Repositories;
using Play.Catalog.Service.Settings;

namespace Play.Catalog.Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServiceDependancies(this IServiceCollection services, IConfiguration configuration)
        {
            var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
            var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();

            services.AddSingleton<IItemsRepository, ItemsRepository>();

            services.AddSingleton<IRepository<Item>>(serviceProvider =>
            {
                var database = serviceProvider.GetService<MongoDB.Driver.IMongoDatabase>();
                return new Repository<Item>(database, "items");
            });

            services.AddSingleton<MongoDB.Driver.IMongoDatabase>(serviceProvider =>
            {
                var mongoClient = new MongoDB.Driver.MongoClient(mongoDbSettings.ConnectionString);
                var database = mongoClient.GetDatabase(serviceSettings.ServiceName);

                return database;
            });

            return services;
        }
    }
}