﻿using Core.Modularity;
using Core.Modularity.Attribute;

namespace Core.EventBus.Local
{
    [DependsOn(typeof(CoreEventBusModule))]
    public class CoreEventBusLocalModule:CoreModuleBase
    {
        public override void ConfigureServices(ServiceCollectionContext context)
        {
            context.Services.AddLocal();
        }
    }
}
