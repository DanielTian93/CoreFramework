using Core.EventBus.Transaction;
using System.Threading.Tasks;
using Core.EventBus.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Core.EventBus.Messaging
{
    public abstract class MessagePublisherBase : IMessagePublisher
    {
        protected readonly IStorage Storage;
        public IServiceScopeFactory ServiceScopeFactory { get; }

        protected MessagePublisherBase(
            IServiceScopeFactory serviceScopeFactory,
            IStorage storage,
            ITransactionAccessor transactionAccessor)
        {
            Storage = storage;
            ServiceScopeFactory = serviceScopeFactory;
            TransactionAccessor = transactionAccessor;
        }
        public ITransactionAccessor TransactionAccessor { get; }

        public Task PublishAsync<T>(T message)
            where T : class, IMessage
        {
            var transaction = (TransactionBase)TransactionAccessor.Transaction;

            Storage.StoreMessage(new MediumMessage(message), transaction?.DbTransaction);

            if (transaction == null)
            {
                //未开启事务
                SendAsync(message);
            }
            else
            {
                if (transaction.AutoCommit)
                {
                    TransactionAccessor.Transaction.Commit();
                    SendAsync(message);
                }
                else
                {
                    transaction.AddMessage(message);
                }
            }
            return Task.CompletedTask;
        }

        public abstract Task SendAsync<T>(T message)
            where T : class, IMessage;
    }
}
