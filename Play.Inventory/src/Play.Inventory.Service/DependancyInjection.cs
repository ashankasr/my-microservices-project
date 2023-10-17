using System;
using System.Net.Http;
using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Play.Common.Interfaces;
using Play.Common.MongoDb;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;
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

            Random jitter = new Random();

            services.AddHttpClient<CatalogClient>(Client =>
            {
                Client.BaseAddress = new Uri("https://localhost:5501");
            })
            .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
                5,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                    + TimeSpan.FromMilliseconds(jitter.Next(0, 1000)),
                onRetry: (outcome, timespan, retryAttempt) =>
                {
                    // avoid this code below. Test purpose only

                    var serviceProvider = services.BuildServiceProvider();
                    serviceProvider.GetService<ILogger<CatalogClient>>()?
                    .LogWarning($"Delaying for {timespan.TotalSeconds} seconds, then making a retry {retryAttempt}");
                }
            ))
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));

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