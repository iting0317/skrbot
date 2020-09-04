using AdaptiveCards;
using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Skrbot.WebApi.BotHelpers.Adaptives
{
    [Serializable]
    public static class AdaptiveCardExtend
    {
        // HeroCard to AdaptiveCard
        public static AdaptiveCard ToAdaptiveCard(this HeroCard card, string version = "1.0")
        {
            return AdaptiveCardBuilder.CreateHeroCard(card, version);
        }
        // ThumbnailCard to ThumbnailCard
        public static AdaptiveCard ToAdaptiveCard(this ThumbnailCard card, string version = "1.0")
        {
            return AdaptiveCardBuilder.CreateThumbnailCard(card, version);
        }
        // SigninCard to SigninCard
        public static AdaptiveCard ToAdaptiveCard(this SigninCard card, string version = "1.0")
        {
            return AdaptiveCardBuilder.CreateSigninCard(card, version);
        }
        // ReceiptCard to SigninCard
        public static AdaptiveCard ToAdaptiveCard(this ReceiptCard card, string totalTitle = "Total", string vatTitle = "VAT", string taxTitle = "Tax", string version = "1.0")
        {
            return AdaptiveCardBuilder.CreateReceiptCard(card, totalTitle, vatTitle, taxTitle, version);
        }
        // AnimationCard to SigninCard
        public static AdaptiveCard ToAdaptiveCard(this AnimationCard card, string version = "1.0")
        {
            return AdaptiveCardBuilder.CreateAnimationCard(card, version);
        }

        // AdaptiveCard to Attachment
        public static Attachment ToAttachment(this AdaptiveCard card)
        {
            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
        }

        // 修改Adaptive Card的Select Action
        public static AdaptiveCard SetSelectAction(this AdaptiveCard card, string id, CardAction tap)
        {
            if (!string.IsNullOrEmpty(id) && tap != null)
            {
                var crawler = new AdaptiveCardCrawler(card);
                crawler.SetSelectActionById(id, tap);
            }
            return card;
        }

        // 修改Adaptive Card的樣式
        public static AdaptiveCard SetElementProperty(this AdaptiveCard card, string id, string propertyName, object value)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var crawler = new AdaptiveCardCrawler(card);
                crawler.SetElementPropertyValueById(id, propertyName, value);
            }
            return card;
        }


        #region Bind Adaptive Data

        // 綁定Adaptive Card的資料
        public static AdaptiveCard BindData<T>(this AdaptiveCard card, string id, T data)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var crawler = new AdaptiveCardCrawler(card);
                card = crawler.BindDataById(id, data);
            }
            return card;
        }
        // 綁定Adaptive Card的資料
        public static AdaptiveCard BindData(this AdaptiveCard card, Dictionary<string, object> data)
        {
            if (data != null)
            {
                var keys = data.Keys;
                foreach (var key in keys)
                {
                    var crawler = new AdaptiveCardCrawler(card);
                    card = crawler.BindDataById(key, data[key]);
                }
            }
            return card;
        }
        // 綁定Adaptive Card的資料
        public static AdaptiveCard BindData(this AdaptiveCard card, JObject data)
        {
            if (data == null)
            {
                return card;
            }

            return card.BindData(data.ToObject<Dictionary<string, object>>());
        }
        // 綁定Adaptive Card的資料
        public static AdaptiveCard BindData<T>(this AdaptiveCard card, T data)
        {
            if (data == null)
            {
                return card;
            }
            var dictionaryData = data.GetType()
                                     .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                     .ToDictionary(prop => prop.Name, prop => prop.GetValue(data, null));
            return card.BindData(dictionaryData);
        }

        #endregion
    }
}