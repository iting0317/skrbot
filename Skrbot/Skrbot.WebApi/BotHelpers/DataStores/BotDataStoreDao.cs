using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;

namespace Skrbot.WebApi.BotHelpers.DataStores
{
    [Serializable]
    public class BotDataStoreDao : IAddress
    {
        public int Id { get; set; }
        public BotStoreType Type { get; set; }
        public string BotId { get; set; }
        public string ChannelId { get; set; }
        public string ConversationId { get; set; }
        public string UserId { get; set; }
        public byte[] Data { get; set; }
        public string ETag { get; set; }
        public string ServiceUrl { get; set; }
        public DateTime TimeStamp { get; set; }
        public BotData BotData
        {
            get
            {
                return new BotData(ETag ?? string.Empty, Deserialize(Data));
            }
            set
            {
                if (value != null)
                {
                    Data = Serialize(value.Data);
                    ETag = value.ETag;
                }
            }
        }


        public BotDataStoreDao()
        {

        }
        public BotDataStoreDao(IAddress key, BotStoreType type, BotData botData)
        {
            Type = type;
            BotId = key.BotId;
            ChannelId = key.ChannelId;
            ConversationId = key.ConversationId;
            UserId = key.UserId;
            ServiceUrl = key.ServiceUrl;
            BotData = botData;
        }
        public BotDataStoreDao(BotStoreType type, string botId, string channelId, string conversationId, string userId, BotData botData)
        {
            Type = type;
            BotId = botId;
            ChannelId = channelId;
            ConversationId = conversationId;
            UserId = userId;
            BotData = botData;
        }


        private static byte[] Serialize(object data)
        {
            using (var cmpStream = new MemoryStream())
            using (var stream = new GZipStream(cmpStream, CompressionMode.Compress))
            using (var streamWriter = new StreamWriter(stream))
            {
                var settings = new JsonSerializerSettings()
                {
                    Formatting = Formatting.None,
                    NullValueHandling = NullValueHandling.Ignore
                };
                var serializedJSon = JsonConvert.SerializeObject(data, settings);
                streamWriter.Write(serializedJSon);
                streamWriter.Close();
                stream.Close();
                return cmpStream.ToArray();
            }
        }
        private static object Deserialize(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            using (var gz = new GZipStream(stream, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(gz))
            {
                return JsonConvert.DeserializeObject(streamReader.ReadToEnd());
            }
        }
    }
}