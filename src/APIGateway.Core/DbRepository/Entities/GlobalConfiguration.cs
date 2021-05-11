using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace APIGateway.Core.DbRepository.Entities
{
    /// <summary>
    /// 网关全局配置
    /// </summary>
    public class GlobalConfiguration
    {
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }

        public string RequestIdKey { get; set; }

        public string ServiceDiscoveryProvider { get; set; }

        public string RateLimitOptions { get; set; }

        public string QoSOptions { get; set; }

        public string BaseUrl { get; set; }

        public string LoadBalancerOptions { get; set; }

        public string DownstreamScheme { get; set; }

        public string HttpHandlerOptions { get; set; }

        public string DownstreamHttpVersion { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime? DeletedTime { get; set; }

        public bool Enable { get; set; }

        public virtual List<ReRoute> ReRoutes { get; set; }

    }
}
