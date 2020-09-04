using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using Skrbot.Domain.Condition;
using Skrbot.Common;
using System.Data;
using Skrbot.Domain;

namespace Skrbot.Dao.Implement
{
    public class RecordDao : BaseDao, IRecordDao
    {
        public int GetCount(RecordCondition condition)
        {
            DynamicParameters parameters = new DynamicParameters();

            StringBuilder fromScript = new StringBuilder();
            fromScript.Append("select count(*) as total from SKR_RECORD_M as r");

            StringBuilder whereScript = new StringBuilder();

            if (condition.CreateDate.HasValue)
            {
                whereScript.Append(" and r.CRE_DTE > @createDate");
                parameters.Add("@createDate", condition.CreateDate.Value);
            }

            if (!string.IsNullOrWhiteSpace(condition.StoreId))
            {
                whereScript.Append(" and r.STORE_SEQ_NO = @storeId");
                parameters.Add("@storeId", condition.StoreId);
            }

            if (!string.IsNullOrWhiteSpace(condition.UserId))
            {
                whereScript.Append(" and r.USER_SEQ_NO = @userId");
                parameters.Add("@userId", condition.UserId);
            }

            string sql = whereScript.Length > 0
                ? $"{fromScript.ToString()} where 1=1 {whereScript.ToString()}"
                : fromScript.ToString();

            int result = 0;

            using (DbFixture)
            {
                try
                {
                    IEnumerable<dynamic> dataRows = DbFixture.Db.Connection.Query(sql, parameters);

                    if (dataRows != null && dataRows.Count() > 0)
                    {
                        IDictionary<string, object> row = dataRows.FirstOrDefault() as IDictionary<string, object>;

                        result = row["total"] as int? ?? 0;
                    }
                }
                catch (Exception exception)
                {
                    Logger.Write(Domain.Enum.EnumLogCategory.Error, exception.ToString());
                }
            }

            return result;
        }

        public IList<Store> GetPositiveRecordNum(IList<Store> stores)
        {
            int i ;
            for (i = 0; i < stores.Count; i++)
            {
                string sql = @"select USER_SEQ_NO from SKR_RECORD_M WITH(NOLOCK)
                                where STORE_SEQ_NO = @STORE_SEQ_NO GROUP BY USER_SEQ_NO";
                DynamicParameters paramter = new DynamicParameters();
                paramter.Add("@STORE_SEQ_NO", stores[i].Id);
                using (this.DbFixture)
                {
                    var storesPositiveList = this.DbFixture.Db.Connection.Query<string>(sql, paramter).ToList();
                    if (storesPositiveList.Count > 0)
                    {
                        stores[i].PositiveRecordList = storesPositiveList;
                    }
                }           
            }
            return stores;
        }

        public void SaveRecord(string storeNo, string userNo, int score)
        {
            using (this.DbFixture)
            {
                IDbTransaction tran = this.DbFixture.Db.Connection.BeginTransaction();
                try
                {
                    string sql = @"
                            INSERT INTO [dbo].[SKR_RECORD_M]
                            ([STORE_SEQ_NO]
                            ,[USER_SEQ_NO]
                            ,[CRE_DTE]
                            ,[CRE_USR]
                            ,[SCORE]
                        )
                        VALUES
                            (@STORE_SEQ_NO
                            ,@USER_SEQ_NO
                            ,@CRE_DTE
                            ,@CRE_USR
                            ,@SCORE
                        )";

                    DynamicParameters Paramter = new DynamicParameters();
                    Paramter.Add("@STORE_SEQ_NO", storeNo);
                    Paramter.Add("@USER_SEQ_NO", userNo);
                    Paramter.Add("@CRE_DTE", DateTime.Now);
                    Paramter.Add("@CRE_USR", "SkrAdmin");
                    Paramter.Add("@SCORE", score);

                    this.DbFixture.Db.Connection.Execute(sql, Paramter, tran);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    Logger.Write(Domain.Enum.EnumLogCategory.Error, ex.ToString());
                }
            }
        }
    }
}
