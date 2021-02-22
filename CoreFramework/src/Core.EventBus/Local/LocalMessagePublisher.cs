using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Core.EventBus.Local
{
    public class LocalMessagePublisher : IMessagePublisher
    {
        private readonly ILogger<LocalMessagePublisher> _logger;
        private readonly IMessageHandlerProvider _messageHandlerProvider;

        public LocalMessagePublisher(
            ILogger<LocalMessagePublisher> logger,
            IMessageHandlerProvider messageHandlerProvider)
        {
            _logger = logger;
            _messageHandlerProvider = messageHandlerProvider;
        }

        public Task PublishAsync<T>(T message)
            where T : class, IMessage
        {
            var messageHandlers = _messageHandlerProvider
                .GetHandlers(typeof(T))
                .ToList();

            if (messageHandlers.Any())
            {
                foreach (var messageHandler in messageHandlers)
                {
                    (messageHandler as IMessageHandler<T>)?.HandAsync(message);
                }
            }
            else
            {
                var messageName = MessageNameAttribute.GetNameOrDefault(message.GetType());
                _logger.LogWarning("No subscription for local memory message: {eventName}", messageName);
            }
            return Task.CompletedTask;
        }
    }
}
