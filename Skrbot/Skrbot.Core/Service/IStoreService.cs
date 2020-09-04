using System.Collections.Generic;
using Skrbot.Domain;
using Skrbot.Domain.Condition;
using Skrbot.Domain.Enum;
using Skrbot.Domain.ServiceModel;

namespace Skrbot.Core.Service
{
    public interface IStoreService
    {
        IList<ScoreStore> GetRandomList(User user, EnumSkrType type);

        IList<ScoreStore> GetList(SimpleFilterCondition condition, User user);

        void SaveRecord(string storeNo, string userNo, int score);

        void InsertOrUpdateScore(string storeNo, string userNo, int score);
    }
}
