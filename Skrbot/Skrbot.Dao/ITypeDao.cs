using System.Collections.Generic;
using Skrbot.Domain;
using Skrbot.Domain.Condition;

namespace Skrbot.Dao
{
    public interface ITypeDao
    {
        IList<Type> GetList(TypeCondition condition);

        IList<Type> GetFavoriteList(string userId);
    }
}
