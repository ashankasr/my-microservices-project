using Microsoft.Extensions.DependencyInjection;
using Play.Common.Interfaces;
using Play.Common.MongoDb;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service
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
            services.AddSingleton<IRepository<InventoryItem>>(serviceProvider =>
            {
                var database = serviceProvider.GetService<MongoDB.Driver.IMongoDatabase>();
                return new Repository<InventoryItem>(database, "inventoryitem");
            });

            return services;
        }
    }
}