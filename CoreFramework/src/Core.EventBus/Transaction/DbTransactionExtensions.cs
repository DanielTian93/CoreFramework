using System;
using Core.EventBus.Messaging;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace Core.EventBus.Transaction
{
    public static class DbTransactionExtensions
    {
        public static ITransaction BeginTransaction(this IDbConnection dbConnection,
            IMessagePublisher publisher, bool autoCommit = false)
        {
            if (dbConnection == null)
            {
                throw new ArgumentException(nameof(dbConnection));
            }
            if (publisher == null)
            {
                throw new ArgumentException(nameof(publisher));
            }
            var publisherBase = (MessagePublisherBase)publisher;
            if (dbConnection.State == ConnectionState.Closed) dbConnection.Open();
            var dbTransaction = dbConnection.BeginTransaction();
            var transaction = publisherBase.ServiceScopeFactory.CreateScope().ServiceProvider
                .GetRequiredService<ITransaction>();
            var transactionBase = (TransactionBase)transaction;
            transactionBase.DbTransaction = dbTransaction;
            transactionBase.AutoCommit = autoCommit;
            var sqlServerTransaction = transactionBase;
            ((MessagePublisherBase)publisher).TransactionAccessor.Transaction = sqlServerTransaction;
            return sqlServerTransaction;
        }

        public static ITransaction BeginTransaction(this DatabaseFacade database,
            IMessagePublisher publisher, bool autoCommit = false)
        {
            if (database == null)
            {
                throw new ArgumentException(nameof(database));
            }
            if (publisher == null)
            {
                throw new ArgumentException(nameof(publisher));
            }
            var publisherBase = (MessagePublisherBase)publisher;
            var dbTransaction = database.BeginTransaction();
            var transaction = publisherBase.ServiceScopeFactory.CreateScope().ServiceProvider
                .GetRequiredService<ITransaction>();
            var transactionBase = (TransactionBase)transaction;
            transactionBase.DbTransaction = dbTransaction;
            transactionBase.AutoCommit = autoCommit;
            var sqlServerTransaction = transactionBase;
            ((MessagePublisherBase)publisher).TransactionAccessor.Transaction = sqlServerTransaction;
            return sqlServerTransaction;
        }
    }
}
