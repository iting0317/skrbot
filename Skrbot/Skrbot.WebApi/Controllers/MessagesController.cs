using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using Skrbot.Common;
using Skrbot.Core.Service;
using Skrbot.Domain;
using Skrbot.Domain.Condition;
using Skrbot.Domain.Enum;
using Skrbot.WebApi.BotHelpers.Authentications;
using Skrbot.WebApi.BotHelpers.Conversations;

namespace Skrbot.WebApi.Controllers
{
    [GssBotAuthentication]
    public class MessagesController : ApiController
    {
        // 接收並回覆使用者的訊息
        // POST: api/Messages
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            Logger.Write(EnumLogCategory.Information, $"收到來自 name = {activity.From.Name}, id = {activity.From.Id} 的訊息 {activity.Text}");
            switch (activity.Type)
            {
                case ActivityTypes.Message:
                    // 處理使用者發送的訊息
                    await HandleUserData(activity);
                    await HandleMessage(activity);
                    break;

                case ActivityTypes.ConversationUpdate:
                    // 當Bot已經加入對話、成員加入和離開對話、對話資料更改時
                    await HandleConversationUpdateMessage(activity);
                    break;

                default:
                    await HandleOtherMessage(activity);
                    break;
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private async Task HandleUserData(Activity activity)
        {
            using (ILifetimeScope scope = DialogModule.BeginLifetimeScope(Conversation.Container, activity))
            {
                IAddress address = Address.FromActivity(activity);
                IBotDataStore<BotData> botDataStore = scope.Resolve<IBotDataStore<BotData>>();
                BotData userData = await botDataStore.LoadAsync(address, BotStoreType.BotUserData, CancellationToken.None);
                User user = userData.GetProperty<User>("User");

                if (user == null)
                {
                    IUserService userService = scope.Resolve<IUserService>();
                    UserCondition condition = new UserCondition()
                    {
                        LineUserId = activity.From.Id,
                        UserName = activity.From.Name
                    };
                    user = userService.GetOrCreate(condition);

                    userData.SetProperty("User", user);
                    await botDataStore.SaveAsync(address, BotStoreType.BotUserData, userData, CancellationToken.None);
                    await botDataStore.FlushAsync(address, CancellationToken.None);
                }
            }
        }

        // 當使用者傳送訊息Bot時
        private async Task<Activity> HandleMessage(Activity activity)
        {
            using (ILifetimeScope scope = DialogModule.BeginLifetimeScope(Conversation.Container, activity))
            {
                await Conversation.SendAsync(activity, () => scope.Resolve<IDialog<object>>());
            }
            return activity;
        }

        // 當Bot已經加入對話、成員加入和離開對話、對話資料更改時
        private async Task<Activity> HandleConversationUpdateMessage(Activity activity)
        {
            if (activity.MembersAdded.Any(o => o.Id == activity.From.Id))
            {
                var context = BotDialogContext.Create(activity);
                await context.ReplyMessageAsync("哈囉!");
            }
            return activity;
        }

        // 處理其他類型的訊息
        private async Task<Activity> HandleOtherMessage(Activity activity)
        {
            switch (activity.Type)
            {
                case ActivityTypes.DeleteUserData:
                    break;

                case ActivityTypes.ContactRelationUpdate:
                    // 當使用者將Bot加為聯絡人、從聯絡人中刪除Bot時
                    break;

                case ActivityTypes.Typing:
                    // 當使用者正在打字時
                    break;

                case ActivityTypes.Ping:
                    // 當使用者正在Ping Bot時
                    break;

                case ActivityTypes.EndOfConversation:
                    // 當對話結束時
                    break;
            }

            return null;
        }
    }
}