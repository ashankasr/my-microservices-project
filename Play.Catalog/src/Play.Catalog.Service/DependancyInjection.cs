using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Catalog.Service.Entities;
using Play.Common.Entities;
using Play.Common.Interfaces;
using Play.Common.MassTransit;
using Play.Common.MongoDb;
using Play.Common.Settings;
using Play.Common.Identity;

namespace Play.Catalog.Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServiceDependancies(this IServiceCollection services, ConfigurationManager configuration)
        {
            var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

            services.AddRepositories();
            services.AddMongoDb();
            services.AddMassTrasitWithRabbitMQ();

            services.AddAuth();

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddRepository<Item>("items");

            return services;
        }

        private static IServiceCollection AddRepository<T>(this IServiceCollection services, string documentCollectionName) where T : BaseEntity
        {
            services.AddSingleton<IRepository<T>>(serviceProvider =>
            {
                var database = serviceProvider.GetService<MongoDB.Driver.IMongoDatabase>();
                return new Repository<T>(database, documentCollectionName);
            });

            return services;
        }

        private static IServiceCollection AddAuth(this IServiceCollection services)
        {
            // services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            // .AddJwtBearer(options =>
            // {
            //     options.Authority = "https://localhost:5701";
            //     options.Audience = serviceName;
            // });

            services.AddJwtBearerAuthentication(); // From Play.Common.Identity

            return services;
        }
    }
}