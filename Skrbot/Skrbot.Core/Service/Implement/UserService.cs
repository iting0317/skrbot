using Skrbot.Dao;
using Skrbot.Domain;
using Skrbot.Domain.Condition;

namespace Skrbot.Core.Service.Implement
{
    public class UserService : IUserService
    {
        private readonly IUserDao _userDao;

        public UserService(IUserDao userDao)
        {
            _userDao = userDao;
        }

        public User Get(UserCondition condition)
        {
            return _userDao.Get(condition);
        }

        public User GetOrCreate(UserCondition condition)
        {
            User user = Get(condition);

            if (user == null)
            {
                user = _userDao.Create(condition.LineUserId, condition.UserName);
            }

            return user;
        }
    }
}
