using Skrbot.Domain.Condition;
using Skrbot.Domain;
using System.Collections.Generic;

namespace Skrbot.Dao
{
    public interface IRecordDao
    {
        int GetCount(RecordCondition condition);

        IList<Store> GetPositiveRecordNum(IList<Store> stores);
    }
}
