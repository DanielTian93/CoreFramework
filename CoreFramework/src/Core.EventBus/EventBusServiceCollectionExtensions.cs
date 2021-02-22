using Core.EventBus;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Reflection;
using Core.EventBus.Transaction;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventBusServiceCollectionExtensions
    {
        public static EventBusBuilder AddEventBus(this IServiceCollection services, Action<EventBusOptions> options = null)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            services.TryAddSingleton<IMessageHandlerManager, MessageHandlerManager>();
            services.TryAddSingleton<IMessageHandlerProvider, MessageHandlerProvider>();
            services.TryAddSingleton<ITransactionAccessor, TransactionAccessor>();
            services.AddHostedService<EventBusBackgroundService>();
            if (options != null)
                services.Configure(options);
            return new EventBusBuilder(services);
        }

        public static IServiceCollection TryRegistrarMessageHandlers(this IServiceCollection services, Assembly[] assemblies)
        {
            if (assemblies == null) return services;
            var handlerTypes = MessageHandlerExtensions.GetHandlerTypes(assemblies);
            foreach (var handlerType in handlerTypes)
            {
                var baseHandlerTypes = MessageHandlerExtensions.GetBaseHandlerTypes(handlerType);
                foreach (var baseHandlerType in baseHandlerTypes)
                {
                    services.TryAddTransient(baseHandlerType, handlerType);
                }
            }
            return services;
        }
    }
}
