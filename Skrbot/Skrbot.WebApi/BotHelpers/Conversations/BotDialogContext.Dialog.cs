using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Skrbot.WebApi.BotHelpers.Conversations
{
    /// <summary>
    /// 處理Dialog Context的操作
    /// </summary>
    public partial class BotDialogContext
    {
        public delegate Task DialogScopeAsyncDelegate(BotDialogContextArgs dialogScopeArgs);
        public delegate void DialogScopeDelegate(BotDialogContextArgs dialogScopeArgs);

        // Wait
        public async Task Wait<T>(ResumeAfter<T> resumeAfter)
        {
            await StartAsync((context) =>
            {
                context.Task.Wait(resumeAfter);
            }, CancellationToken.None);
        }

        // Call Dialog
        public async Task Call(IDialog<object> dialog, ResumeAfter<object> resumeAfter = null)
        {
            await StartAsync(async (context) =>
            {
                var interruption = dialog.Void<object, IMessageActivity>();
                context.Task.Call(interruption, resumeAfter);
                await context.Task.PollAsync(CancellationToken.None);
            }, CancellationToken.None);
        }

        // Forward Dialog
        public async Task Forward(IDialog<object> dialog, ResumeAfter<object> resumeAfter = null)
        {
            await StartAsync(async (context) =>
            {
                var interruption = dialog.Void<object, IMessageActivity>();
                await context.Task.Forward(interruption, resumeAfter, Activity, CancellationToken.None);
                await context.Task.PollAsync(CancellationToken.None);
            }, CancellationToken.None);
        }

        // Done Dialog
        public async Task Done<T>(T data)
        {
            await StartAsync(async (context) =>
            {
                if (context.Task.Frames.Count == 0)
                {
                    throw new Exception("DialogStack is empty");
                }

                context.Task.Done<object>(null);
                await context.Task.PollAsync(CancellationToken.None);
            }, CancellationToken.None);
        }

        // Reset Dialog Stack
        public async Task Reset()
        {
            await StartAsync((context) =>
            {
                context.Task.Reset();
            }, CancellationToken.None);
        }

        // Fail
        public async Task Fail(Exception e)
        {
            await StartAsync((context) =>
            {
                context.Task.Fail(e);
            }, CancellationToken.None);
        }


        #region Common Method

        protected async Task StartAsync(DialogScopeAsyncDelegate callback, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, Activity))
            {
                var botData = scope.Resolve<IBotData>();
                await botData.LoadAsync(cancellationToken);

                var task = scope.Resolve<IDialogTask>();
                var botToUser = scope.Resolve<IBotToUser>();

                if (callback != null)
                {
                    await callback.Invoke(new BotDialogContextArgs(Activity, task, botData, botToUser, scope));
                }

                await botData.FlushAsync(cancellationToken);
            }
        }
        protected async Task StartAsync(DialogScopeDelegate callback, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, Activity))
            {
                var botData = scope.Resolve<IBotData>();
                await botData.LoadAsync(cancellationToken);

                var task = scope.Resolve<IDialogTask>();
                var botToUser = scope.Resolve<IBotToUser>();

                if (callback != null)
                {
                    callback.Invoke(new BotDialogContextArgs(Activity, task, botData, botToUser, scope));
                }

                await botData.FlushAsync(cancellationToken);
            }
        }


        #endregion
    }

    // EventArgs
    [Serializable]
    public class BotDialogContextArgs
    {
        public IActivity Activity { get; }
        public IDialogTask Task { get; }
        public IBotData BotData { get; }
        public ILifetimeScope DialogScope { get; }
        public IBotToUser BotToUser { get; }

        public BotDialogContextArgs(IActivity activity, IDialogTask task, IBotData data, IBotToUser botToUser, ILifetimeScope scope)
        {
            Activity = activity;
            Task = task;
            BotData = data;
            DialogScope = scope;
            BotToUser = botToUser;
        }
    }
}