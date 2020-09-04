using Skrbot.Domain.Condition;

namespace Skrbot.Core.Service
{
    public interface IFavoriteService
    {
        bool SaveUserHateTypeList(FavotiteCondition condition);

        bool SaveUserLikeTypeList(FavotiteCondition condition);

        bool SaveUserHateList(string storeNo, string userNo);

        bool ResetUserLikeTypeList(string userNo);

        bool ResetUserHateTypeList(string userNo);
    }
}
