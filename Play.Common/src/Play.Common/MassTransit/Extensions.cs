using System;
using System.Reflection;
using GreenPipes;
using GreenPipes.Configurators;
using MassTransit;
using MassTransit.Definition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Settings;

namespace Play.Common.MassTransit
{
    public static class Extensions
    {
        public static IServiceCollection AddMassTrasitWithRabbitMQ(this IServiceCollection services,
            Action<IRetryConfigurator> configureRetries = null)
        {
            services.AddMassTransit(options =>
            {
                options.AddConsumers(Assembly.GetEntryAssembly());

                options.UsingPlayEconomyRabbitMq(configureRetries);
            });

            return services;
        }

        public static void UsingPlayEconomyRabbitMq(this IBusRegistrationConfigurator options, Action<IRetryConfigurator> configureRetries = null)
        {
            options.UsingRabbitMq((context, configurator) =>
            {
                var configuration = context.GetService<IConfiguration>();
                var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                var rabbitMQSettings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();

                configurator.Host(rabbitMQSettings.Host);
                configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));

                if (configureRetries == null)
                {
                    configureRetries = (retryConfigurator) => retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));
                }

                configurator.UseMessageRetry(configureRetries);
            });
        }
    }
}