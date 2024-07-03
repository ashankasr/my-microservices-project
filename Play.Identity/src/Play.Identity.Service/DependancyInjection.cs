using System;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Play.Common.MassTransit;
using Play.Common.Settings;
using Play.Identity.Service.Entities;
using Play.Identity.Service.Exceptions;
using Play.Identity.Service.HostedServices;
using Play.Identity.Service.Settings;

namespace Play.Identity.Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServiceDependancies(this IServiceCollection services, ConfigurationManager configuration)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

            var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
            var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();

            var identityServerSettings = configuration.GetSection(nameof(IdentityServerSettings)).Get<IdentityServerSettings>();

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<ApplicationRole>()
                .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>
                (
                    mongoDbSettings.ConnectionString,
                    serviceSettings.ServiceName
                );

            services.AddMassTrasitWithRabbitMQ(retryConfigurator =>
            {
                retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));
                retryConfigurator.Ignore<UnknownUserException>();
                retryConfigurator.Ignore<InsufficientGilException>();
            });

            services.Configure<IdentitySettings>(configuration.GetSection(nameof(IdentitySettings)));

            services.AddIdentityServer(options =>
            {
                options.Events.RaiseSuccessEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseErrorEvents = true;
            })
            .AddAspNetIdentity<ApplicationUser>()
            .AddInMemoryApiScopes(identityServerSettings.ApiScopes)
            .AddInMemoryApiResources(identityServerSettings.ApiResources)
            .AddInMemoryClients(identityServerSettings.Clients)
            .AddInMemoryIdentityResources(identityServerSettings.IdentityResources)
            .AddDeveloperSigningCredential();

            services.AddLocalApiAuthentication();

            services.AddHostedService<IdentitySeedHostedService>();

            return services;
        }
    }
}
