using AdaptiveCards;
using Skrbot.WebApi.BotHelpers.Adaptives;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Resource;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Skrbot.WebApi.BotHelpers.Dialogs
{
    [Serializable]
    public static class PromptAdaptiveDialog
    {
        // Text
        public static void Text(IDialogContext context, ResumeAfter<string> resume, string prompt, string retry = null, int attempts = 3, string pattern = null, RegexOptions regexOption = RegexOptions.None)
        {
            var child = new PromptText(prompt, retry, attempts, pattern, regexOption);
            context.Call(child, resume);
        }
        public static void Text(IDialogContext context, ResumeAfter<string> resume, IEnumerable<string> patterns, string prompt, string retry = null, int attempts = 3, RegexOptions regexOption = RegexOptions.None)
        {
            var child = new PromptText(prompt, retry, attempts, patterns, regexOption);
            context.Call(child, resume);
        }

        // Number
        public static void Number(IDialogContext context, ResumeAfter<long> resume, string prompt, string retry = null, int attempts = 3, string speak = null, long? min = default(long?), long? max = default(long?))
        {
            PromptDialog.Number(context, resume, prompt, retry, attempts, speak, min, max);
        }
        public static void Number(IDialogContext context, ResumeAfter<double> resume, string prompt, string retry = null, int attempts = 3, string speak = null, double? min = default(double?), double? max = default(double?))
        {
            PromptDialog.Number(context, resume, prompt, retry, attempts, speak, min, max);
        }

        // DateTime
        public static void DateTime(IDialogContext context, ResumeAfter<DateTime> resume, string prompt, string retry = null, int attempts = 3, IEnumerable<string> patterns = null)
        {
            var child = new PromptDateTime(prompt, retry, attempts, patterns);
            context.Call(child, resume);
        }

        // Attachment
        public static void Attachment(IDialogContext context, ResumeAfter<IEnumerable<Attachment>> resume, string prompt, IEnumerable<string> contentTypes = null, string retry = null, int attempts = 3)
        {
            PromptDialog.Attachment(context, resume, prompt, contentTypes, retry, attempts);
        }

        // Confirm
        public static void Confirm(IDialogContext context, ResumeAfter<bool> resume, IPromptOptions<string> promptOptions, bool isAdaptiveCard = true, PromptStyle promptStyle = PromptStyle.Auto, string[][] patterns = null)
        {
            var adPromptOptions = new PromptOptions<string>(promptOptions.Prompt,
                                                    promptOptions.Retry,
                                                    attempts: promptOptions.Attempts,
                                                    options: promptOptions.Options ?? PromptDialog.PromptConfirm.Options,
                                                    promptStyler: new AdaptivePromptStyler(promptStyle, isAdaptiveCard));
            PromptDialog.Confirm(context, resume, adPromptOptions, patterns);
        }
        public static void Confirm(IDialogContext context, ResumeAfter<bool> resume, string prompt, string retry = null, int attempts = 3, bool isAdaptiveCard = true, PromptStyle promptStyle = PromptStyle.Auto, string[] options = null, string[][] patterns = null)
        {
            var adPromptOptions = new PromptOptions<string>(prompt,
                                                            retry,
                                                            attempts: attempts,
                                                            options: options ?? PromptDialog.PromptConfirm.Options,
                                                            promptStyler: new AdaptivePromptStyler(promptStyle, isAdaptiveCard));
            PromptDialog.Confirm(context, resume, adPromptOptions, patterns);
        }

        // Choice
        public static void Choice<T>(IDialogContext context, ResumeAfter<T> resume, IPromptOptions<T> promptOptions, bool isAdaptiveCard = true, PromptStyle promptStyle = PromptStyle.Auto, bool recognizeChoices = true, bool recognizeNumbers = true, bool recognizeOrdinals = true, double minScore = 0.4)
        {
            IPromptOptions<T> adPromptOptions;
            if (promptOptions.GetType() == typeof(PromptOptions<T>))
            {
                adPromptOptions = new PromptOptions<T>(promptOptions.Prompt,
                                                       promptOptions.Retry,
                                                       attempts: promptOptions.Attempts,
                                                       options: promptOptions.Options.ToList(),
                                                       promptStyler: new AdaptivePromptStyler(promptStyle, isAdaptiveCard),
                                                       descriptions: promptOptions.Descriptions?.ToList());
            }
            else
            {
                adPromptOptions = new PromptOptionsWithSynonyms<T>(promptOptions.Prompt,
                                                                   promptOptions.Retry,
                                                                   attempts: promptOptions.Attempts,
                                                                   choices: promptOptions.Choices.ToDictionary(x => x.Key,
                                                                                                               x => (IReadOnlyList<T>)x.Value.ToList().AsReadOnly()),
                                                                   promptStyler: new AdaptivePromptStyler(promptStyle, isAdaptiveCard),
                                                                   descriptions: promptOptions.Descriptions?.ToList());
            }
            PromptDialog.Choice(context, resume, adPromptOptions, recognizeChoices, recognizeNumbers, recognizeOrdinals, minScore);
        }
        public static void Choice<T>(IDialogContext context, ResumeAfter<T> resume, IEnumerable<T> options, string prompt, string retry = null, int attempts = 3, bool isAdaptiveCard = true, PromptStyle promptStyle = PromptStyle.Auto, IEnumerable<string> descriptions = null)
        {
            var adPromptOptions = new PromptOptions<T>(prompt, retry, attempts: attempts,
                                                       options: options.ToList(),
                                                       promptStyler: new AdaptivePromptStyler(promptStyle, isAdaptiveCard),
                                                       descriptions: descriptions?.ToList());
            PromptDialog.Choice(context, resume, adPromptOptions);
        }
        public static void Choice<T>(IDialogContext context, ResumeAfter<T> resume, IDictionary<T, IEnumerable<T>> choices, string prompt, string retry = null, int attempts = 3, bool isAdaptiveCard = true, PromptStyle promptStyle = PromptStyle.Auto, IEnumerable<string> descriptions = null, bool recognizeChoices = true, bool recognizeNumbers = true, bool recognizeOrdinals = true, double minScore = 0.4)
        {
            var adPromptOptions = new PromptOptionsWithSynonyms<T>(prompt,
                                                                   retry,
                                                                   attempts: attempts,
                                                                   choices: choices.ToDictionary(x => x.Key,
                                                                                                 x => (IReadOnlyList<T>)x.Value.ToList().AsReadOnly()),
                                                                   promptStyler: new AdaptivePromptStyler(promptStyle, isAdaptiveCard),
                                                                   descriptions: descriptions?.ToList());
            PromptDialog.Choice(context, resume, adPromptOptions, recognizeChoices, recognizeNumbers, recognizeOrdinals, minScore);
        }

        // Adaptive Card
        public static void AdaptiveCard(IDialogContext context, ResumeAfter<AdaptiveCardResult> resume, AdaptiveCard card, string prompt = null, string retry = null, int attempts = 3, bool isRememberUserInput = true, PromptAdaptiveCardVaildateDelegate vaildDelegate = null)
        {
            var child = new PromptAdaptiveCard(card, prompt, retry, attempts, isRememberUserInput, vaildDelegate);
            context.Call(child, resume);
        }
    }

    // Adaptive Card Prompt Styler
    [Serializable]
    public class AdaptivePromptStyler : PromptStyler
    {
        protected bool isAdaptiveCard = true;
        public AdaptivePromptStyler(PromptStyle promptStyle = PromptStyle.Auto, bool isAdaptiveCard = true) : base(promptStyle)
        {
            this.isAdaptiveCard = isAdaptiveCard;
        }

        public override void Apply<T>(ref IMessageActivity message, string prompt, IReadOnlyList<T> options, IReadOnlyList<string> descriptions = null, string speak = null)
        {
            SetField.CheckNull(nameof(prompt), prompt);
            SetField.CheckNull(nameof(options), options);
            message.Speak = speak;
            message.InputHint = InputHints.ExpectingInput;
            if (descriptions == null)
            {
                descriptions = (from option in options select option.ToString()).ToList();
            }
            switch (PromptStyle)
            {
                case PromptStyle.Auto:
                case PromptStyle.Keyboard:
                    if (options != null && options.Any())
                    {
                        if (PromptStyle == PromptStyle.Keyboard)
                        {
                            message.SuggestedActions = new SuggestedActions(actions: GenerateButtons(options, descriptions));
                            message.Text = prompt;
                        }
                        else
                        {
                            var heroCard = new HeroCard(text: prompt, buttons: GenerateButtons(options, descriptions));
                            message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                            message.Attachments = new List<Attachment>()
                            {
                                (isAdaptiveCard) ? heroCard.ToAdaptiveCard().ToAttachment() : heroCard.ToAttachment()
                            };
                        }
                    }
                    else
                    {
                        message.Text = prompt;
                    }
                    break;
                case PromptStyle.AutoText:
                    Apply(ref message, prompt, options, options?.Count() > 4 ? PromptStyle.PerLine : PromptStyle.Inline, descriptions);
                    break;
                case PromptStyle.Inline:
                    //TODO: Refactor buildlist function to a more generic namespace when changing prompt to use recognizers.
                    message.Text = $"{prompt} {Microsoft.Bot.Builder.FormFlow.Advanced.Language.BuildList(descriptions, Resources.DefaultChoiceSeparator, Resources.DefaultChoiceLastSeparator)}";
                    break;
                case PromptStyle.PerLine:
                    message.Text = $"{prompt}{Environment.NewLine}{Microsoft.Bot.Builder.FormFlow.Advanced.Language.BuildList(descriptions.Select(description => $"* {description}"), Environment.NewLine, Environment.NewLine)}";
                    break;
                case PromptStyle.None:
                default:
                    message.Text = prompt;
                    break;
            }
        }

        protected IList<CardAction> GenerateButtons<T>(IEnumerable<T> options, IEnumerable<string> descriptions = null)
        {
            var actions = new List<CardAction>();
            int i = 0;
            var adescriptions = descriptions?.ToArray();
            foreach (var option in options)
            {
                var title = (adescriptions == null ? option.ToString() : adescriptions[i]);
                actions.Add(new CardAction
                {
                    Title = title,
                    Type = ActionTypes.ImBack,
                    Value = option.ToString()
                });
                ++i;
            }
            return actions;
        }
    }
}