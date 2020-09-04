using Skrbot.Domain;
using Skrbot.Domain.Condition;

namespace Skrbot.Dao
{
    public interface IUserDao
    {
        User GetById(string id);

        User Get(UserCondition condition);

        User Create(string lineUserId, string userName);
    }
}
