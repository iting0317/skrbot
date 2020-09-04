using AdaptiveCards;
using Skrbot.WebApi.BotHelpers.Adaptives;
using Skrbot.WebApi.BotHelpers.Extends;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skrbot.WebApi.BotHelpers.Dialogs
{
    public delegate PromptVaildateResult PromptAdaptiveCardVaildateDelegate(AdaptiveCardResult result);

    // Prompt Adaptive Card
    [Serializable]
    public class PromptAdaptiveCard : IDialog<AdaptiveCardResult>
    {
        protected readonly IPromptOptions<string> promptOptions;

        // 卡片內容
        protected string CardContent { get; }

        // 重試訊息 (重填的理由)
        protected string VaildateReason { get; set; } = string.Empty;

        // 是否記住使用者前一次錯誤輸入
        protected bool IsRememberUserInput { get; set; }

        // 使用者前一次錯誤輸入的資料
        private string _rememberedUserInputData = string.Empty;
        protected JObject RememberedUserInputData
        {
            get
            {
                if (string.IsNullOrEmpty(_rememberedUserInputData))
                {
                    return null;
                }
                try
                {
                    return JObject.Parse(_rememberedUserInputData);
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                if (value != null)
                {
                    _rememberedUserInputData = value.ToString();
                }
            }
        }


        public event PromptAdaptiveCardVaildateDelegate VaildateResult = null;

        // 重試文字訊息
        protected string Retry
        {
            get
            {
                if (string.IsNullOrEmpty(VaildateReason))
                {
                    return promptOptions.Retry;
                }
                return VaildateReason + Environment.NewLine + promptOptions.Retry;
            }
        }


        public PromptAdaptiveCard(AdaptiveCard card, string prompt, string retry, int attempts, bool isRememberUserInput, PromptAdaptiveCardVaildateDelegate vaildDelegate) : this(card, new PromptOptions<string>(prompt, retry, attempts: attempts), isRememberUserInput, vaildDelegate)
        {

        }
        public PromptAdaptiveCard(AdaptiveCard card, IPromptOptions<string> promptOptions, bool isRememberUserInput, PromptAdaptiveCardVaildateDelegate vaildDelegate)
        {
            SetField.NotNull(out this.promptOptions, nameof(promptOptions), promptOptions);

            // 是否記住使用者輸入的Form
            IsRememberUserInput = isRememberUserInput;

            // 卡片內容轉成JSON字串
            CardContent = JsonConvert.SerializeObject(card ?? new AdaptiveCard());

            // 設定Adaptive Card資料驗證Callback
            VaildateResult = vaildDelegate;
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.ReplyMessageAsync(MakePrompt(context, promptOptions.Prompt, promptOptions.Choices?.Keys.ToList().AsReadOnly(), promptOptions.Descriptions, promptOptions.Speak));
            context.Wait(MessageReceivedAsync);
        }

        // 建立Prpmpt
        protected IMessageActivity MakePrompt(IDialogContext context, string prompt, IReadOnlyList<string> options = null, IReadOnlyList<string> descriptions = null, string speak = null)
        {
            // 建立Adaptive Card訊息
            var card = JsonConvert.DeserializeObject<AdaptiveCard>(CardContent);
            var msg = context.MakeMessage();

            msg.Text = prompt;
            msg.Attachments = new List<Attachment>() { card.ToAttachment() };
            msg.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            return msg;
        }

        // 訊息接收後
        protected async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> message)
        {
            AdaptiveCardResult result;
            VaildateReason = string.Empty;
            RememberedUserInputData = null;

            if (TryParse(await message, out result))
            {
                context.Done(result);
            }
            else
            {
                --promptOptions.Attempts;
                if (promptOptions.Attempts >= 0)
                {
                    // 建立重試的訊息
                    var retryMessage = context.MakeMessage();

                    var card = JsonConvert.DeserializeObject<AdaptiveCard>(CardContent);
                    if (IsRememberUserInput && RememberedUserInputData != null)
                    {   // 自動填入使用者前一次的錯誤輸入
                        card.BindData(RememberedUserInputData);
                    }
                    // 重試訊息
                    retryMessage.Text = Retry;
                    retryMessage.Attachments = new List<Attachment>() { card.ToAttachment() };
                    retryMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;

                    await context.ReplyMessageAsync(retryMessage);

                    context.Wait(MessageReceivedAsync);
                }
                else
                {
                    //too many attempts, throw.
                    await context.ReplyMessageAsync(promptOptions.TooManyAttempts);
                    throw new TooManyAttemptsException(promptOptions.TooManyAttempts);
                }
            }
        }

        // 剖析訊息內容
        protected bool TryParse(IMessageActivity message, out AdaptiveCardResult result)
        {
            bool parseResult = false;
            result = new AdaptiveCardResult();

            // 取得Text內容
            if (!string.IsNullOrEmpty(message.Text))
            {
                result.Text = message.Text;
                parseResult = parseResult | true;
            }

            // 取得Value內容
            if (message.Value != null)
            {
                try
                {
                    RememberedUserInputData = JObject.FromObject(message.Value);
                    result.Value = RememberedUserInputData;
                    parseResult = parseResult | true;
                }
                catch (Exception e)
                {
                    result.InnerException = e;
                }
            }

            if (VaildateResult != null)
            {   // 執行自訂驗證處理
                var vaildResult = VaildateResult.Invoke(result);
                VaildateReason = vaildResult.VaildateReason;
                return vaildResult.IsVaild;
            }

            return parseResult;
        }
    }

    [Serializable]
    public class AdaptiveCardResult
    {
        public string Text { get; set; }

        private string _value = string.Empty;

        public JObject Value
        {
            get
            {
                if (string.IsNullOrEmpty(_value))
                {
                    return null;
                }
                try
                {
                    return JObject.Parse(_value);
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                if (value != null)
                {
                    _value = value.ToString();
                }
            }
        }

        public Exception InnerException { get; set; }
    }
    [Serializable]
    public class PromptVaildateResult
    {
        public PromptVaildateResult()
        {

        }

        public PromptVaildateResult(bool isVaild, string reason = "")
        {
            IsVaild = isVaild;
            VaildateReason = reason;
        }

        // 資料檢查是否通過
        public bool IsVaild { get; set; }

        // 重試訊息 (重填的理由)
        public string VaildateReason { get; set; }
    }
}