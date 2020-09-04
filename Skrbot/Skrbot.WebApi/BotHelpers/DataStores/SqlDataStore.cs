using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Skrbot.WebApi.BotHelpers.DataStores
{
    [Serializable]
    public class SqlDataStore : IBotDataStore<BotData>
    {
        private string ConnectionString { get; set; }

        public SqlDataStore(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public async Task<BotData> LoadAsync(IAddress key, BotStoreType botStoreType, CancellationToken cancellationToken)
        {
            var service = new SqlDataStoreService(ConnectionString);

            var data = service.Select(key, botStoreType);

            if (data == null)
            {
                return new BotData(eTag: string.Empty, data: null);
            }

            return data.BotData;
        }

        public async Task SaveAsync(IAddress key, BotStoreType botStoreType, BotData data, CancellationToken cancellationToken)
        {
            var model = new BotDataStoreDao(key, botStoreType, data)
            {
                TimeStamp = DateTime.Now
            };
            var service = new SqlDataStoreService(ConnectionString);
            await service.InsertOrUpdateAsync(model);
        }

        public Task<bool> FlushAsync(IAddress key, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}