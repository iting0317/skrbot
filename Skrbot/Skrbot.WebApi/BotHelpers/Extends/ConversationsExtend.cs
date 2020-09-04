using Microsoft.Bot.Connector;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Skrbot.WebApi.BotHelpers.Extends
{
    [Serializable]
    public static class ConversationsExtend
    {
        // Create Conversaion
        // POST "v3/conversations"
        public static ConversationResourceResponse CreateConversation(this IConversations operations, ConversationParameters parameters, Dictionary<string, List<string>> customHeaders)
        {
            return operations.CreateConversationAsync(parameters, customHeaders).GetAwaiter().GetResult();
        }
        public static async Task<ConversationResourceResponse> CreateConversationAsync(this IConversations operations, ConversationParameters parameters, Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _result = await operations.CreateConversationWithHttpMessagesAsync(parameters, customHeaders, cancellationToken).ConfigureAwait(false))
            {
                return _result.Body;
            }
        }


        // Send To Conversation
        // POST "v3/conversations/{conversationId}/activities"
        public static ResourceResponse SendToConversation(this IConversations operations, Activity activity, Dictionary<string, List<string>> customHeaders)
        {
            return Task.Factory.StartNew(s => ((IConversations)s).SendToConversationAsync(activity.Conversation.Id, activity, customHeaders), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }
        public static ResourceResponse SendToConversation(this IConversations operations, string conversationId, Activity activity, Dictionary<string, List<string>> customHeaders)
        {
            return operations.SendToConversationAsync(conversationId, activity, customHeaders).GetAwaiter().GetResult();
        }
        public static Task<ResourceResponse> SendToConversationAsync(this IConversations operations, Activity activity, Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken = default(CancellationToken))
        {
            return operations.SendToConversationAsync(activity.Conversation.Id, activity, customHeaders, cancellationToken);
        }
        public static async Task<ResourceResponse> SendToConversationAsync(this IConversations operations, string conversationId, Activity activity, Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _result = await operations.SendToConversationWithHttpMessagesAsync(conversationId, activity, customHeaders, cancellationToken).ConfigureAwait(false))
            {
                return _result.Body;
            }
        }


        // Reply To Activity
        // Post "v3/conversations/{conversationId}/activities/{activityId}"
        public static ResourceResponse ReplyToActivity(this IConversations operations, Activity activity, Dictionary<string, List<string>> customHeaders)
        {
            return Task.Factory.StartNew(s => ((IConversations)s).ReplyToActivityAsync(activity.Conversation.Id, activity.ReplyToId, activity, customHeaders), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }
        public static ResourceResponse ReplyToActivity(this IConversations operations, string conversationId, string activityId, Activity activity, Dictionary<string, List<string>> customHeaders)
        {
            return operations.ReplyToActivityAsync(conversationId, activityId, activity, customHeaders).GetAwaiter().GetResult();
        }
        public static Task<ResourceResponse> ReplyToActivityAsync(this IConversations operations, Activity activity, Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (activity.ReplyToId == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "ReplyToId");
            }

            return operations.ReplyToActivityAsync(activity.Conversation.Id, activity.ReplyToId, activity, customHeaders, cancellationToken);
        }
        public static async Task<ResourceResponse> ReplyToActivityAsync(this IConversations operations, string conversationId, string activityId, Activity activity, Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _result = await operations.ReplyToActivityWithHttpMessagesAsync(conversationId, activityId, activity, customHeaders, cancellationToken).ConfigureAwait(false))
            {
                return _result.Body;
            }
        }

        // Update Activity
        // POST "v3/conversations/{conversationId}/activities/{activityId}"
        public static ResourceResponse UpdateActivity(this IConversations operations, Activity activity, Dictionary<string, List<string>> customHeaders)
        {
            return Task.Factory.StartNew(s => ((IConversations)s).UpdateActivityAsync(activity.Conversation.Id, activity.Id, activity, customHeaders), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }
        public static ResourceResponse UpdateActivity(this IConversations operations, string conversationId, string activityId, Activity activity, Dictionary<string, List<string>> customHeaders)
        {
            return operations.UpdateActivityAsync(conversationId, activityId, activity, customHeaders).GetAwaiter().GetResult();
        }
        public static Task<ResourceResponse> UpdateActivityAsync(this IConversations operations, Activity activity, Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken = default(CancellationToken))
        {
            return operations.UpdateActivityAsync(activity.Conversation.Id, activity.Id, activity, customHeaders, cancellationToken);
        }
        public static async Task<ResourceResponse> UpdateActivityAsync(this IConversations operations, string conversationId, string activityId, Activity activity, Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _result = await operations.UpdateActivityWithHttpMessagesAsync(conversationId, activityId, activity, customHeaders, cancellationToken).ConfigureAwait(false))
            {
                return _result.Body;
            }
        }


        public static void DeleteActivity(this IConversations operations, string conversationId, string activityId, Dictionary<string, List<string>> customHeaders)
        {
            operations.DeleteActivityAsync(conversationId, activityId, customHeaders).GetAwaiter().GetResult();
        }
        public static async Task DeleteActivityAsync(this IConversations operations, string conversationId, string activityId, Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken = default(CancellationToken))
        {
            (await operations.DeleteActivityWithHttpMessagesAsync(conversationId, activityId, customHeaders, cancellationToken).ConfigureAwait(false)).Dispose();
        }


        public static ResourceResponse UploadAttachment(this IConversations operations, string conversationId, AttachmentData attachmentUpload, Dictionary<string, List<string>> customHeaders)
        {
            return operations.UploadAttachmentAsync(conversationId, attachmentUpload, customHeaders).GetAwaiter().GetResult();
        }
        public static async Task<ResourceResponse> UploadAttachmentAsync(this IConversations operations, string conversationId, AttachmentData attachmentUpload, Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _result = await operations.UploadAttachmentWithHttpMessagesAsync(conversationId, attachmentUpload, customHeaders, cancellationToken).ConfigureAwait(false))
            {
                return _result.Body;
            }
        }

        public static IList<ChannelAccount> GetConversationMembers(this IConversations operations, string conversationId, Dictionary<string, List<string>> customHeaders)
        {
            return operations.GetConversationMembersAsync(conversationId, customHeaders).GetAwaiter().GetResult();
        }
        public static async Task<IList<ChannelAccount>> GetConversationMembersAsync(this IConversations operations, string conversationId, Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _result = await operations.GetConversationMembersWithHttpMessagesAsync(conversationId, customHeaders, cancellationToken).ConfigureAwait(false))
            {
                return _result.Body;
            }
        }
    }
}