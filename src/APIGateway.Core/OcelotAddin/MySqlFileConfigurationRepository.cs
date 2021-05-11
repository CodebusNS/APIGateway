using Ocelot.Configuration.File;
using Ocelot.Configuration.Repository;
using Ocelot.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using APIGateway.Core.DbRepository;
using APIGateway.Core.DbRepository.Entities;
using Ocelot.Configuration.ChangeTracking;

namespace APIGateway.Core.OcelotAddin
{
    public class MySqlFileConfigurationRepository : IFileConfigurationRepository
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IOcelotConfigurationChangeTokenSource _changeTokenSource;

        public MySqlFileConfigurationRepository(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _changeTokenSource = serviceProvider.GetRequiredService<IOcelotConfigurationChangeTokenSource>();
        }

        public async Task<Response<FileConfiguration>> Get()
        {
            var _gatewayDbContext = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<GatewayDbContext>();
            var dbGlobalConfig = _gatewayDbContext.GlobalConfiguration.Where(x => x.Enable).FirstOrDefault();

            if (dbGlobalConfig == null)
                throw new Exception("未监测到任何可用的配置信息");
            if (string.IsNullOrEmpty(dbGlobalConfig.BaseUrl))
                throw new Exception("BaseUrl不可为空");

            // Globalconfig
            var fileConfiguration = new FileConfiguration();
            var fileGlobalConfiguration = new FileGlobalConfiguration();
            fileGlobalConfiguration.BaseUrl = dbGlobalConfig.BaseUrl;
            fileGlobalConfiguration.DownstreamScheme = dbGlobalConfig.DownstreamScheme;
            fileGlobalConfiguration.RequestIdKey = dbGlobalConfig.RequestIdKey;
            fileGlobalConfiguration.DownstreamHttpVersion = dbGlobalConfig.DownstreamHttpVersion;
            if (!string.IsNullOrEmpty(dbGlobalConfig.HttpHandlerOptions))
            {
                fileGlobalConfiguration.HttpHandlerOptions = JsonConvert.DeserializeObject<FileHttpHandlerOptions>(dbGlobalConfig.HttpHandlerOptions);
            }
            if (!string.IsNullOrEmpty(dbGlobalConfig.LoadBalancerOptions))
            {
                fileGlobalConfiguration.LoadBalancerOptions = JsonConvert.DeserializeObject<FileLoadBalancerOptions>(dbGlobalConfig.LoadBalancerOptions);
            }
            if (!string.IsNullOrEmpty(dbGlobalConfig.QoSOptions))
            {
                fileGlobalConfiguration.QoSOptions = JsonConvert.DeserializeObject<FileQoSOptions>(dbGlobalConfig.QoSOptions);
            }
            if (!string.IsNullOrEmpty(dbGlobalConfig.ServiceDiscoveryProvider))
            {
                fileGlobalConfiguration.ServiceDiscoveryProvider = JsonConvert.DeserializeObject<FileServiceDiscoveryProvider>(dbGlobalConfig.ServiceDiscoveryProvider);
            }
            if (!string.IsNullOrEmpty(dbGlobalConfig.RateLimitOptions))
            {
                fileGlobalConfiguration.RateLimitOptions = JsonConvert.DeserializeObject<FileRateLimitOptions>(dbGlobalConfig.RateLimitOptions);
            }
            fileConfiguration.GlobalConfiguration = fileGlobalConfiguration;

            var routeresult = dbGlobalConfig.ReRoutes;
            if (routeresult == null || routeresult.Count <= 0)
                return await Task.FromResult(new OkResponse<FileConfiguration>(null));

            // Reroutes
            var reroutelist = new List<FileReRoute>();
            foreach (var model in routeresult)
            {
                var fileReroute = new FileReRoute();

                if (!string.IsNullOrEmpty(model.AuthenticationOptions))
                {
                    fileReroute.AuthenticationOptions = JsonConvert.DeserializeObject<FileAuthenticationOptions>(model.AuthenticationOptions);
                }
                if (!string.IsNullOrEmpty(model.FileCacheOptions))
                {
                    fileReroute.FileCacheOptions = JsonConvert.DeserializeObject<FileCacheOptions>(model.FileCacheOptions);
                }
                if (!string.IsNullOrEmpty(model.DelegatingHandlers))
                {
                    fileReroute.DelegatingHandlers = JsonConvert.DeserializeObject<List<string>>(model.DelegatingHandlers);
                }
                if (!string.IsNullOrEmpty(model.LoadBalancerOptions))
                {
                    fileReroute.LoadBalancerOptions = JsonConvert.DeserializeObject<FileLoadBalancerOptions>(model.LoadBalancerOptions);
                }
                if (!string.IsNullOrEmpty(model.QoSOptions))
                {
                    fileReroute.QoSOptions = JsonConvert.DeserializeObject<FileQoSOptions>(model.QoSOptions);
                }
                if (!string.IsNullOrEmpty(model.DownstreamHostAndPorts))
                {
                    fileReroute.DownstreamHostAndPorts = JsonConvert.DeserializeObject<List<FileHostAndPort>>(model.DownstreamHostAndPorts);
                }
                if (!string.IsNullOrEmpty(model.HttpHandlerOptions))
                {
                    fileReroute.HttpHandlerOptions = JsonConvert.DeserializeObject<FileHttpHandlerOptions>(model.HttpHandlerOptions);
                }
                if (!string.IsNullOrEmpty(model.RateLimitOptions))
                {
                    fileReroute.RateLimitOptions = JsonConvert.DeserializeObject<FileRateLimitRule>(model.RateLimitOptions);
                }

                fileReroute.DownstreamPathTemplate = model.DownstreamPathTemplate;
                fileReroute.DownstreamScheme = model.DownstreamScheme;
                fileReroute.Key = model.Key ?? "";
                fileReroute.Priority = model.Priority;
                fileReroute.RequestIdKey = model.RequestIdKey ?? "";
                fileReroute.ServiceName = model.ServiceName ?? "";
                fileReroute.UpstreamHost = model.UpstreamHost ?? "";
                fileReroute.UpstreamHttpMethod = JsonConvert.DeserializeObject<List<string>>(model.UpstreamHttpMethod);
                fileReroute.UpstreamPathTemplate = model.UpstreamPathTemplate;
                fileReroute.DownstreamHttpVersion = model.DownstreamHttpVersion;
                reroutelist.Add(fileReroute);
            }
            fileConfiguration.ReRoutes = reroutelist;

            // Aggregates
            var dbAggregates = _gatewayDbContext.Aggregates.Where(x => x.Enable);
            foreach (var aggregate in dbAggregates)
            {
                var fileAggregate = new FileAggregateReRoute();
                if (!string.IsNullOrEmpty(aggregate.ReRouteKeys))
                    fileAggregate.ReRouteKeys = JsonConvert.DeserializeObject<List<string>>(aggregate.ReRouteKeys);
                if (!string.IsNullOrEmpty(aggregate.ReRouteKeysConfig))
                    fileAggregate.ReRouteKeysConfig = JsonConvert.DeserializeObject<List<AggregateReRouteConfig>>(aggregate.ReRouteKeysConfig);
                fileAggregate.UpstreamPathTemplate = aggregate.UpstreamPathTemplate;
                fileAggregate.UpstreamHost = aggregate.UpstreamHost;
                fileAggregate.ReRouteIsCaseSensitive = aggregate.ReRouteIsCaseSensitive;
                fileAggregate.Aggregator = aggregate.Aggregator;
                fileAggregate.Priority = aggregate.Priority;

                fileConfiguration.Aggregates.Add(fileAggregate);
            }

            if (fileConfiguration.ReRoutes == null || fileConfiguration.ReRoutes.Count <= 0)
                return await Task.FromResult(new OkResponse<FileConfiguration>(null));

            return await Task.FromResult(new OkResponse<FileConfiguration>(fileConfiguration));
        }

        public async Task<Response> Set(FileConfiguration fileConfiguration)
        {
            if (fileConfiguration == null)
                throw new Exception("未监测到任何可用的配置信息");
            if (string.IsNullOrEmpty(fileConfiguration.GlobalConfiguration.BaseUrl))
                throw new Exception("BaseUrl不可为空");

            var _gatewayDbContext = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<GatewayDbContext>();

            var dbGlobalConfigs = _gatewayDbContext.GlobalConfiguration.Where(x => x.Enable).ToList();
            dbGlobalConfigs.ForEach(x =>
            {
                x.Enable = false;
                x.ReRoutes.ForEach(m => m.Enable = false);
            });
            _gatewayDbContext.GlobalConfiguration.UpdateRange(dbGlobalConfigs);

            // Globalconfig
            var dbGlobalConfig = new GlobalConfiguration();
            dbGlobalConfig.Id = Guid.NewGuid().ToString("N");
            dbGlobalConfig.CreatedTime = DateTime.Now;
            dbGlobalConfig.Enable = true;
            dbGlobalConfig.BaseUrl = fileConfiguration.GlobalConfiguration.BaseUrl;
            dbGlobalConfig.DownstreamScheme = fileConfiguration.GlobalConfiguration.DownstreamScheme;
            dbGlobalConfig.RequestIdKey = fileConfiguration.GlobalConfiguration.RequestIdKey;
            dbGlobalConfig.DownstreamHttpVersion = fileConfiguration.GlobalConfiguration.DownstreamHttpVersion;
            dbGlobalConfig.HttpHandlerOptions = JsonConvert.SerializeObject(fileConfiguration.GlobalConfiguration.HttpHandlerOptions);
            dbGlobalConfig.LoadBalancerOptions = JsonConvert.SerializeObject(fileConfiguration.GlobalConfiguration.LoadBalancerOptions);
            dbGlobalConfig.QoSOptions = JsonConvert.SerializeObject(fileConfiguration.GlobalConfiguration.QoSOptions);
            dbGlobalConfig.ServiceDiscoveryProvider = JsonConvert.SerializeObject(fileConfiguration.GlobalConfiguration.ServiceDiscoveryProvider);
            dbGlobalConfig.RateLimitOptions = JsonConvert.SerializeObject(fileConfiguration.GlobalConfiguration.RateLimitOptions);
            dbGlobalConfig.ReRoutes = new List<ReRoute>();

            // Reroutes
            foreach (var model in fileConfiguration.ReRoutes)
            {
                var dbReroute = new ReRoute();
                dbReroute.Id = Guid.NewGuid().ToString("N");
                dbReroute.GlobalConfigurationId = dbGlobalConfig.Id;
                dbReroute.CreatedTime = dbGlobalConfig.CreatedTime;
                dbReroute.Enable = true;
                dbReroute.AuthenticationOptions = JsonConvert.SerializeObject(model.AuthenticationOptions);
                dbReroute.FileCacheOptions = JsonConvert.SerializeObject(model.FileCacheOptions);
                dbReroute.DelegatingHandlers = JsonConvert.SerializeObject(model.DelegatingHandlers);
                dbReroute.LoadBalancerOptions = JsonConvert.SerializeObject(model.LoadBalancerOptions);
                dbReroute.QoSOptions = JsonConvert.SerializeObject(model.QoSOptions);
                dbReroute.DownstreamHostAndPorts = JsonConvert.SerializeObject(model.DownstreamHostAndPorts);
                dbReroute.HttpHandlerOptions = JsonConvert.SerializeObject(model.HttpHandlerOptions);
                dbReroute.RateLimitOptions = JsonConvert.SerializeObject(model.RateLimitOptions);
                dbReroute.DownstreamPathTemplate = model.DownstreamPathTemplate;
                dbReroute.DownstreamScheme = model.DownstreamScheme;
                dbReroute.Key = model.Key ?? "";
                dbReroute.Priority = model.Priority;
                dbReroute.RequestIdKey = model.RequestIdKey ?? "";
                dbReroute.ServiceName = model.ServiceName ?? "";
                dbReroute.UpstreamHost = model.UpstreamHost ?? "";
                dbReroute.UpstreamHttpMethod = JsonConvert.SerializeObject(model.UpstreamHttpMethod);
                dbReroute.UpstreamPathTemplate = model.UpstreamPathTemplate;
                dbReroute.DownstreamHttpVersion = model.DownstreamHttpVersion;
                dbGlobalConfig.ReRoutes.Add(dbReroute);
            }

            // Aggregates
            var dbAggregates = _gatewayDbContext.Aggregates.Where(x => x.Enable).ToList();
            dbAggregates.ForEach(x => x.Enable = false);
            _gatewayDbContext.Aggregates.UpdateRange(dbAggregates);
            dbAggregates = new List<Aggregates>();
            foreach (var aggregate in fileConfiguration.Aggregates)
            {
                var dbAggregate = new Aggregates();
                dbAggregate.Id = Guid.NewGuid().ToString("N");
                dbAggregate.CreatedTime = dbGlobalConfig.CreatedTime;
                dbAggregate.Enable = true;
                dbAggregate.ReRouteKeys = JsonConvert.SerializeObject(aggregate.ReRouteKeys);
                dbAggregate.ReRouteKeysConfig = JsonConvert.SerializeObject(aggregate.ReRouteKeysConfig);
                dbAggregate.UpstreamPathTemplate = aggregate.UpstreamPathTemplate;
                dbAggregate.UpstreamHost = aggregate.UpstreamHost;
                dbAggregate.ReRouteIsCaseSensitive = aggregate.ReRouteIsCaseSensitive;
                dbAggregate.Aggregator = aggregate.Aggregator;
                dbAggregate.Priority = aggregate.Priority;
                dbAggregates.Add(dbAggregate);
            }
            await _gatewayDbContext.GlobalConfiguration.AddAsync(dbGlobalConfig);
            await _gatewayDbContext.Aggregates.AddRangeAsync(dbAggregates);

            var changeCount = await _gatewayDbContext.SaveChangesAsync();

            _changeTokenSource.Activate();
            return await Task.FromResult(new OkResponse());
        }
    }
}