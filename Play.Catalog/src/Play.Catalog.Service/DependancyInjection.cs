using Microsoft.Extensions.DependencyInjection;
using Play.Catalog.Service.Entities;
using Play.Common.Interfaces;
using Play.Common.MongoDb;

namespace Play.Catalog.Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServiceDependancies(this IServiceCollection services)
        {
            services.AddRepositories();
            services.AddMongoDb();

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddSingleton<IRepository<Item>>(serviceProvider =>
            {
                var database = serviceProvider.GetService<MongoDB.Driver.IMongoDatabase>();
                return new Repository<Item>(database, "items");
            });

            return services;
        }
    }
}