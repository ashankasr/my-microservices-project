using MassTransit;
using Play.Common.Identity;
using Play.Common.MassTransit;
using Play.Common.MongoDb;
using Play.Common.Settings;
using Play.Trading.Service.StateMachines;

namespace Play.Trading.Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServiceDependancies(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddMongoDb();
            services.AddMassTransitExt(configuration);

            services.AddAuth();

            return services;
        }

        private static IServiceCollection AddMassTransitExt(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(options =>
            {
                options.UsingPlayEconomyRabbitMq();
                options.AddSagaStateMachine<PurchaseStateMachine, PurchaseState>()
                .MongoDbRepository(rr =>
                {
                    var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                    var mongoSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();

                    rr.Connection = mongoSettings.ConnectionString;
                    rr.DatabaseName = serviceSettings.ServiceName;
                });
            });

            services.AddMassTransitHostedService();

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

            // services.AddAuthorization(options =>
            // {
            //     options.AddPolicy(Policies.Read, policy =>
            //     {
            //         policy.RequireClaim("scope", "catalog.readaccess", "catalog.fullaccess");
            //     });

            //     options.AddPolicy(Policies.Write, policy =>
            //     {
            //         policy.RequireRole("Admin");
            //         policy.RequireClaim("scope", "catalog.writeaccess", "catalog.fullaccess");
            //     });
            // });

            return services;
        }
    }
}