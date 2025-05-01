using Microsoft.Extensions.DependencyInjection;
using ConfigMaster.Application.Services;
using ConfigMaster.Domain.Interfaces;
using ConfigMaster.Infrastructure.Repositories;

namespace ConfigMaster.Infra.Ioc
{
    public static class DependencyInjection
    {
        public static void AddDependencies(this IServiceCollection services)
        {
            services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
            services.AddScoped<ConfigurationService>();
        }
    }
}