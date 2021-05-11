using System;
using System.ComponentModel.DataAnnotations;

namespace APIGateway.Core.DbRepository.Entities
{
    /// <summary>
    /// 网关路由
    /// </summary>
    public class ReRoute
    {
        [Key]
        public string Id { get; set; }
        public string GlobalConfigurationId { get; set; }
        public string DownstreamPathTemplate { get; set; }
        public string UpstreamPathTemplate { get; set; }
        public string UpstreamHttpMethod { get; set; }
        public string DownstreamHttpMethod { get; set; }
        public string AddHeadersToRequest { get; set; }
        public string UpstreamHeaderTransform { get; set; }
        public string DownstreamHeaderTransform { get; set; }
        public string AddClaimsToRequest { get; set; }
        public string RouteClaimsRequirement { get; set; }
        public string AddQueriesToRequest { get; set; }
        public string ChangeDownstreamPathTemplate { get; set; }
        public string RequestIdKey { get; set; }
        public string FileCacheOptions { get; set; }
        public bool ReRouteIsCaseSensitive { get; set; }
        public string ServiceName { get; set; }
        public string ServiceNamespace { get; set; }
        public string DownstreamScheme { get; set; }
        public string QoSOptions { get; set; }
        public string LoadBalancerOptions { get; set; }
        public string RateLimitOptions { get; set; }
        public string AuthenticationOptions { get; set; }
        public string HttpHandlerOptions { get; set; }
        public string DownstreamHostAndPorts { get; set; }
        public string UpstreamHost { get; set; }
        public string Key { get; set; }
        public string DelegatingHandlers { get; set; }
        public int Priority { get; set; }
        public int Timeout { get; set; }
        public bool DangerousAcceptAnyServerCertificateValidator { get; set; }
        public string SecurityOptions { get; set; }
        public string DownstreamHttpVersion { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? DeletedTime { get; set; }
        public bool Enable { get; set; }
        public virtual GlobalConfiguration GlobalConfiguration { get; set; }
    }
}
