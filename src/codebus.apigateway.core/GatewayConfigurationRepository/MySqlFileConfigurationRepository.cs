﻿using codebus.apigateway.core.Entities;
using Ocelot.Configuration.File;
using Ocelot.Configuration.Repository;
using Ocelot.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;

namespace codebus.apigateway.core.GatewayConfigurationRepository
{
    public class MySqlFileConfigurationRepository : IFileConfigurationRepository
    {
        private readonly IServiceProvider _serviceProvider;

        public MySqlFileConfigurationRepository(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<Response<FileConfiguration>> Get()
        {
           var  _gatewayDbContext = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<GatewayDbContext>();
            var dbGlobalConfig = _gatewayDbContext.GlobalConfiguration.Where(x => x.Enable).FirstOrDefault();

            if (dbGlobalConfig == null)
                throw new Exception("未监测到任何可用的配置信息");
            if (string.IsNullOrEmpty(dbGlobalConfig.BaseUrl))
                throw new Exception("BaseUrl不可为空");

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
                fileReroute.Key = model.RequestIdKey ?? "";
                fileReroute.Priority = model.Priority;
                fileReroute.RequestIdKey = model.RequestIdKey ?? "";
                fileReroute.ServiceName = model.ServiceName ?? "";
                fileReroute.UpstreamHost = model.UpstreamHost ?? "";
                fileReroute.UpstreamHttpMethod = JsonConvert.DeserializeObject<List<string>>(model.UpstreamHttpMethod);
                fileReroute.UpstreamPathTemplate = model.UpstreamPathTemplate;
                fileReroute.DownstreamHttpVersion = model.DownstreamHttpVersion;

                var dbAggregate = _gatewayDbContext.Aggregates.FirstOrDefault(x => x.ReRouteId == model.Id && x.Enable);
                if (dbAggregate != null)
                {
                    var aggregate = new FileAggregateReRoute();
                    if (!string.IsNullOrEmpty(dbAggregate.ReRouteKeys))
                        aggregate.ReRouteKeys = JsonConvert.DeserializeObject<List<string>>(dbAggregate.ReRouteKeys);
                    if (!string.IsNullOrEmpty(dbAggregate.ReRouteKeysConfig))
                        aggregate.ReRouteKeysConfig = JsonConvert.DeserializeObject<List<AggregateReRouteConfig>>(dbAggregate.ReRouteKeysConfig);
                    aggregate.UpstreamPathTemplate = dbAggregate.UpstreamPathTemplate;
                    aggregate.UpstreamHost = dbAggregate.UpstreamHost;
                    aggregate.ReRouteIsCaseSensitive = dbAggregate.ReRouteIsCaseSensitive;
                    aggregate.Aggregator = dbAggregate.Aggregator;
                    aggregate.Priority = dbAggregate.Priority;

                    fileConfiguration.Aggregates.Add(aggregate);
                }
                reroutelist.Add(fileReroute);
            }
            fileConfiguration.ReRoutes = reroutelist;

            if (fileConfiguration.ReRoutes == null || fileConfiguration.ReRoutes.Count <= 0)
                return await Task.FromResult(new OkResponse<FileConfiguration>(null));

            return await Task.FromResult(new OkResponse<FileConfiguration>(fileConfiguration));
        }

        public async Task<Response> Set(FileConfiguration fileConfiguration)
        {
            return await Task.FromResult(new OkResponse());
        }
    }
}