using Autofac;
using Skrbot.Dao;
using Skrbot.Dao.Implement;

namespace Skrbot.WebApi.Autofac
{
    public class DaoModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FavoriteDao>()
                .SingleInstance();

            builder.RegisterType<RecordDao>()
                .As<IRecordDao>()
                .SingleInstance();

            builder.RegisterType<StoreDao>()
                .As<IStoreDao>()
                .SingleInstance();

            builder.RegisterType<UserDao>()
                .As<IUserDao>()
                .SingleInstance();
            
            builder.RegisterType<TypeDao>()
                .As<ITypeDao>()
                .SingleInstance();
        }
    }
}
