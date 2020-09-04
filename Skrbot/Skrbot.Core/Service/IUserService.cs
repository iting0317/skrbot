using Skrbot.Domain;
using Skrbot.Domain.Condition;

namespace Skrbot.Core.Service
{
    public interface IUserService
    {
        User Get(UserCondition condition);

        User GetOrCreate(UserCondition condition);
    }
}
