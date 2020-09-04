using System.Collections.Generic;
using Skrbot.Domain;
using Skrbot.Domain.Condition;

namespace Skrbot.Core.Service
{
    public interface ITypeService
    {
        IList<Type> GetList(TypeCondition condition);

        IList<Type> GetFavoriteList(string userId);
    }
}
