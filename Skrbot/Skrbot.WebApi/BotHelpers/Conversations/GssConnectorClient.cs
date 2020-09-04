using Skrbot.WebApi.BotHelpers.Authentications;
using Microsoft.Bot.Connector;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Skrbot.WebApi.BotHelpers.Conversations
{
    [Serializable]
    public class GssConnectorClient : ConnectorClient
    {
        protected GssConnectorClient(params DelegatingHandler[] handlers) : base(handlers)
        {
            AddAuthenticationHeaders();
        }
        public GssConnectorClient(ServiceClientCredentials credentials, params DelegatingHandler[] handlers) : base(credentials, handlers)
        {
            AddAuthenticationHeaders();
        }
        public GssConnectorClient(HttpClientHandler rootHandler, params DelegatingHandler[] handlers) : base(rootHandler, handlers)
        {
            AddAuthenticationHeaders();
        }

        public GssConnectorClient(Uri baseUri, string microsoftAppId = null, string microsoftAppPassword = null, params DelegatingHandler[] handlers) : base(baseUri, microsoftAppId, microsoftAppPassword, handlers)
        {
            AddAuthenticationHeaders();
        }

        public GssConnectorClient(Uri baseUri, MicrosoftAppCredentials credentials, bool addJwtTokenRefresher = true, params DelegatingHandler[] handlers) : base(baseUri, credentials, addJwtTokenRefresher, handlers)
        {
            AddAuthenticationHeaders();
        }

        public GssConnectorClient(Uri baseUri, MicrosoftAppCredentials credentials, HttpClientHandler httpClientHandler, bool addJwtTokenRefresher = true, params DelegatingHandler[] handlers) : base(baseUri, credentials, httpClientHandler, addJwtTokenRefresher, handlers)
        {
            AddAuthenticationHeaders();
        }

        public GssConnectorClient(Uri uri, MicrosoftAppCredentials microsoftAppCredentials) : base(uri, microsoftAppCredentials)
        {
            AddAuthenticationHeaders();
        }

        public GssConnectorClient(Uri uri) : base(uri)
        {
            AddAuthenticationHeaders();
        }

        public GssConnectorClient() : base()
        {
            AddAuthenticationHeaders();
        }

        // Add Bot Authentication Headers
        protected void AddAuthenticationHeaders()
        {
            var authKey = GssBotAuthenticationService.GssBotAuthenticationHeaderKey;
            var accessToken = GssBotAuthenticationService.GssBotAppAccessToken ?? string.Empty;

            HttpClient.DefaultRequestHeaders.TryAddWithoutValidation(authKey, new List<string>() { accessToken });
        }
    }
}