using Skrbot.Dao;
using Skrbot.Domain.Condition;

namespace Skrbot.Core.Service.Implement
{
    public class FavoriteService : IFavoriteService
    {
        private readonly FavoriteDao _favoriteDao;

        public FavoriteService(FavoriteDao favoriteDao)
        {
            _favoriteDao = favoriteDao;
        }

        public bool SaveUserHateTypeList(FavotiteCondition condition)
        {
            return _favoriteDao.SaveUserHateTypeList(condition);
        }

        public bool SaveUserLikeTypeList(FavotiteCondition condition)
        {
            return _favoriteDao.SaveUserLikeTypeList(condition);
        }

       public bool SaveUserHateList(string storeNo, string userNo)
        {
            return _favoriteDao.SaveUserHateList(storeNo, userNo);
        }

        public bool ResetUserLikeTypeList(string userNo)
        {
            return _favoriteDao.ResetUserLikeTypeList(userNo);
        }

        public bool ResetUserHateTypeList(string userNo)
        {
            return _favoriteDao.ResetUserHateTypeList(userNo);
        }


    }
}
