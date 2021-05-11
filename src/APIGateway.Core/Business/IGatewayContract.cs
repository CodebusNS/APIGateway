using APIGateway.Core.Model;
using Ocelot.Configuration.File;
using System.Threading.Tasks;

namespace APIGateway.Core.Business
{
    public interface IGatewayContract
    {
        Task<OperationResult> Get();

        Task<OperationResult> Post(FileConfiguration fileConfiguration);
    }
}
