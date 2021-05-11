using APIGateway.Core.Model;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using APIGateway.Core.DbRepository;
using System.Linq;
using Ocelot.Configuration.Repository;
using Newtonsoft.Json;
using Ocelot.Configuration.File;
using Ocelot.Configuration.Setter;

namespace APIGateway.Core.Business
{
    public class GatewayServices : IGatewayContract
    {
        private readonly IFileConfigurationRepository _repo;
        private readonly IFileConfigurationSetter _setter;


        public GatewayServices(IServiceProvider serviceProvider)
        {
            _repo = serviceProvider.GetService<IFileConfigurationRepository>();
            _setter = serviceProvider.GetService<IFileConfigurationSetter>();
        }

        public async Task<OperationResult> Get()
        {
            var response = await _repo.Get();

            if (response.IsError)
                return OperationResult.CreateError(JsonConvert.SerializeObject(response.Errors));

            return OperationResult.CreateSuccess("ok", response.Data);
        }

        public async Task<OperationResult> Post(FileConfiguration fileConfiguration)
        {
            try
            {
                var response = await _setter.Set(fileConfiguration);
                if (response.IsError)
                    return OperationResult.CreateError(JsonConvert.SerializeObject(response.Errors));
                return OperationResult.CreateSuccess("ok", fileConfiguration);
            }
            catch (Exception ex)
            {
                return OperationResult.CreateError($"{ex.Message}:{ex.StackTrace}");
            }
        }
    }
}
