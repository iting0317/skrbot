using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Skrbot.WebApi.BotHelpers.Dialogs
{
    // Prompt DateTime
    [Serializable]
    public class PromptDateTime : Prompt<DateTime, string>
    {
        public List<string> Patterns { get; }

        public PromptDateTime(string prompt, string retry, int attempts, string pattern) : this(new PromptOptions<string>(prompt, retry, attempts: attempts), pattern)
        {

        }
        public PromptDateTime(string prompt, string retry, int attempts, IEnumerable<string> patterns) : this(new PromptOptions<string>(prompt, retry, attempts: attempts), patterns)
        {

        }
        public PromptDateTime(IPromptOptions<string> promptOptions, IEnumerable<string> patterns) : base(promptOptions)
        {
            Patterns = (patterns == null) ? new List<string>() : new List<string>(patterns);
        }
        public PromptDateTime(IPromptOptions<string> promptOptions, string pattern) : base(promptOptions)
        {
            Patterns = (string.IsNullOrEmpty(pattern)) ? new List<string>() : new List<string>() { pattern };
        }

        //剖析訊息內容
        protected override bool TryParse(IMessageActivity message, out DateTime result)
        {
            if (Patterns.Count == 0)
            {   // 預設日期時間格式
                if (DateTime.TryParse(message.Text, out result))
                {
                    return true;
                }
                return false;
            }
            else
            {   // 自訂日期時間格式
                if (DateTime.TryParseExact(message.Text, Patterns.ToArray(),
                                           CultureInfo.InvariantCulture,
                                           DateTimeStyles.None, out result))
                {
                    return true;
                }
                return false;
            }
        }
    }
}