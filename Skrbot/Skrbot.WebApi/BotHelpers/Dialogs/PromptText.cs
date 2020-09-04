using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Skrbot.WebApi.BotHelpers.Dialogs
{
    // Prompt Text
    [Serializable]
    public class PromptText : Prompt<string, string>
    {
        public List<string> Patterns { get; }
        public RegexOptions Option { get; }

        public PromptText(string prompt, string retry, int attempts, string pattern, RegexOptions regexOption = RegexOptions.None) : this(new PromptOptions<string>(prompt, retry, attempts: attempts), pattern, regexOption)
        {

        }
        public PromptText(string prompt, string retry, int attempts, IEnumerable<string> patterns, RegexOptions regexOption = RegexOptions.None) : this(new PromptOptions<string>(prompt, retry, attempts: attempts), patterns, regexOption)
        {

        }
        public PromptText(IPromptOptions<string> promptOptions, string pattern, RegexOptions regexOption) : base(promptOptions)
        {
            Patterns = (string.IsNullOrEmpty(pattern)) ? new List<string>() : new List<string>() { pattern };
            Option = regexOption;
        }
        public PromptText(IPromptOptions<string> promptOptions, IEnumerable<string> patterns, RegexOptions regexOption) : base(promptOptions)
        {
            Patterns = (patterns == null) ? new List<string>() : new List<string>(patterns);
            Option = regexOption;
        }

        //剖析訊息內容
        protected override bool TryParse(IMessageActivity message, out string result)
        {
            if (string.IsNullOrWhiteSpace(message.Text))
            {   // 空白訊息
                result = null;
                return false;
            }

            if (Patterns.Count == 0)
            {   // 不檢查規則
                result = message.Text;
                return true;
            }
            else
            {
                // 判斷訊息是否符合
                foreach (var pattern in Patterns)
                {
                    if (Regex.IsMatch(message.Text, pattern, Option))
                    {   // 符合規則
                        result = message.Text;
                        return true;
                    }
                }

                // 不符合規則
                result = null;
                return false;
            }
        }
    }
}