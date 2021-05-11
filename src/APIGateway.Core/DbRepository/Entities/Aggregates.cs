using System;
using System.ComponentModel.DataAnnotations;

namespace APIGateway.Core.DbRepository.Entities
{
    public class Aggregates
    {
        [Key]
        public string Id { get; set; }
        public string ReRouteKeys { get; set; }
        public string ReRouteKeysConfig { get; set; }
        public string UpstreamPathTemplate { get; set; }
        public string UpstreamHost { get; set; }
        public bool ReRouteIsCaseSensitive { get; set; }
        public string Aggregator { get; set; }
        public int Priority { get; set; }
        public bool Enable { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? DeletedTime { get; set; }
    }
}
