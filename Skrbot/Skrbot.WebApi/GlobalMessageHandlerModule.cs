using Autofac;
using Skrbot.WebApi.Scorables;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Scorables;
using Microsoft.Bot.Connector;
using System;

namespace Skrbot.WebApi
{
    [Serializable]
    public class GlobalMessageHandlerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            //TODO: 在此處註冊你的全域訊息處理

            // 註冊全域訊息 - 重置對話
            builder.Register(c => new ResetDialogScorable(c))
                   .As<IScorable<IActivity, double>>()
                   .InstancePerLifetimeScope();

            builder.Register(c => new SkrDialogScorable(c))
                .As<IScorable<IActivity, double>>()
                .InstancePerLifetimeScope();
        }
    }
}