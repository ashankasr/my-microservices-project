using System;
using System.Net.Http;
using Amazon.Runtime.Internal.Util;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Play.Common.Entities;
using Play.Common.Identity;
using Play.Common.Interfaces;
using Play.Common.MassTransit;
using Play.Common.MongoDb;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;
using Play.Inventory.Service.Exceptions;
using Polly;
using Polly.Timeout;

namespace Play.Inventory.Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServiceDependancies(this IServiceCollection services)
        {
            services.AddRepositories();
            services.AddMongoDb();

            // Synchronous microservice communication
            // services.AddCatalogHttpClient();

            // Asynchronous configuration
            services.AddMassTrasitWithRabbitMQ(retryConfigurator =>
            {
                retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));
                retryConfigurator.Ignore(typeof(UnknowItemException));
            });

            services.AddAuth();

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddRepository<InventoryItem>("inventoryitems");
            services.AddRepository<CatalogItem>("catalogitems");

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
            services.AddJwtBearerAuthentication(); // From Play.Common.Identity

            return services;
        }

        private static IServiceCollection AddCatalogHttpClient(this IServiceCollection services)
        {
            Random jitter = new Random();

            services.AddHttpClient<CatalogClient>(Client =>
            {
                Client.BaseAddress = new Uri("https://localhost:5501");
            })
            .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
                5, // Number of attempts
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                    + TimeSpan.FromMilliseconds(jitter.Next(0, 1000)), // Time wait for the next attempt
                onRetry: (outcome, timespan, retryAttempt) =>
                {
                    // On the retry

                    // Avoid creating a logger like code below. Test purpose only
                    var serviceProvider = services.BuildServiceProvider();
                    serviceProvider.GetService<ILogger<CatalogClient>>()?
                    .LogWarning($"Delaying for {timespan.TotalSeconds} seconds, then making a retry {retryAttempt}");
                }
            ))
            .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
                3, // Number of failed request before opening the circuit
                TimeSpan.FromSeconds(15),
                onBreak: (outcome, timespan) =>
                {
                    var serviceProvider = services.BuildServiceProvider();
                    serviceProvider.GetService<ILogger<CatalogClient>>()?
                    .LogWarning($"Openning cirtcuit for {timespan.TotalSeconds} seconds...");
                },
                onReset: () =>
                {
                    var serviceProvider = services.BuildServiceProvider();
                    serviceProvider.GetService<ILogger<CatalogClient>>()?
                    .LogWarning($"Closing the cirtcuit...");
                }
            ))
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));

            return services;
        }
    }
}