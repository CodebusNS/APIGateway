namespace APIGateway.Core.Model
{
    public class GatewayConfiguration
    {
        public bool AutoUpdate { get; set; } = false;

        public int UpdateInterval { get; set; }
    }
}
