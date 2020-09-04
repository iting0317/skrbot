using System.Web;
using System.Web.Http;
using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Skrbot.WebApi.Autofac;
using Skrbot.WebApi.BotHelpers.Conversations;
using Skrbot.WebApi.BotHelpers.DataStores;
using Skrbot.WebApi.Dialogs;

namespace Skrbot.WebApi
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            // Update Coversation Container
            Conversation.UpdateContainer(builder =>
            {
                // 選擇Bot State儲存的位置
                var botStoreModule = new BotDataStoreModule().UseInMemoryDataStore();

                // 註冊模組 - Bot State儲存的位置
                builder.RegisterModule(botStoreModule);

                // 註冊模組 - ConnectorClient加入GssBotAppAccessToken
                builder.RegisterModule(new GssConnectorClientModule());

                // 註冊模組 - 全域訊息處理
                builder.RegisterModule(new ReflectionSurrogateModule());
                builder.RegisterModule<GlobalMessageHandlerModule>();
                builder.RegisterModule<ServiceModule>();
                builder.RegisterModule<DaoModule>();

                builder.RegisterType<RootDialog>()
                    .As<IDialog<object>>()
                    .InstancePerDependency();

            });

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
