using Dapper;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using System;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skrbot.WebApi.BotHelpers.DataStores
{
    [Serializable]
    public class SQLiteDataStoreService
    {
        public string ConnectionString { get; set; }

        public SQLiteDataStoreService()
        {

        }
        public SQLiteDataStoreService(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public virtual BotDataStoreDao Select(IAddress data, BotStoreType type)
        {
            BotDataStoreDao botState = null;

            using (var cn = new SQLiteConnection(ConnectionString))
            {
                cn.Open();

                var sql = new StringBuilder(@"SELECT * FROM BotState 
                                                WHERE Type = @Type ");
                var sqlParams = new DynamicParameters();
                sqlParams.Add("Type", (int)type);

                switch (type)
                {
                    case BotStoreType.BotUserData:
                        sql.Append(@" AND ChannelId = @ChannelId 
                                      AND UserId = @UserId");
                        sqlParams.Add("ChannelId", data.ChannelId);
                        sqlParams.Add("UserId", data.UserId);
                        break;
                    case BotStoreType.BotConversationData:
                        sql.Append(@" AND ChannelId = @ChannelId 
                                      AND ConversationId = @ConversationId;");
                        sqlParams.Add("ChannelId", data.ChannelId);
                        sqlParams.Add("ConversationId", data.ConversationId);
                        break;
                    case BotStoreType.BotPrivateConversationData:
                        sql.Append(@" AND ChannelId = @ChannelId 
                                      AND ConversationId = @ConversationId 
                                      AND UserId = @UserId;");
                        sqlParams.Add("ChannelId", data.ChannelId);
                        sqlParams.Add("UserId", data.UserId);
                        sqlParams.Add("ConversationId", data.ConversationId);
                        break;
                    default:
                        throw new ArgumentException("Unsupported bot store type!");
                }

                botState = cn.Query<BotDataStoreDao>(sql.ToString(), sqlParams).FirstOrDefault();

                cn.Close();
            }

            return botState;
        }

        public virtual async Task<bool> InsertAsync(BotDataStoreDao data)
        {
            int count = 0;

            using (var cn = new SQLiteConnection(ConnectionString))
            {
                cn.Open();

                var sql = @"INSERT INTO BotState
	                            (Type, BotId, ConversationId, ChannelId, UserId, Data, ETag, ServiceUrl, Timestamp)
	                            VALUES
	                            (@Type, @BotId, @ConversationId, @ChannelId, @UserId, @Data, @ETag, @ServiceUrl, @TimeStamp)";
                count = await cn.ExecuteAsync(sql, data);

                cn.Close();
            }

            return (count != 0);
        }

        public virtual async Task<bool> UpdateAsync(BotDataStoreDao data)
        {
            int count = 0;

            using (var cn = new SQLiteConnection(ConnectionString))
            {
                cn.Open();

                var sql = new StringBuilder(@"UPDATE BotState
	                                            SET Data = @Data,
		                                            ETag = @ETag,
		                                            Timestamp = @TimeStamp
                                                WHERE Type = @Type");
                var sqlParams = new DynamicParameters();
                sqlParams.Add("Data", data.Data);
                sqlParams.Add("ETag", data.ETag);
                sqlParams.Add("TimeStamp", data.TimeStamp);
                sqlParams.Add("Type", (int)data.Type);

                switch (data.Type)
                {
                    case BotStoreType.BotUserData:
                        sql.Append(@" AND ChannelId = @ChannelId 
                                      AND UserId = @UserId");
                        sqlParams.Add("ChannelId", data.ChannelId);
                        sqlParams.Add("UserId", data.UserId);
                        break;
                    case BotStoreType.BotConversationData:
                        sql.Append(@" AND ChannelId = @ChannelId 
                                      AND ConversationId = @ConversationId;");
                        sqlParams.Add("ChannelId", data.ChannelId);
                        sqlParams.Add("ConversationId", data.ConversationId);
                        break;
                    case BotStoreType.BotPrivateConversationData:
                        sql.Append(@" AND ChannelId = @ChannelId 
                                      AND ConversationId = @ConversationId 
                                      AND UserId = @UserId;");
                        sqlParams.Add("ChannelId", data.ChannelId);
                        sqlParams.Add("UserId", data.UserId);
                        sqlParams.Add("ConversationId", data.ConversationId);
                        break;
                    default:
                        throw new ArgumentException("Unsupported bot store type!");
                }

                count = await cn.ExecuteAsync(sql.ToString(), sqlParams);

                cn.Close();
            }

            return (count != 0);
        }

        public virtual async Task<bool> InsertOrUpdateAsync(BotDataStoreDao data)
        {
            var result = Select(data, data.Type);

            if (result == null)
            {
                return await InsertAsync(data);
            }
            else
            {
                return await UpdateAsync(data);
            }
        }

        public virtual async Task<bool> DeleteStateForUserAsync(IAddress key)
        {
            return await DeleteStateForUserAsync(key.ChannelId, key.UserId);
        }
        public async Task<bool> DeleteStateForUserAsync(string channelId, string userId)
        {
            int count = 0;

            using (var cn = new SQLiteConnection(ConnectionString))
            {
                cn.Open();

                var sql = new StringBuilder(@"DELETE FROM BotState
                                                WHERE ChannelId = @ChannelId
                                                AND UserId = @UserId");
                var sqlParams = new DynamicParameters();
                sqlParams.Add("ChannelId", channelId);
                sqlParams.Add("UserId", userId);

                count = await cn.ExecuteAsync(sql.ToString(), sqlParams);

                cn.Close();
            }

            return (count != 0);
        }
    }
}