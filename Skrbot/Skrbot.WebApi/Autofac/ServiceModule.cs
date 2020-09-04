using Autofac;
using Microsoft.Bot.Builder.Internals.Fibers;
using Skrbot.Core.Service;
using Skrbot.Core.Service.Implement;

namespace Skrbot.WebApi.Autofac
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<StoreService>()
                .Keyed<IStoreService>(FiberModule.Key_DoNotSerialize)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<FavoriteService>()
                .Keyed<IFavoriteService>(FiberModule.Key_DoNotSerialize)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<TypeService>()
                .Keyed<ITypeService>(FiberModule.Key_DoNotSerialize)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<UserService>()
                .Keyed<IUserService>(FiberModule.Key_DoNotSerialize)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}
