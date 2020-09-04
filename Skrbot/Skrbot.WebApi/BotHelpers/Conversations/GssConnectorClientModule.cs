using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System;

namespace Skrbot.WebApi.BotHelpers.Conversations
{
    [Serializable]
    public class GssConnectorClientModule : Module
    {
        // Load Module
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c => new GssConnectorClient(new Uri(c.Resolve<IAddress>().ServiceUrl),
                                                                 c.Resolve<MicrosoftAppCredentials>()))
                   .As<IConnectorClient>()
                   .InstancePerMatchingLifetimeScope(typeof(DialogModule));
        }
    }
}