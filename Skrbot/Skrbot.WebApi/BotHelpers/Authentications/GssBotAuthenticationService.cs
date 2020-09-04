using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http.Controllers;

namespace Skrbot.WebApi.BotHelpers.Authentications
{
    [Serializable]
    public class GssBotAuthenticationService
    {
        public const string GssBotAuthenticationHeaderKey = "x-gss-bot-authentication";
        public static string GssBotAppAccessToken
        {
            get
            {
                return ConfigurationManager.AppSettings["GssBotAppAccessToken"] ?? string.Empty;
            }
        }

        public bool VaildAuthorizedWithAccessToken(HttpActionContext actionContext)
        {
            // 如果GssBotAppAccessToken的值沒設定就By Pass
            if (string.IsNullOrEmpty(GssBotAppAccessToken))
            {
                return true;
            }

            // 取得Request Header (GSS Bot Signature)
            IEnumerable<string> accessTokens = new List<string>();
            actionContext.Request.Headers.TryGetValues(GssBotAuthenticationHeaderKey, out accessTokens);

            if (accessTokens != null && accessTokens.Any() && !string.IsNullOrEmpty(accessTokens.FirstOrDefault()))
            {
                var accessToken = accessTokens.FirstOrDefault();
                return (string.Compare(accessToken, GssBotAppAccessToken, true) == 0);
            }

            return false;
        }


        #region 另一個加密驗證方式 (未使用)

        // 取得Activity重要內容加密後的內容
        public string GetActivityContentSecret(Activity activity, string key)
        {
            if (activity != null)
            {
                var content = GetActivityContent(activity);

                if (!string.IsNullOrEmpty(content))
                {
                    var md5 = new HMACSHA256(Encoding.UTF8.GetBytes(key));
                    byte[] bytes = Encoding.UTF8.GetBytes(content);
                    var crypto = md5.ComputeHash(bytes);
                    var contentSecret = Convert.ToBase64String(crypto);

                    return contentSecret;
                }
            }
            return string.Empty;
        }

        // 取得Activity重要內容
        protected string GetActivityContent(Activity activity)
        {
            if (activity != null)
            {
                var content = JsonConvert.SerializeObject(new
                {
                    type = activity.Type ?? string.Empty,
                    channelId = activity.ChannelId ?? string.Empty,
                    conversationId = activity.Conversation.Id ?? string.Empty,
                    recipientId = activity.Recipient.Id ?? string.Empty,
                    fromId = activity.From.Id ?? string.Empty,
                    text = activity.Text ?? string.Empty,
                    attachments = activity.Attachments ?? new List<Attachment>(),
                    value = activity.Value ?? new JObject(),
                    serviceUrl = activity.ServiceUrl ?? string.Empty
                });
                return content;
            }

            return string.Empty;
        }

        // 取得Activity
        protected IList<Activity> GetActivities(HttpActionContext actionContext)
        {
            var activties = actionContext.ActionArguments.Select(t => t.Value).OfType<Activity>().ToList();
            if (activties.Any())
            {
                return activties;
            }
            else
            {
                var objects =
                    actionContext.ActionArguments.Where(t => t.Value is JObject || t.Value is JArray).Select(t => t.Value).ToArray();
                if (objects.Any())
                {
                    activties = new List<Activity>();
                    foreach (var obj in objects)
                    {
                        activties.AddRange((obj is JObject) ? new Activity[] { ((JObject)obj).ToObject<Activity>() } : ((JArray)obj).ToObject<Activity[]>());
                    }
                }
            }
            return activties;
        }

        #endregion
    }
}