using System;
using System.Collections.Generic;
using Autofac;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using Skrbot.WebApi.BotHelpers.Authentications;

namespace Skrbot.WebApi.BotHelpers.DataStores
{
    [Serializable]
    public class BotDataStoreModule : Module
    {
        #region 共用屬性

        protected enum BotStoreType { InMemory, MsSql, SQLite, BotConnector }

        protected IBotDataStore<BotData> BotDataStore { get; set; }
        protected BotStoreType Type { get; set; }

        #endregion


        #region Bot Connector DataStore專用屬性

        protected string ServiceUrl { get; set; }
        protected Dictionary<string, string> Headers { get; set; }

        #endregion


        #region Override Method

        // Load Module
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            if (Type == BotStoreType.BotConnector)
            {   // Use Custom Bot Connector
                builder.Register(c => new CachingBotDataStore(new ConnectorDataStore(c.Resolve<IActivity>(),
                                                                                     ServiceUrl,
                                                                                     Headers),
                                    CachingBotDataStoreConsistencyPolicy.LastWriteWins))
                       .As<IBotDataStore<BotData>>()
                       .AsSelf()
                       .InstancePerLifetimeScope();
            }
            else
            {
                if (BotDataStore != null)
                {
                    builder.Register(c => new CachingBotDataStore(BotDataStore,
                                        CachingBotDataStoreConsistencyPolicy.LastWriteWins))
                           .As<IBotDataStore<BotData>>()
                           .AsSelf()
                           .InstancePerLifetimeScope();
                }
            }
        }

        #endregion


        #region 設定Bot State儲存的位置

        // 將Bot State儲存在In Memory (Bot Application)
        public BotDataStoreModule UseInMemoryDataStore()
        {
            BotDataStore = new InMemoryDataStore();
            Type = BotStoreType.InMemory;
            return this;
        }

        // 將Bot State儲存到Microsoft SQL Server
        public BotDataStoreModule UseSqlDataStore(string connectionString)
        {
            BotDataStore = new SqlDataStore(connectionString);
            Type = BotStoreType.MsSql;
            return this;
        }

        // 將Bot State儲存到SQLite
        public BotDataStoreModule UseSQLiteDataStore(string connectionString)
        {
            BotDataStore = new SQLiteDataStore(connectionString);
            Type = BotStoreType.SQLite;
            return this;
        }

        // 將Bot State儲存到Bot Connector
        public BotDataStoreModule UseBotConnectorDataStore(string serviceUrl = null, Dictionary<string, string> headers = null)
        {
            ServiceUrl = serviceUrl;
            Headers = headers ?? new Dictionary<string, string>();
            Type = BotStoreType.BotConnector;
            return this;
        }

        // 將Bot State儲存到Gss Bot Connector
        public BotDataStoreModule UseGssBotConnectorDataStore(string serviceUrl = null, Dictionary<string, string> headers = null)
        {
            // Add GSS Bot Authentication Headers
            var newHeaders = headers ?? new Dictionary<string, string>();
            newHeaders.Add(GssBotAuthenticationService.GssBotAuthenticationHeaderKey, GssBotAuthenticationService.GssBotAppAccessToken);
            return UseBotConnectorDataStore(serviceUrl, newHeaders);
        }

        #endregion
    }
}