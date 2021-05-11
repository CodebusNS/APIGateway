using APIGateway.Core.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Ocelot.Configuration.Repository;
using Ocelot.DependencyInjection;
using System;

namespace APIGateway.Core.OcelotAddin
{
    public static class RepositoryExtension
    {
        public static IOcelotBuilder AddConfigurationRepository(this IOcelotBuilder builder, Action<GatewayConfiguration> options)
        {
            builder.Services.Configure(options);
            builder.Services.AddSingleton(option => option.GetRequiredService<IOptions<GatewayConfiguration>>().Value);
            builder.Services.AddHostedService<DBConfigurationPoller>();
            builder.Services.AddSingleton<IFileConfigurationRepository, MySqlFileConfigurationRepository>();
            return builder;
        }
    }
}
