using System.Collections.Generic;
using Skrbot.Dao;
using Skrbot.Domain.Condition;

namespace Skrbot.Core.Service.Implement
{
    public class TypeService : ITypeService
    {
        private readonly ITypeDao _typeDao;

        public TypeService(ITypeDao typeDao)
        {
            _typeDao = typeDao;
        }

        public IList<Domain.Type> GetList(TypeCondition condition)
        {
            return _typeDao.GetList(condition);
        }

        public IList<Domain.Type> GetFavoriteList(string userId)
        {
            return _typeDao.GetFavoriteList(userId);
        }
    }
}
