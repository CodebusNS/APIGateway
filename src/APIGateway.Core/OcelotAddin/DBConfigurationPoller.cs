using APIGateway.Core.Model;
using Microsoft.Extensions.Hosting;
using Ocelot.Configuration.Creator;
using Ocelot.Configuration.Repository;
using Ocelot.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace APIGateway.Core.OcelotAddin
{
    public class DBConfigurationPoller : IHostedService
    {
        private readonly IOcelotLogger _logger;
        private readonly IFileConfigurationRepository _repo;
        private readonly GatewayConfiguration _option;
        private Timer _timer;
        private bool _polling;
        private readonly IInternalConfigurationRepository _internalConfigRepo;
        private readonly IInternalConfigurationCreator _internalConfigCreator;

        public DBConfigurationPoller(IOcelotLoggerFactory ocelotLoggerFactory,
            IFileConfigurationRepository fileConfigurationRepository,
            IInternalConfigurationRepository internalConfigurationRepository,
            IInternalConfigurationCreator internalConfigurationCreator,
            GatewayConfiguration gatewayConfiguration)
        {
            _internalConfigRepo = internalConfigurationRepository;
            _internalConfigCreator = internalConfigurationCreator;
            _logger = ocelotLoggerFactory.CreateLogger<DBConfigurationPoller>();
            _repo = fileConfigurationRepository;
            _option = gatewayConfiguration;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_option.AutoUpdate)
            {
                _logger.LogInformation($"{nameof(DBConfigurationPoller)} is starting.");
                _timer = new Timer(async x =>
                {
                    if (_polling)
                    {
                        return;
                    }
                    _polling = true;
                    await Poll();
                    _polling = false;
                }, null, _option.UpdateInterval, _option.UpdateInterval);
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_option.AutoUpdate)
            {
                _logger.LogInformation($"{nameof(DBConfigurationPoller)} is stopping.");
                _timer?.Change(Timeout.Infinite, 0);
            }
            return Task.CompletedTask;
        }

        private async Task Poll()
        {
            var fileConfig = await _repo.Get();

            if (fileConfig.IsError)
            {
                _logger.LogWarning($"error geting file config, errors are {string.Join(",", fileConfig.Errors.Select(x => x.Message))}");
                return;
            }
            else
            {
                var config = await _internalConfigCreator.Create(fileConfig.Data);
                if (!config.IsError)
                {
                    _internalConfigRepo.AddOrReplace(config.Data);
                }
            }
            _logger.LogInformation("Finished polling");
        }
    }
}
