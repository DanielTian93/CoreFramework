using Core.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace Core.EventBus.Local
{
    public class CoreEventBusLocalModule : CoreModuleBase
    {
        public override void ConfigureServices(ServiceCollectionContext context)
        {
            context.Services
                .AddEventBus()
                .AddLocalMq();
        }
    }
}
