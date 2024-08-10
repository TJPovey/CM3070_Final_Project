using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using SnagIt.API.Core.Application.Behaviours;
using SnagIt.API.Core.Infrastructure.Middleware;
using SnagIt.API.Core.Infrastructure.Repositiories;
using SnagIt.API.Core.Infrastructure.Repositiories.Cosmos;
using SnagIt.API.Core.Infrastructure.Repositiories.Cosmos.Clients;
using SnagIt.API.Core.Infrastructure.Services;
using System.Text;


var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(builder =>
    {
        builder
            .UseMiddleware<HttpContextAccessorMiddleware>()
            .UseWhen<VerifyTokenMiddleware>(context =>
            {
                return context.FunctionDefinition.Name != "API_User_Post" && 
                       context.FunctionDefinition.Name != "API_User_Login";
            });
    })
    .ConfigureServices(services =>
    {
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
            .AddHttpContextAccessor()
            .AddLogging()
            .AddSingleton<IJwtSecurityTokenHandlerService, JwtSecurityTokenHandlerService>()
            .AddSingleton<IManagementCosmosClient, ManagementCosmosClient>()
            .AddSingleton<IUserCosmosClient, UserCosmosClient>()
            .AddSingleton<IUserRepository, UserRepository>()
            .AddSingleton<IPropertyRepository, PropertyRepository>();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
                {
                    var signingKey = Environment.GetEnvironmentVariable("SigningKey");
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.ASCII.GetBytes(signingKey)),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = "SnagItApp",
                        ValidAudience = "SnagItClient" 
                    };
                });

        services.AddAuthorization();

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(Program).Assembly);
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorisationBehavior<,>), ServiceLifetime.Transient);
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>), ServiceLifetime.Transient);
        });
        
        services.AddMvcCore().AddNewtonsoftJson(x =>
            x.SerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb));

    })
    .Build();

host.Run();
