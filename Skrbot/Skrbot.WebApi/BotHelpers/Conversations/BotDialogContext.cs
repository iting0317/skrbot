using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;

namespace Skrbot.WebApi.BotHelpers.Conversations
{
    [Serializable]
    public partial class BotDialogContext
    {
        public Activity Activity { get; set; }

        public BotDialogContext(IActivity activity)
        {
            Activity = activity as Activity;
        }
        public BotDialogContext(IDialogContext context)
        {
            Activity = context.Activity as Activity;
        }

        public static BotDialogContext Create(IActivity activity)
        {
            return new BotDialogContext(activity);
        }
        public static BotDialogContext Create(IDialogContext context)
        {
            return new BotDialogContext(context.Activity);
        }
    }
}