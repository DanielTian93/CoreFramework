using Core.EventBus.Storage;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Core.EventBus
{
    public class EventBusBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public EventBusBackgroundService(
            IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var provider = _serviceScopeFactory.CreateScope().ServiceProvider;

            var options = provider.GetRequiredService<IOptions<EventBusOptions>>();
            var messageSubscribe = provider.GetRequiredService<IMessageSubscribe>();
            var storage = provider.GetRequiredService<IStorage>();

            storage?.InitializeAsync(stoppingToken);
            messageSubscribe?.Initialize(options.Value.AutoRegistrarHandlersAssemblies);
            return Task.CompletedTask;
        }
    }
}
