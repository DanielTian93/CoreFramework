using System.Data;
using Core.EventBus.Storage;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore.Storage;

namespace Core.EventBus.SqlServer
{
    public class SqlServerStorage : IStorage
    {
        private readonly IOptions<EventBusSqlServerOptions> _options;
        private readonly ILogger<SqlServerStorage> _logger;

        public SqlServerStorage(
            IOptions<EventBusSqlServerOptions> options,
            ILogger<SqlServerStorage> logger)
        {
            _options = options;
            _logger = logger;
        }

        /// <summary>
        /// 初始化消息表
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;
            var sql = @"
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EventBusMessages')
BEGIN
CREATE TABLE [EventBusMessages](
	[Id] [nvarchar](200) NOT NULL PRIMARY KEY,
    [Version] [int] NOT NULL,
	[MessageType] [nvarchar](200) NOT NULL,
	[MessageData] [nvarchar](200) NOT NULL,
	[CreateTime] [datetime2](7) NOT NULL,
	[UtcTime] [datetime2](7) NOT NULL
	)
END;";
            using (var connection = new SqlConnection(_options.Value.ConnectionString))
                connection.ExecuteNonQuery(sql);

            _logger.LogInformation("create message table successfully");
            await Task.CompletedTask;
        }

        /// <summary>
        /// 新增消息记录
        /// </summary>
        /// <param name="message"></param>
        /// <param name="dbTransaction"></param>
        public void StoreMessage(MediumMessage message, object dbTransaction = null)
        {
            object[] sqlParams =
            {
                new SqlParameter("@Id", message.Id),
                new SqlParameter("@Version", message.Version),
                new SqlParameter("@MessageType", message.MessageType),
                new SqlParameter("@MessageData", message.MessageData),
                new SqlParameter("@CreateTime", message.CreateTime),
                new SqlParameter("@UtcTime", message.UtcTime)
            };

            var sql = @"INSERT INTO EventBusMessages ([ID],[Version],[MessageType],[MessageData],[CreateTime],[UtcTime]) 
VALUES (@id,@Version,@MessageType,@MessageData,@CreateTime,@UtcTime);";

            if (dbTransaction == null)
            {
                using var connection = new SqlConnection(_options.Value.ConnectionString);
                connection.ExecuteNonQuery(sql, sqlParams: sqlParams);
            }
            else
            {
                IDbTransaction dbTrans = null;
                switch (dbTransaction)
                {
                    case IDbTransaction dbTran:
                        dbTrans = dbTran;
                        break;
                    case IDbContextTransaction dbContextTransaction:
                        dbTrans = dbContextTransaction.GetDbTransaction();
                        break;
                }
                var conn = dbTrans?.Connection;
                conn?.ExecuteNonQuery(sql, dbTrans, sqlParams);

            }
            _logger.LogInformation("create message table successfully");
        }
    }
}
