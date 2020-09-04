using AdaptiveCards;
using Skrbot.WebApi.BotHelpers.Conversations;
using Skrbot.WebApi.BotHelpers.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Skrbot.WebApi.BotHelpers.Extends
{
    [Serializable]
    public static class DialogContextExtend
    {
        #region Reply Message (Sync)

        // 回覆訊息
        public static ResourceResponse ReplyMessage(this IDialogContext context, string message, List<CardAction> suggestedActions = null, int waitTime = 0, bool isSendTyping = false)
        {
            var botContext = BotDialogContext.Create(context).EnableShowTyping(isSendTyping, waitTime);
            return botContext.ReplyMessage(message: message, suggestedActions: suggestedActions);
        }
        public static ResourceResponse ReplyMessage(this IDialogContext context, IMessageActivity message, int waitTime = 0, bool isSendTyping = false)
        {
            var botContext = BotDialogContext.Create(context).EnableShowTyping(isSendTyping, waitTime);
            return botContext.ReplyMessage(message: message);
        }
        public static ResourceResponse ReplyMessage(this IDialogContext context, string message, string action, string actionName = null, int waitTime = 0, bool isSendTyping = false)
        {
            var botContext = BotDialogContext.Create(context).EnableShowTyping(isSendTyping, waitTime);
            return botContext.ReplyMessage(message: message, action: action, actionName: actionName);
        }

        // 回覆附件訊息
        public static ResourceResponse ReplyAttachment(this IDialogContext context, Attachment attachment, string text = null, List<CardAction> suggestedActions = null, int waitTime = 0, bool isSendTyping = false)
        {
            var botContext = BotDialogContext.Create(context).EnableShowTyping(isSendTyping, waitTime);
            return botContext.ReplyAttachment(attachment: attachment, text: text, suggestedActions: suggestedActions);
        }
        public static ResourceResponse ReplyAttachment(this IDialogContext context, IEnumerable<Attachment> attachments, string text = null, string layout = AttachmentLayoutTypes.Carousel, List<CardAction> suggestedActions = null, int waitTime = 0, bool isSendTyping = false)
        {
            var botContext = BotDialogContext.Create(context).EnableShowTyping(isSendTyping, waitTime);
            return botContext.ReplyAttachment(attachments: attachments, text: text, layout: layout, suggestedActions: suggestedActions);
        }
        public static ResourceResponse ReplyMedia(this IDialogContext context, string contentUrl, string contentType, string fileName, List<CardAction> suggestedActions = null, int waitTime = 0, bool isSendTyping = false)
        {
            var botContext = BotDialogContext.Create(context).EnableShowTyping(isSendTyping, waitTime);
            return botContext.ReplyMedia(contentUrl: contentUrl, contentType: contentType, fileName: fileName, suggestedActions: suggestedActions);
        }

        // 回覆語音
        public static ResourceResponse ReplySpeakAsync(this IDialogContext context, string speakText, string inputHint = null, int waitTime = 0, bool isSendTyping = false)
        {
            var botContext = BotDialogContext.Create(context).EnableShowTyping(isSendTyping, waitTime);
            return botContext.ReplySpeak(speakText: speakText, inputHint: inputHint);
        }

        // 回覆正在打字狀態
        public static ResourceResponse ReplyTyping(this IDialogContext context, int waitTime = 0, bool isSendTyping = false)
        {
            var botContext = BotDialogContext.Create(context).EnableShowTyping(isSendTyping, waitTime);
            return botContext.ReplyTyping();
        }

        #endregion


        #region Reply Message (Async)

        // 回覆訊息
        public static async Task<ResourceResponse> ReplyMessageAsync(this IDialogContext context, string message, List<CardAction> suggestedActions = null, int waitTime = 0, bool isSendTyping = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            var botContext = BotDialogContext.Create(context).EnableShowTyping(isSendTyping, waitTime);
            return await botContext.ReplyMessageAsync(message: message, suggestedActions: suggestedActions, cancellationToken: cancellationToken);
        }
        public static async Task<ResourceResponse> ReplyMessageAsync(this IDialogContext context, IMessageActivity message, int waitTime = 0, bool isSendTyping = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            var botContext = BotDialogContext.Create(context).EnableShowTyping(isSendTyping, waitTime);
            return await botContext.ReplyMessageAsync(message: message, cancellationToken: cancellationToken);
        }
        public static async Task<ResourceResponse> ReplyMessageAsync(this IDialogContext context, string message, string action, string actionName = null, int waitTime = 0, bool isSendTyping = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            var botContext = BotDialogContext.Create(context).EnableShowTyping(isSendTyping, waitTime);
            return await botContext.ReplyMessageAsync(message: message, action: action, actionName: actionName, cancellationToken: cancellationToken);
        }

        // 回覆附件訊息
        public static async Task<ResourceResponse> ReplyAttachmentAsync(this IDialogContext context, Attachment attachment, string text = null, List<CardAction> suggestedActions = null, int waitTime = 0, bool isSendTyping = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            var botContext = BotDialogContext.Create(context).EnableShowTyping(isSendTyping, waitTime);
            return await botContext.ReplyAttachmentAsync(attachment: attachment, text: text, suggestedActions: suggestedActions, cancellationToken: cancellationToken);
        }
        public static async Task<ResourceResponse> ReplyAttachmentAsync(this IDialogContext context, IEnumerable<Attachment> attachments, string text = null, string layout = AttachmentLayoutTypes.Carousel, List<CardAction> suggestedActions = null, int waitTime = 0, bool isSendTyping = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            var botContext = BotDialogContext.Create(context).EnableShowTyping(isSendTyping, waitTime);
            return await botContext.ReplyAttachmentAsync(attachments: attachments, text: text, layout: layout, suggestedActions: suggestedActions, cancellationToken: cancellationToken);
        }
        public static async Task<ResourceResponse> ReplyMediaAsync(this IDialogContext context, string contentUrl, string contentType, string fileName, List<CardAction> suggestedActions = null, int waitTime = 0, bool isSendTyping = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            var botContext = BotDialogContext.Create(context).EnableShowTyping(isSendTyping, waitTime);
            return await botContext.ReplyMediaAsync(contentUrl: contentUrl, contentType: contentType, fileName: fileName, suggestedActions: suggestedActions, cancellationToken: cancellationToken);
        }

        // 回覆語音
        public static async Task<ResourceResponse> ReplySpeakAsync(this IDialogContext context, string speakText, string inputHint = null, int waitTime = 0, bool isSendTyping = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            var botContext = BotDialogContext.Create(context).EnableShowTyping(isSendTyping, waitTime);
            return await botContext.ReplySpeakAsync(speakText: speakText, inputHint: inputHint, cancellationToken: cancellationToken);
        }

        // 回覆正在打字狀態
        public static async Task<ResourceResponse> ReplyTypingAsync(this IDialogContext context, int waitTime = 0, bool isSendTyping = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            var botContext = BotDialogContext.Create(context).EnableShowTyping(isSendTyping, waitTime);
            return await botContext.ReplyTypingAsync(cancellationToken: cancellationToken);
        }

        #endregion


        #region Update Message & Delete Message

        // 修改訊息
        public static ResourceResponse UpdateMessage(this IDialogContext context, string activityId, string message)
        {
            var botContext = BotDialogContext.Create(context);
            return botContext.UpdateMessage(activityId, message);
        }
        public static async Task<ResourceResponse> UpdateMessageAsync(this IDialogContext context, string activityId, string message)
        {
            var botContext = BotDialogContext.Create(context);
            return await botContext.UpdateMessageAsync(activityId, message);
        }

        // 刪除訊息
        public static void DeleteMessage(this IDialogContext context, string activityId)
        {
            var botContext = BotDialogContext.Create(context);
            botContext.DeleteMessage(activityId);
        }
        public static async Task DeleteMessageAsync(this IDialogContext context, string activityId)
        {
            var botContext = BotDialogContext.Create(context);
            await botContext.DeleteMessageAsync(activityId);
        }

        #endregion


        #region Prompt Message

        // Prompt Text
        public static void PromptText(this IDialogContext context, ResumeAfter<string> resume, string prompt, string retry = null, int attempts = 3, string pattern = null, RegexOptions regexOption = RegexOptions.None)
        {
            PromptAdaptiveDialog.Text(context, resume, prompt, retry, attempts, pattern, regexOption);
        }
        public static void PromptText(this IDialogContext context, ResumeAfter<string> resume, IEnumerable<string> patterns, string prompt, string retry = null, int attempts = 3, RegexOptions regexOption = RegexOptions.None)
        {
            PromptAdaptiveDialog.Text(context, resume, patterns, prompt, retry, attempts, regexOption);
        }

        // Prompt Number
        public static void PromptNumber(this IDialogContext context, ResumeAfter<long> resume, string prompt, string retry = null, int attempts = 3, string speak = null, long? min = default(long?), long? max = default(long?))
        {
            PromptAdaptiveDialog.Number(context, resume, prompt, retry, attempts, speak, min, max);
        }
        public static void PromptNumber(this IDialogContext context, ResumeAfter<double> resume, string prompt, string retry = null, int attempts = 3, string speak = null, double? min = default(double?), double? max = default(double?))
        {
            PromptAdaptiveDialog.Number(context, resume, prompt, retry, attempts, speak, min, max);
        }

        // Prompt DateTime
        public static void PromptDateTime(this IDialogContext context, ResumeAfter<DateTime> resume, string prompt, string retry = null, int attempts = 3, IEnumerable<string> patterns = null)
        {
            PromptAdaptiveDialog.DateTime(context, resume, prompt, retry, attempts, patterns);
        }

        // Prompt Confirm
        public static void PromptConfirm(this IDialogContext context, ResumeAfter<bool> resume, string prompt, string retry = null, int attempts = 3, bool isAdaptiveCard = true, PromptStyle promptStyle = PromptStyle.Auto, string[] options = null, string[][] patterns = null)
        {
            PromptAdaptiveDialog.Confirm(context, resume, prompt, retry, attempts, isAdaptiveCard, promptStyle, options, patterns);
        }

        // Prompt Choice
        public static void PromptChoice<T>(this IDialogContext context, ResumeAfter<T> resume, IEnumerable<T> options, string prompt, string retry = null, int attempts = 3, bool isAdaptiveCard = true, PromptStyle promptStyle = PromptStyle.Auto, IEnumerable<string> descriptions = null)
        {
            PromptAdaptiveDialog.Choice(context, resume, options, prompt, retry, attempts, isAdaptiveCard, promptStyle, descriptions);
        }
        public static void PromptChoice<T>(this IDialogContext context, ResumeAfter<T> resume, IDictionary<T, IEnumerable<T>> choices, string prompt, string retry = null, int attempts = 3, bool isAdaptiveCard = true, PromptStyle promptStyle = PromptStyle.Auto, IEnumerable<string> descriptions = null, bool recognizeChoices = true, bool recognizeNumbers = true, bool recognizeOrdinals = true, double minScore = 0.4)
        {
            PromptAdaptiveDialog.Choice(context, resume, choices, prompt, retry, attempts, isAdaptiveCard, promptStyle, descriptions, recognizeChoices, recognizeNumbers, recognizeOrdinals, minScore);
        }

        // Prompt AdaptiveCard
        public static void PromptAdaptiveCard(this IDialogContext context, ResumeAfter<AdaptiveCardResult> resume, AdaptiveCard card, string prompt = null, string retry = null, int attempts = 3, bool isRememberUserInput = true, PromptAdaptiveCardVaildateDelegate vaildDelegate = null)
        {
            PromptAdaptiveDialog.AdaptiveCard(context, resume, card, prompt, retry, attempts, isRememberUserInput, vaildDelegate);
        }

        // Prompt Attachment
        public static void PromptAttachment(this IDialogContext context, ResumeAfter<IEnumerable<Attachment>> resume, string prompt, IEnumerable<string> contentTypes = null, string retry = null, int attempts = 3)
        {
            PromptAdaptiveDialog.Attachment(context, resume, prompt, contentTypes, retry, attempts);
        }

        #endregion
    }
}