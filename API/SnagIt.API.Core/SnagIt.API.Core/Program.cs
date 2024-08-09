using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using SnagIt.API.Core.Application.Authorisation;
using SnagIt.API.Core.Application.Behaviours;
using SnagIt.API.Core.Infrastructure.Repositiories;
using SnagIt.API.Core.Infrastructure.Repositiories.Cosmos;
using SnagIt.API.Core.Infrastructure.Repositiories.Cosmos.Clients;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Text;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services
            .AddHttpContextAccessor()
            .AddLogging();

        services.AddSingleton(x =>
        {
            var cosmosClient = new CosmosClient(
                Environment.GetEnvironmentVariable("CosmosDBConnectionString"))
            {
                ClientOptions =
                    {
                        Serializer = new NewtonsoftJsonCosmosSerializer()
                    }
            };

            return cosmosClient;
        });

        services
            .AddSingleton<IManagementCosmosClient, ManagementCosmosClient>()
            .AddSingleton<IUserRepository, UserRepository>();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
                {
                    var signingKey = Environment.GetEnvironmentVariable("CosmosDBConnectionString");
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.ASCII.GetBytes(signingKey)),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = "SnagItApp",
                        ValidAudiences = new[] { "http://localhost:4200/", "postman" }
                    };
                });

        services.AddAuthorization();

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(Program).Assembly);
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>), ServiceLifetime.Transient);
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorisationBehavior<,>), ServiceLifetime.Transient);
        });
        
        services.AddMvcCore().AddNewtonsoftJson(x =>
            x.SerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb));

    })
    .ConfigureAppConfiguration((a, b) =>
    {
        
    })
    .Build();

host.Run();
