using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Skrbot.WebApi.BotHelpers.Conversations
{
    /// <summary>
    /// 處理Bot State的操作
    /// 不建議在Dialog中使用，因為Bot State會被蓋掉
    /// </summary>
    public partial class BotDialogContext
    {
        #region Common Method (Private)

        protected async Task<T> GetBotStateAsync<T>(BotStoreType type, string key, T defaultValue = default(T), CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, Activity))
            {
                // Get Bot Data
                var botData = scope.Resolve<IBotData>();

                // Load Bot Data
                await botData.LoadAsync(cancellationToken);

                switch (type)
                {
                    case BotStoreType.BotUserData:
                        return botData.UserData.GetValueOrDefault(key, defaultValue);
                    case BotStoreType.BotConversationData:
                        return botData.ConversationData.GetValueOrDefault(key, defaultValue);
                    case BotStoreType.BotPrivateConversationData:
                        return botData.PrivateConversationData.GetValueOrDefault(key, defaultValue);
                    default:
                        throw new ArgumentException($"{type} is not a valid store type!");
                }
            }
        }
        protected async Task SetBotStateAsync<T>(BotStoreType type, string key, T value, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, Activity))
            {
                // Get Bot Data
                var botData = scope.Resolve<IBotData>();

                // Load Bot Data
                await botData.LoadAsync(cancellationToken);

                switch (type)
                {
                    case BotStoreType.BotUserData:
                        botData.UserData.SetValue(key, value);
                        break;
                    case BotStoreType.BotConversationData:
                        botData.ConversationData.SetValue(key, value);
                        break;
                    case BotStoreType.BotPrivateConversationData:
                        botData.PrivateConversationData.SetValue(key, value);
                        break;
                    default:
                        throw new ArgumentException($"{type} is not a valid store type!");
                }

                // Save Bot Data
                await botData.FlushAsync(cancellationToken);
            }
        }
        protected async Task RemoveBotStateAsync(BotStoreType type, string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, Activity))
            {
                // Get Bot Data
                var botData = scope.Resolve<IBotData>();

                // Load Bot Data
                await botData.LoadAsync(cancellationToken);

                switch (type)
                {
                    case BotStoreType.BotUserData:
                        botData.UserData.RemoveValue(key);
                        break;
                    case BotStoreType.BotConversationData:
                        botData.ConversationData.RemoveValue(key);
                        break;
                    case BotStoreType.BotPrivateConversationData:
                        botData.PrivateConversationData.RemoveValue(key);
                        break;
                    default:
                        throw new ArgumentException($"{type} is not a valid store type!");
                }

                // Save Bot Data
                await botData.FlushAsync(cancellationToken);
            }
        }
        protected async Task ClearBotStateAsync(BotStoreType type, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, Activity))
            {
                // Get Bot Data
                var botData = scope.Resolve<IBotData>();

                // Load Bot Data
                await botData.LoadAsync(cancellationToken);

                switch (type)
                {
                    case BotStoreType.BotUserData:
                        botData.UserData.Clear();
                        break;
                    case BotStoreType.BotConversationData:
                        botData.ConversationData.Clear();
                        break;
                    case BotStoreType.BotPrivateConversationData:
                        botData.PrivateConversationData.Clear();
                        break;
                    default:
                        throw new ArgumentException($"{type} is not a valid store type!");
                }

                // Save Bot Data
                await botData.FlushAsync(cancellationToken);
            }
        }

        #endregion


        #region Get Bot State

        public async Task<T> GetUserDataOrDefaultAsync<T>(string key, T defaultValue, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetBotStateAsync(BotStoreType.BotUserData, key, defaultValue, cancellationToken);
        }
        public async Task<T> GetConversationDataOrDefaultAsync<T>(string key, T defaultValue, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetBotStateAsync(BotStoreType.BotConversationData, key, defaultValue, cancellationToken);
        }
        public async Task<T> GetPrivateConversatDataOrDefaultAsync<T>(string key, T defaultValue, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetBotStateAsync(BotStoreType.BotPrivateConversationData, key, defaultValue, cancellationToken);
        }

        #endregion


        #region Set Bot State

        public async Task SetUserDataAsync<T>(string key, T value, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SetBotStateAsync(BotStoreType.BotUserData, key, value, cancellationToken);
        }
        public async Task SetConversationDataAsync<T>(string key, T value, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SetBotStateAsync(BotStoreType.BotConversationData, key, value, cancellationToken);
        }
        public async Task SetPrivateConversatDataAsync<T>(string key, T value, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SetBotStateAsync(BotStoreType.BotPrivateConversationData, key, value, cancellationToken);
        }


        #endregion


        #region Remove Bot State

        public async Task RemoveUserDataAsync(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RemoveBotStateAsync(BotStoreType.BotUserData, key, cancellationToken);
        }
        public async Task RemoveConversationDataAsync(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RemoveBotStateAsync(BotStoreType.BotConversationData, key, cancellationToken);
        }
        public async Task RemovePrivateConversatDataAsync(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RemoveBotStateAsync(BotStoreType.BotPrivateConversationData, key, cancellationToken);
        }

        #endregion


        #region Clear Bot State

        public async Task ClearUserDataAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await ClearBotStateAsync(BotStoreType.BotUserData, cancellationToken);
        }
        public async Task ClearConversationDataAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await ClearBotStateAsync(BotStoreType.BotConversationData, cancellationToken);
        }
        public async Task ClearPrivateConversatDataAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await ClearBotStateAsync(BotStoreType.BotPrivateConversationData, cancellationToken);
        }

        #endregion
    }
}