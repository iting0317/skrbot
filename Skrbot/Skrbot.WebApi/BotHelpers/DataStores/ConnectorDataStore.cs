using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Skrbot.WebApi.BotHelpers.DataStores
{
    [Serializable]
    public class ConnectorDataStore : IBotDataStore<BotData>
    {
        private readonly IActivity Activity;
        private ConnectorDataStoreClient DataStoreClient;
        private string ServiceUrl;
        private Dictionary<string, string> Headers;


        public ConnectorDataStore(IActivity activity, string serviceUrl = null, Dictionary<string, string> headers = null)
        {
            SetField.NotNull(out Activity, nameof(activity), activity);

            ServiceUrl = (string.IsNullOrEmpty(serviceUrl)) ? activity.ServiceUrl : serviceUrl;
            Headers = headers ?? new Dictionary<string, string>();
            DataStoreClient = new ConnectorDataStoreClient(ServiceUrl, Headers);
        }

        public async Task<BotData> LoadAsync(IAddress key, BotStoreType botStoreType, CancellationToken cancellationToken)
        {
            var channelId = GetChannelId(key);
            BotData botData;
            switch (botStoreType)
            {
                case BotStoreType.BotConversationData:
                    botData = await DataStoreClient.GetConversationDataAsync(channelId, key.ConversationId, cancellationToken);
                    break;
                case BotStoreType.BotUserData:
                    botData = await DataStoreClient.GetUserDataAsync(channelId, key.UserId, cancellationToken);
                    break;
                case BotStoreType.BotPrivateConversationData:
                    botData = await DataStoreClient.GetPrivateConversationDataAsync(channelId, key.ConversationId, key.UserId, cancellationToken);
                    break;
                default:
                    throw new ArgumentException($"{botStoreType} is not a valid store type!");
            }
            return botData;
        }

        public async Task SaveAsync(IAddress key, BotStoreType botStoreType, BotData data, CancellationToken cancellationToken)
        {
            var channelId = GetChannelId(key);
            switch (botStoreType)
            {
                case BotStoreType.BotConversationData:
                    await DataStoreClient.SetConversationDataAsync(channelId, key.ConversationId, data, cancellationToken);
                    break;
                case BotStoreType.BotUserData:
                    await DataStoreClient.SetUserDataAsync(channelId, key.UserId, data, cancellationToken);
                    break;
                case BotStoreType.BotPrivateConversationData:
                    await DataStoreClient.SetPrivateConversationDataAsync(channelId, key.ConversationId, key.UserId, data, cancellationToken);
                    break;
                default:
                    throw new ArgumentException($"{botStoreType} is not a valid store type!");
            }
        }

        public Task<bool> FlushAsync(IAddress key, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }


        // Get Channel Id
        protected virtual string GetChannelId(IAddress key)
        {
            var channelData = JObject.FromObject(Activity.ChannelData);
            string channelId = channelData.SelectToken("channelId")?.ToString();
            if (string.IsNullOrEmpty(channelId))
            {
                channelId = key.ChannelId;
            }
            return channelId;
        }
    }
}