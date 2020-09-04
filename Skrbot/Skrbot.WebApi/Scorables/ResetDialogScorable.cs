using Autofac;
using Skrbot.WebApi.BotHelpers.Extends;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Scorables.Internals;
using Microsoft.Bot.Connector;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Skrbot.WebApi.Scorables
{
    // 註冊全域訊息 - 重置對話
    [Serializable]
    public class ResetDialogScorable : ScorableBase<IActivity, string, double>
    {
        protected readonly IBotToUser _botToUser;
        protected readonly IBotData _botData;
        protected readonly IDialogTask _task;
        protected readonly IActivity _activity;
        protected readonly IDialogContext _context;

        public const string Pattern = "reset";

        public ResetDialogScorable(IComponentContext component)
        {
            var userToBot = component.Resolve<IBotToUser>();
            var botData = component.Resolve<IBotData>();
            var task = component.Resolve<IDialogTask>();
            var activity = component.Resolve<IActivity>();
            var context = new DialogContext(userToBot, botData, task, activity, CancellationToken.None);

            SetField.NotNull(out _botToUser, nameof(userToBot), userToBot);
            SetField.NotNull(out _botData, nameof(botData), botData);
            SetField.NotNull(out _task, nameof(task), task);
            SetField.NotNull(out _activity, nameof(activity), activity);
            SetField.NotNull(out _context, nameof(context), context);
        }

        /**
         * 過濾進來的訊息
         * 如果進來的訊息為特定關鍵字，則攔截進來的訊息；反之，則讓進來的訊息通過 (即回傳null)
         **/
        protected override async Task<string> PrepareAsync(IActivity item, CancellationToken token)
        {
            var message = item as IMessageActivity;

            if (message != null && !string.IsNullOrWhiteSpace(message.Text))
            {
                if (message.Text.Equals(Pattern, StringComparison.InvariantCultureIgnoreCase))
                {
                    return message.Text;
                }
            }

            return null;
        }

        // 當攔截到進來的訊息時，進入到這個函式
        protected override async Task PostAsync(IActivity item, string state, CancellationToken token)
        {
            await _context.ReplyMessageAsync("Reset Dialog", waitTime: 1000, isSendTyping: true);
            _task.Reset();
        }

        // 有無優先權
        protected override bool HasScore(IActivity item, string state)
        {
            return state != null;
        }

        // 訊息的優先權 (範圍：1.0 ~ 0.0)
        protected override double GetScore(IActivity item, string state)
        {
            return 1.0;
        }

        // 執行結束
        protected override Task DoneAsync(IActivity item, string state, CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }
}