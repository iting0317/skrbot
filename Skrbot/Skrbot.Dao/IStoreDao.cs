using System.Collections.Generic;
using Skrbot.Domain;
using Skrbot.Domain.Condition;
using Skrbot.Domain.Enum;

namespace Skrbot.Dao
{
    public interface IStoreDao
    {
        IList<Store> GetList(User user, EnumSkrType type);

        IList<Store> GetList(SimpleFilterCondition condition, User user);
    }
}
