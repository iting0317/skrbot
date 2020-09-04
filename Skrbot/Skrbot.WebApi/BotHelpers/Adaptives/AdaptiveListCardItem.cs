using Microsoft.Bot.Connector;
using System;

namespace Skrbot.WebApi.BotHelpers.Adaptives
{
    [Serializable]
    public class AdaptiveListCardItem
    {
        public CardImage Image { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Text { get; set; }
        public CardAction Tap { get; set; }

        public AdaptiveListCardItem()
        {

        }
        public AdaptiveListCardItem(string title = null, string subtitle = null, string text = null, CardImage images = null, CardAction tap = null)
        {
            Title = title;
            Subtitle = subtitle;
            Text = text;
            Image = images;
            Tap = tap;
        }
    }
}