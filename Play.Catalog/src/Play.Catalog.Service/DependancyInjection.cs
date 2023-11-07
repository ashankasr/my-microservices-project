using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Settings;
using Play.Common.Interfaces;
using Play.Common.MongoDb;
using Play.Common.Settings;

namespace Play.Catalog.Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServiceDependancies(this IServiceCollection services, ConfigurationManager configuration)
        {
            var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

            services.AddRepositories();
            services.AddMongoDb();

            services.AddMassTransit(options =>
            {
                options.UsingRabbitMq((context, configurator) =>
                {
                    var rabbitMQSettings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();

                    configurator.Host(rabbitMQSettings.Host);
                    configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
                });
            });

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