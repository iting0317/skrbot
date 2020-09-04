using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Skrbot.WebApi.BotHelpers.DataStores
{
    [Serializable]
    public class ConnectorDataStoreClient
    {
        public string ServiceUrl { get; set; } = string.Empty;
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();


        public ConnectorDataStoreClient(string serviceUrl, Dictionary<string, string> headers = null)
        {
            ServiceUrl = serviceUrl;
            Headers = headers ?? new Dictionary<string, string>();
        }


        // Get User Data
        public virtual async Task<BotData> GetUserDataAsync(string channelId, string userId, CancellationToken cancellationToken)
        {
            var userDataUrl = new Uri($"{ServiceUrl}/v3/botstate/{channelId}/users/{userId}");
            var botData = await HttpGetAsync(userDataUrl.ToString(), Headers);
            return botData;
        }
        // Get Conversation Data
        public virtual async Task<BotData> GetConversationDataAsync(string channelId, string convId, CancellationToken cancellationToken)
        {
            var userDataUrl = new Uri($"{ServiceUrl}/v3/botstate/{channelId}/conversations/{convId}");
            var botData = await HttpGetAsync(userDataUrl.ToString(), Headers);
            return botData;
        }
        // Get Private Conversation Data
        public virtual async Task<BotData> GetPrivateConversationDataAsync(string channelId, string convId, string userId, CancellationToken cancellationToken)
        {
            var userDataUrl = new Uri($"{ServiceUrl}/v3/botstate/{channelId}/conversations/{convId}/users/{userId}");
            var botData = await HttpGetAsync(userDataUrl.ToString(), Headers);
            return botData;
        }


        // Set User Data
        public virtual async Task<BotData> SetUserDataAsync(string channelId, string userId, BotData data, CancellationToken cancellationToken)
        {
            var userDataUrl = new Uri($"{ServiceUrl}/v3/botstate/{channelId}/users/{userId}");
            var botData = await HttpPostAsync(userDataUrl.ToString(), data, Headers);
            return botData;
        }
        // Set Conversation Data
        public virtual async Task<BotData> SetConversationDataAsync(string channelId, string convId, BotData data, CancellationToken cancellationToken)
        {
            var userDataUrl = new Uri($"{ServiceUrl}/v3/botstate/{channelId}/conversations/{convId}");
            var botData = await HttpPostAsync(userDataUrl.ToString(), data, Headers);
            return botData;
        }
        // Set Private Conversation Data
        public virtual async Task<BotData> SetPrivateConversationDataAsync(string channelId, string convId, string userId, BotData data, CancellationToken cancellationToken)
        {
            var userDataUrl = new Uri($"{ServiceUrl}/v3/botstate/{channelId}/conversations/{convId}/users/{userId}");
            var botData = await HttpPostAsync(userDataUrl.ToString(), data, Headers);
            return botData;
        }
        // Delete State For User Data
        public virtual async Task DeleteStateForUserAsync(string channelId, string userId, CancellationToken cancellationToken)
        {
            var userDataUrl = new Uri($"{ServiceUrl}/v3/botstate/{channelId}/users/{userId}");
            await HttpDeleteAsync(userDataUrl.ToString(), Headers);
        }


        // Http Get
        protected async Task<BotData> HttpGetAsync(string url, Dictionary<string, string> headers)
        {
            var client = new HttpClient();

            // Add Header
            client.DefaultRequestHeaders.Clear();
            var headerKeys = new List<string>(headers.Keys);
            foreach (var key in headerKeys)
            {
                var value = headers[key];
                if (CheckHttpHeader(key, value))
                {
                    client.DefaultRequestHeaders.Add(key, new List<string>() { value });
                }
            }

            // Http GET
            var response = await client.GetAsync(url);

            // Get BotData
            var botdata = await GetResponseContent<BotData>(response);
            return botdata;
        }
        // Http Post (Json)
        protected async Task<BotData> HttpPostAsync<T>(string url, T data, Dictionary<string, string> headers)
        {
            var client = new HttpClient();

            // Add Header
            client.DefaultRequestHeaders.Clear();
            var headerKeys = new List<string>(headers.Keys);
            foreach (var key in headerKeys)
            {
                var value = headers[key];

                if (CheckHttpHeader(key, value))
                {
                    client.DefaultRequestHeaders.Add(key, value);
                }
            }

            // Http Post
            var response = await client.PostAsJsonAsync(url, data);

            var botdata = await GetResponseContent<BotData>(response);
            return botdata;
        }
        // Http Delete
        protected async Task HttpDeleteAsync(string url, Dictionary<string, string> headers)
        {
            var client = new HttpClient();

            // Add Header
            client.DefaultRequestHeaders.Clear();
            var headerKeys = new List<string>(headers.Keys);
            foreach (var key in headerKeys)
            {
                var value = headers[key];
                if (CheckHttpHeader(key, value))
                {
                    client.DefaultRequestHeaders.Add(key, value);
                }
            }

            // Http Delete
            var response = await client.DeleteAsync(url);
        }

        // Check Header
        protected bool CheckHttpHeader(string authKey, string authValue)
        {
            if (string.IsNullOrEmpty(authKey) || string.IsNullOrEmpty(authValue))
            {
                return false;
            }
            return true;
        }
        // Get Response Content (Response to Model)
        protected async Task<T> GetResponseContent<T>(HttpResponseMessage response)
        {
            // Read Response Content
            var content = await response.Content.ReadAsStringAsync();
            content = WebUtility.HtmlDecode(content);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(content);
            }
            return default(T);
        }
    }
}