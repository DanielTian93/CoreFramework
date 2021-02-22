using Core.Modularity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.EventBus.SqlServer
{
    public class CoreEventBusSqlServerModule : CoreModuleBase
    {
        private readonly IConfiguration _configuration;

        public CoreEventBusSqlServerModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public override void ConfigureServices(ServiceCollectionContext context)
        {
            context.Services.Configure<EventBusSqlServerOptions>(_configuration.GetSection("EventBusStorageConnection"));
            context.Services
                .AddEventBus()
                .AddSqlServer();
        }
    }
}
