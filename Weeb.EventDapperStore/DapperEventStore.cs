using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Weeb.Event;
using Dapper;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace Weeb.EventDapperStore
{
    public class DapperEventStore : IEventStore
    {
        private readonly string _connectionString;
        private readonly ILogger<DapperEventStore> _log;

        public DapperEventStore(string connectionString,ILogger<DapperEventStore> log)
        {
            _connectionString = connectionString;
            _log = log;
            _log.LogInformation($"DapperEventStore构造函数调用完成。Hash Code：{this.GetHashCode()}.");
        }

        public async Task SaveEventAsync<TEvent>(TEvent @event) where TEvent : IEvent
        {
            const string sql = @"insert into `Events` (EventId,EventPayLoad,EventTimeStamp)values(@EventId,@EventPayLoad,@EventTimeStamp)";
            try
            {
                using (var connection = new MySql.Data.MySqlClient.MySqlConnection(_connectionString))
                {
                    await connection.ExecuteAsync(sql, new
                    {
                        EventId = @event.Id,
                        EventTimeStamp = @event.TimeStamp,
                        EventPayLoad = JsonConvert.SerializeObject(@event)
                    });
                }
            }
            catch(Exception ex)
            {
                _log.LogError(ex, $"SaveEventAsync sql:{sql}");
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    _log.LogInformation($"DapperEventStore 已经被Dispose. Hash COde:{GetHashCode()}. {DateTime.Now.Millisecond}");
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~DapperEventStore() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion


    }
}
