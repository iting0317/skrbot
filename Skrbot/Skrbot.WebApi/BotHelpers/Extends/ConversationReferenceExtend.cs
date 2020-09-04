using Microsoft.Bot.Connector;
using System;

namespace Skrbot.WebApi.BotHelpers.Extends
{
    [Serializable]
    public static class ConversationReferenceExtend
    {
        // Set Post Message
        public static ConversationReference SetPostMessage(this ConversationReference conversation, IMessageActivity activity)
        {
            conversation.ActivityId = activity.Id;
            conversation.User = activity.From;
            conversation.Bot = activity.Recipient;
            conversation.ChannelId = activity.ChannelId;
            conversation.Conversation = activity.Conversation;
            conversation.ServiceUrl = activity.ServiceUrl;
            return conversation;
        }
    }
}