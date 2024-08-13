using Microsoft.Extensions.DependencyInjection;
using SnagIt.API.Core.Application.Authorisation;
using System.Reflection;


namespace SnagIt.API.Core.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        // So we don't have to install AutoFac for registering just this one interface
        // inspired from:
        // https://theburningmonk.com/2010/09/net-tips-how-to-determine-if-a-type-implements-a-generic-interface-type/
        public static IServiceCollection RegisterGenericImplementations(this IServiceCollection services)
        {
            var policyType = typeof(IAuthoriseRequestPolicy<>);

            var implementations = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(type => type.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == policyType));


            foreach (var implementation in implementations)
            {
                var interfaceType = implementation.GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == policyType);

                services.AddTransient(interfaceType, implementation);
            }

            return services;
        }
    }
}
