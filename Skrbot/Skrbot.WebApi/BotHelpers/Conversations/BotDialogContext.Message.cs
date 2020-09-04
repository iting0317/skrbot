using Skrbot.WebApi.BotHelpers.Extends;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Skrbot.WebApi.BotHelpers.Conversations
{
    /// <summary>
    /// 處理Bot訊息發送/回覆
    /// </summary>
    public partial class BotDialogContext
    {
        public const int DefaultTypingTime = 1000;  // 1秒鐘
        public bool IsDisplayTyping { get; set; } = false;
        public int TypingTime { get; set; } = DefaultTypingTime;


        /// <summary>
        /// 顯示Bot正在打字
        /// </summary>
        /// <param name="typingTimeMs">打字時間(毫秒)</param>
        /// <returns></returns>
        public BotDialogContext EnableShowTyping(bool isDisplayTyping = true, int typingTimeMs = DefaultTypingTime)
        {
            IsDisplayTyping = isDisplayTyping;
            TypingTime = typingTimeMs;
            return this;
        }


        #region Reply Message (Sync)

        public ResourceResponse ReplyMessage(string message, List<CardAction> suggestedActions = null)
        {
            // Create Reply
            var reply = Activity.CreateReply(message);
            reply.ChannelData = Activity.ChannelData;

            // Add Suggested Action
            if (suggestedActions != null && suggestedActions.Count != 0)
            {
                reply.SuggestedActions = new SuggestedActions()
                {
                    Actions = suggestedActions
                };
            }

            // Reply
            return ReplyToActivity(reply);
        }
        public ResourceResponse ReplyMessage(IMessageActivity message)
        {
            // Reply
            return ReplyToActivity(message);
        }
        public ResourceResponse ReplyMessage(string message, string action, string actionName = null)
        {
            // Create Reply
            var reply = Activity.CreateReply(message);
            reply.ChannelData = Activity.ChannelData;
            reply.Action = action;
            reply.Name = (!string.IsNullOrEmpty(actionName)) ? actionName : string.Empty;

            // Reply
            return ReplyToActivity(reply);
        }


        public ResourceResponse ReplyAttachment(Attachment attachment, string text = null, List<CardAction> suggestedActions = null)
        {
            // Create Reply
            var reply = (Activity as Activity).CreateReply();
            reply.ChannelData = Activity.ChannelData;
            reply.Attachments = new List<Attachment>() { attachment };
            reply.AttachmentLayout = AttachmentLayoutTypes.List;

            // Add Text
            if (!string.IsNullOrEmpty(text))
            {
                reply.Text = text;
            }

            // Add Suggested Action
            if (suggestedActions != null && suggestedActions.Count != 0)
            {
                reply.SuggestedActions = new SuggestedActions()
                {
                    Actions = suggestedActions
                };
            }

            // Reply
            return ReplyToActivity(reply);
        }
        public ResourceResponse ReplyAttachment(IEnumerable<Attachment> attachments, string text = null, string layout = AttachmentLayoutTypes.Carousel, List<CardAction> suggestedActions = null)
        {
            // Create Reply
            var reply = (Activity as Activity).CreateReply();
            reply.ChannelData = Activity.ChannelData;
            reply.Attachments = new List<Attachment>(attachments);
            reply.AttachmentLayout = layout;

            // Add Text
            if (!string.IsNullOrEmpty(text))
            {
                reply.Text = text;
            }

            // Add Suggested Action
            if (suggestedActions != null && suggestedActions.Count != 0)
            {
                reply.SuggestedActions = new SuggestedActions()
                {
                    Actions = suggestedActions
                };
            }

            // Reply
            return ReplyToActivity(reply);
        }
        public ResourceResponse ReplyMedia(string contentUrl, string contentType, string fileName, List<CardAction> suggestedActions = null)
        {
            var attachment = new Attachment()
            {
                ContentUrl = contentUrl,
                ContentType = contentType,
                Name = fileName
            };
            return ReplyAttachment(attachment: attachment, suggestedActions: suggestedActions);
        }

        public ResourceResponse ReplySpeak(string speakText, string inputHint = null)
        {
            // Create Reply
            var reply = Activity.CreateReply(speakText);
            reply.ChannelData = Activity.ChannelData;
            reply.Speak = speakText;
            if (!string.IsNullOrEmpty(inputHint))
            {
                reply.InputHint = inputHint;
            }

            // Reply
            return ReplyToActivity(reply);
        }

        public ResourceResponse ReplyTyping()
        {
            // Create Reply
            var reply = Activity.CreateReply();
            reply.ChannelData = Activity.ChannelData;
            reply.Type = ActivityTypes.Typing;

            return ReplyToActivity(reply);
        }

        #endregion


        #region Reply Message (Async)

        public async Task<ResourceResponse> ReplyMessageAsync(string message, List<CardAction> suggestedActions = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Create Reply
            var reply = Activity.CreateReply(message);
            reply.ChannelData = Activity.ChannelData;

            // Add Suggested Action
            if (suggestedActions != null && suggestedActions.Count != 0)
            {
                reply.SuggestedActions = new SuggestedActions()
                {
                    Actions = suggestedActions
                };
            }

            // Reply
            return await ReplyToActivityAsync(reply, cancellationToken);
        }
        public async Task<ResourceResponse> ReplyMessageAsync(IMessageActivity message, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Reply
            return await ReplyToActivityAsync(message, cancellationToken);
        }
        public async Task<ResourceResponse> ReplyMessageAsync(string message, string action, string actionName = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Create Reply
            var reply = Activity.CreateReply(message);
            reply.ChannelData = Activity.ChannelData;
            reply.Action = action;
            reply.Name = (!string.IsNullOrEmpty(actionName)) ? actionName : string.Empty;

            // Reply
            return await ReplyToActivityAsync(reply, cancellationToken);
        }


        public async Task<ResourceResponse> ReplyAttachmentAsync(Attachment attachment, string text = null, List<CardAction> suggestedActions = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Create Reply
            var reply = (Activity as Activity).CreateReply();
            reply.ChannelData = Activity.ChannelData;
            reply.Attachments = new List<Attachment>() { attachment };
            reply.AttachmentLayout = AttachmentLayoutTypes.List;

            // Add Text
            if (!string.IsNullOrEmpty(text))
            {
                reply.Text = text;
            }

            // Add Suggested Action
            if (suggestedActions != null && suggestedActions.Count != 0)
            {
                reply.SuggestedActions = new SuggestedActions()
                {
                    Actions = suggestedActions
                };
            }

            // Reply
            return await ReplyToActivityAsync(reply, cancellationToken);
        }
        public async Task<ResourceResponse> ReplyAttachmentAsync(IEnumerable<Attachment> attachments, string text = null, string layout = AttachmentLayoutTypes.Carousel, List<CardAction> suggestedActions = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Create Reply
            var client = new ConnectorClient(new Uri(Activity.ServiceUrl));
            var reply = (Activity as Activity).CreateReply();
            reply.ChannelData = Activity.ChannelData;
            reply.Attachments = new List<Attachment>(attachments);
            reply.AttachmentLayout = layout;

            // Add Text
            if (!string.IsNullOrEmpty(text))
            {
                reply.Text = text;
            }

            // Add Suggested Action
            if (suggestedActions != null && suggestedActions.Count != 0)
            {
                reply.SuggestedActions = new SuggestedActions()
                {
                    Actions = suggestedActions
                };
            }

            // Reply
            return await ReplyToActivityAsync(reply, cancellationToken);
        }
        public async Task<ResourceResponse> ReplyMediaAsync(string contentUrl, string contentType, string fileName, List<CardAction> suggestedActions = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var attachment = new Attachment()
            {
                ContentUrl = contentUrl,
                ContentType = contentType,
                Name = fileName
            };
            return await ReplyAttachmentAsync(attachment: attachment, suggestedActions: suggestedActions, cancellationToken: cancellationToken);
        }


        public async Task<ResourceResponse> ReplySpeakAsync(string speakText, string inputHint = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Create Reply
            var reply = Activity.CreateReply(speakText);
            reply.ChannelData = Activity.ChannelData;
            reply.Speak = speakText;
            if (!string.IsNullOrEmpty(inputHint))
            {
                reply.InputHint = inputHint;
            }

            // Reply
            return await ReplyToActivityAsync(reply, cancellationToken);
        }

        public async Task<ResourceResponse> ReplyTypingAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // Create Reply
            var reply = Activity.CreateReply();
            reply.ChannelData = Activity.ChannelData;
            reply.Type = ActivityTypes.Typing;

            // Reply
            return await ReplyToActivityAsync(reply, cancellationToken);
        }

        #endregion


        #region Send Message (Sync)

        // Send Message
        public ResourceResponse SendMessage(string message, List<CardAction> suggestedActions = null)
        {
            // Create Message
            var client = new ConnectorClient(new Uri(Activity.ServiceUrl));
            var reply = (Activity as Activity).CreateReply(message);
            reply.ChannelData = Activity.ChannelData;

            // Add Suggested Action
            if (suggestedActions != null && suggestedActions.Count != 0)
            {
                reply.SuggestedActions = new SuggestedActions()
                {
                    Actions = suggestedActions
                };
            }

            // Send Message
            return SendToConversation(reply);
        }
        public ResourceResponse SendMessage(IMessageActivity message)
        {
            // Send Message
            return SendToConversation(message);
        }
        public ResourceResponse SendMessage(string message, string action, string actionName = null)
        {
            // Create Message
            var reply = (Activity as Activity).CreateReply(message);
            reply.ChannelData = Activity.ChannelData;
            reply.Action = action;
            reply.Name = (!string.IsNullOrEmpty(actionName)) ? actionName : string.Empty;

            // Send Message
            return SendToConversation(reply);
        }

        // Send Attachment
        public ResourceResponse SendAttachment(Attachment attachment, string text = null, List<CardAction> suggestedActions = null)
        {
            // Send Message
            var reply = (Activity as Activity).CreateReply();
            reply.ChannelData = Activity.ChannelData;
            reply.Attachments = new List<Attachment>() { attachment };
            reply.AttachmentLayout = AttachmentLayoutTypes.List;

            // Add Text
            if (!string.IsNullOrEmpty(text))
            {
                reply.Text = text;
            }

            // Add Suggested Action
            if (suggestedActions != null && suggestedActions.Count != 0)
            {
                reply.SuggestedActions = new SuggestedActions()
                {
                    Actions = suggestedActions
                };
            }

            // Send Message
            return SendToConversation(reply);
        }
        public ResourceResponse SendAttachment(IEnumerable<Attachment> attachments, string text = null, string layout = AttachmentLayoutTypes.Carousel, List<CardAction> suggestedActions = null)
        {
            // Send Message
            var reply = (Activity as Activity).CreateReply();
            reply.ChannelData = Activity.ChannelData;
            reply.Attachments = new List<Attachment>(attachments);
            reply.AttachmentLayout = layout;

            // Add Text
            if (!string.IsNullOrEmpty(text))
            {
                reply.Text = text;
            }

            // Add Suggested Action
            if (suggestedActions != null && suggestedActions.Count != 0)
            {
                reply.SuggestedActions = new SuggestedActions()
                {
                    Actions = suggestedActions
                };
            }

            // Send Message
            return SendToConversation(reply);
        }
        public ResourceResponse SendMedia(string contentUrl, string contentType, string fileName, List<CardAction> suggestedActions = null)
        {
            var attachment = new Attachment()
            {
                ContentUrl = contentUrl,
                ContentType = contentType,
                Name = fileName
            };
            return SendAttachment(attachment: attachment, suggestedActions: suggestedActions);
        }

        // Send Speak
        public ResourceResponse SendSpeak(string speakText, string inputHint = null)
        {
            // Create Message
            var reply = Activity.CreateReply(speakText);
            reply.ChannelData = Activity.ChannelData;
            reply.Speak = speakText;
            if (!string.IsNullOrEmpty(inputHint))
            {
                reply.InputHint = inputHint;
            }

            // Send Message
            return SendToConversation(reply);
        }

        // Send Typing
        public ResourceResponse SendTyping()
        {
            // Create Message
            var reply = Activity.CreateReply();
            reply.ChannelData = Activity.ChannelData;
            reply.Type = ActivityTypes.Typing;

            // Send Message
            return SendToConversation(reply);
        }

        #endregion


        #region Send Message (Async)

        // Send Message
        public async Task<ResourceResponse> SendMessageAsync(string message, List<CardAction> suggestedActions = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Create Message
            var reply = (Activity as Activity).CreateReply(message);
            reply.ChannelData = Activity.ChannelData;

            // Add Suggested Action
            if (suggestedActions != null && suggestedActions.Count != 0)
            {
                reply.SuggestedActions = new SuggestedActions()
                {
                    Actions = suggestedActions
                };
            }

            // Send Message
            return await SendToConversationAsync(reply, cancellationToken);
        }
        public async Task<ResourceResponse> SendMessageAsync(IMessageActivity message, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Send Message
            return await SendToConversationAsync(message, cancellationToken);
        }
        public async Task<ResourceResponse> SendMessageAsync(string message, string action, string actionName = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Create Message
            var reply = (Activity as Activity).CreateReply(message);
            reply.ChannelData = Activity.ChannelData;
            reply.Action = action;
            reply.Name = (!string.IsNullOrEmpty(actionName)) ? actionName : string.Empty;

            // Send Message
            return await SendToConversationAsync(reply, cancellationToken);
        }


        // Send Attachment
        public async Task<ResourceResponse> SendAttachmentAsync(Attachment attachment, string text = null, List<CardAction> suggestedActions = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Create Message
            var reply = (Activity as Activity).CreateReply();
            reply.ChannelData = Activity.ChannelData;
            reply.Attachments = new List<Attachment>() { attachment };
            reply.AttachmentLayout = AttachmentLayoutTypes.List;

            // Add Text
            if (!string.IsNullOrEmpty(text))
            {
                reply.Text = text;
            }

            // Add Suggested Action
            if (suggestedActions != null && suggestedActions.Count != 0)
            {
                reply.SuggestedActions = new SuggestedActions()
                {
                    Actions = suggestedActions
                };
            }

            // Send Message
            return await SendToConversationAsync(reply, cancellationToken);
        }
        public async Task<ResourceResponse> SendAttachmentAsync(IEnumerable<Attachment> attachments, string text = null, string layout = AttachmentLayoutTypes.Carousel, List<CardAction> suggestedActions = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Create Message
            var reply = (Activity as Activity).CreateReply();
            reply.ChannelData = Activity.ChannelData;
            reply.Attachments = new List<Attachment>(attachments);
            reply.AttachmentLayout = layout;

            // Add Text
            if (!string.IsNullOrEmpty(text))
            {
                reply.Text = text;
            }

            // Add Suggested Action
            if (suggestedActions != null && suggestedActions.Count != 0)
            {
                reply.SuggestedActions = new SuggestedActions()
                {
                    Actions = suggestedActions
                };
            }

            // Send Message
            return await SendToConversationAsync(reply, cancellationToken);
        }
        public async Task<ResourceResponse> SendMediaAsync(string contentUrl, string contentType, string fileName, List<CardAction> suggestedActions = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var attachment = new Attachment()
            {
                ContentUrl = contentUrl,
                ContentType = contentType,
                Name = fileName
            };
            return await SendAttachmentAsync(attachment: attachment, suggestedActions: suggestedActions, cancellationToken: cancellationToken);
        }

        // Send Speak
        public async Task<ResourceResponse> SendSpeakAsync(string speakText, string inputHint = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Create Message
            var reply = Activity.CreateReply(speakText);
            reply.ChannelData = Activity.ChannelData;
            reply.Speak = speakText;
            if (!string.IsNullOrEmpty(inputHint))
            {
                reply.InputHint = inputHint;
            }

            // Send Message
            return await SendToConversationAsync(reply, cancellationToken);
        }

        // Send Typing
        public async Task<ResourceResponse> SendTypingAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // Create Message
            var reply = Activity.CreateReply();
            reply.ChannelData = Activity.ChannelData;
            reply.Type = ActivityTypes.Typing;

            // Send Message
            return await SendToConversationAsync(reply, cancellationToken);
        }

        #endregion


        #region Update Message & Delete Message

        // Update Message
        public ResourceResponse UpdateMessage(string activityId, string message)
        {
            var modifyMessage = (Activity as Activity).CreateReply(message);
            modifyMessage.ChannelData = Activity.ChannelData;

            return UpdateActivity(Activity.Conversation.Id, activityId, modifyMessage);
        }
        public async Task<ResourceResponse> UpdateMessageAsync(string activityId, string message)
        {
            var modifyMessage = (Activity as Activity).CreateReply(message);
            modifyMessage.ChannelData = Activity.ChannelData;

            return await UpdateActivityAsync(Activity.Conversation.Id, activityId, modifyMessage);
        }

        // Delete Message
        public void DeleteMessage(string activityId)
        {
            DeleteActivity(Activity.Conversation.Id, activityId);
        }
        public async Task DeleteMessageAsync(string activityId)
        {
            await DeleteActivityAsync(Activity.Conversation.Id, activityId);
        }

        #endregion


        #region Common Method

        // Reply Message
        public ResourceResponse ReplyToActivity(IActivity activity)
        {
            var client = new GssConnectorClient(new Uri(Activity.ServiceUrl));

            if (TypingTime != 0)
            {
                if (IsDisplayTyping)
                {
                    // 顯示Bot正在打字
                    var typingReply = Activity.CreateReply();
                    typingReply.ChannelData = Activity.ChannelData;
                    typingReply.Type = ActivityTypes.Typing;

                    // Reply
                    client.Conversations.ReplyToActivity(typingReply);
                }

                // 等候
                Thread.Sleep(TypingTime);
            }

            return client.Conversations.ReplyToActivity(activity as Activity);
        }
        public async Task<ResourceResponse> ReplyToActivityAsync(IActivity activity, CancellationToken cancellationToken = default(CancellationToken))
        {
            var client = new GssConnectorClient(new Uri(Activity.ServiceUrl));

            if (TypingTime != 0)
            {
                if (IsDisplayTyping)
                {
                    // 顯示Bot正在打字
                    var typingReply = Activity.CreateReply();
                    typingReply.ChannelData = Activity.ChannelData;
                    typingReply.Type = ActivityTypes.Typing;

                    // Reply
                    await client.Conversations.ReplyToActivityAsync(typingReply, cancellationToken);
                }

                // 等候
                Thread.Sleep(TypingTime);
            }

            return await client.Conversations.ReplyToActivityAsync(activity as Activity, cancellationToken);
        }

        // Send Message
        public ResourceResponse SendToConversation(IActivity activity)
        {
            var client = new GssConnectorClient(new Uri(Activity.ServiceUrl));

            if (TypingTime != 0)
            {
                if (IsDisplayTyping)
                {
                    // 顯示Bot正在打字
                    var typingReply = Activity.CreateReply();
                    typingReply.ChannelData = Activity.ChannelData;
                    typingReply.Type = ActivityTypes.Typing;

                    // Reply
                    client.Conversations.SendToConversation(typingReply);
                }

                // 等候
                Thread.Sleep(TypingTime);
            }

            return client.Conversations.SendToConversation(activity as Activity);
        }
        public async Task<ResourceResponse> SendToConversationAsync(IActivity activity, CancellationToken cancellationToken = default(CancellationToken))
        {
            var client = new GssConnectorClient(new Uri(Activity.ServiceUrl));

            if (TypingTime != 0)
            {
                if (IsDisplayTyping)
                {
                    // 顯示Bot正在打字
                    var typingReply = Activity.CreateReply();
                    typingReply.ChannelData = Activity.ChannelData;
                    typingReply.Type = ActivityTypes.Typing;

                    // Reply
                    await client.Conversations.SendToConversationAsync(typingReply, cancellationToken);
                }

                // 等候
                Thread.Sleep(TypingTime);
            }

            return await client.Conversations.SendToConversationAsync(activity as Activity, cancellationToken);
        }

        // Update Message
        public ResourceResponse UpdateActivity(IActivity activity)
        {
            var client = new GssConnectorClient(new Uri(Activity.ServiceUrl));

            return client.Conversations.UpdateActivity(activity as Activity);
        }
        public ResourceResponse UpdateActivity(string conversationId, string activityId, IActivity activity)
        {
            var client = new GssConnectorClient(new Uri(Activity.ServiceUrl));

            return client.Conversations.UpdateActivity(conversationId, activityId, activity as Activity);
        }
        public async Task<ResourceResponse> UpdateActivityAsync(string conversationId, string activityId, IActivity activity, CancellationToken cancellationToken = default(CancellationToken))
        {
            var client = new GssConnectorClient(new Uri(Activity.ServiceUrl));

            return await client.Conversations.UpdateActivityAsync(conversationId, activityId, activity as Activity, cancellationToken);
        }
        public async Task<ResourceResponse> UpdateActivityAsync(IActivity activity, CancellationToken cancellationToken = default(CancellationToken))
        {
            var client = new GssConnectorClient(new Uri(Activity.ServiceUrl));

            return await client.Conversations.UpdateActivityAsync(activity as Activity, cancellationToken);
        }

        // Delete Message
        public void DeleteActivity(string conversationId, string activityId)
        {
            var client = new GssConnectorClient(new Uri(Activity.ServiceUrl));

            client.Conversations.DeleteActivity(conversationId, activityId);
        }
        public async Task DeleteActivityAsync(string conversationId, string activityId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var client = new GssConnectorClient(new Uri(Activity.ServiceUrl));

            await client.Conversations.DeleteActivityAsync(conversationId, activityId, cancellationToken);
        }

        // Create Conversation
        public ConversationResourceResponse CreateConversation(string channelId, ChannelAccount userAccount, ChannelAccount botAccount, string serviceUrl, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CreateConversation(new Activity()
            {
                From = userAccount,
                Recipient = botAccount,
                ChannelId = channelId,
                ServiceUrl = serviceUrl
            });
        }
        public ConversationResourceResponse CreateConversation(Activity activity, CancellationToken cancellationToken = default(CancellationToken))
        {
            var user = activity.From;
            var bot = activity.Recipient;

            // 建立參數
            var members = new List<ChannelAccount>() { user };
            var conversationParams = new ConversationParameters(bot: bot, members: members, activity: activity);

            // 建立Conversation
            var client = new GssConnectorClient(new Uri(Activity.ServiceUrl));
            var response = client.Conversations.CreateConversation(conversationParams);

            return response;
        }
        public async Task<ConversationResourceResponse> CreateConversationAsync(string channelId, ChannelAccount userAccount, ChannelAccount botAccount, string serviceUrl, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await CreateConversationAsync(new Activity()
            {
                From = userAccount,
                Recipient = botAccount,
                ChannelId = channelId,
                ServiceUrl = serviceUrl
            });
        }
        public async Task<ConversationResourceResponse> CreateConversationAsync(Activity activity, CancellationToken cancellationToken = default(CancellationToken))
        {
            var user = activity.From;
            var bot = activity.Recipient;

            // 建立參數
            var members = new List<ChannelAccount>() { user };
            var conversationParams = new ConversationParameters(bot: bot, members: members, activity: activity);

            // 建立Conversation
            var client = new GssConnectorClient(new Uri(Activity.ServiceUrl));
            var response = await client.Conversations.CreateConversationAsync(conversationParams);

            return response;
        }

        #endregion
    }
}