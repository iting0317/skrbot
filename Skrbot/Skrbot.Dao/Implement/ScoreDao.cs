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
    public class ScoreDao : BaseDao
    {
        public void InsertOrUpdateScore(string storeNo, string userNo, int score)
        {
            Score thisScore = new Score();
            string sql = @"SELECT STORE_SEQ_NO AS StoreNo, USER_SEQ_NO AS UserNo, SCORE AS ScorePoint
                        FROM [dbo].[SKR_SCORE] WITH(NOLOCK) 
                        WHERE STORE_SEQ_NO = @STORE_SEQ_NO 
                        AND USER_SEQ_NO = @USER_SEQ_NO";
            DynamicParameters paramter = new DynamicParameters();
            paramter.Add("@STORE_SEQ_NO", storeNo);
            paramter.Add("@USER_SEQ_NO", userNo);
            using (this.DbFixture)
            {
                thisScore = this.DbFixture.Db.Connection.Query<Score>(sql, paramter).FirstOrDefault();
            }

            using (this.DbFixture)
            {
                IDbTransaction tran = this.DbFixture.Db.Connection.BeginTransaction();
                try
                {                
                    DynamicParameters insertOrUpdateParamter = new DynamicParameters();
                    string insertOrUpdateSql = "";

                    if (thisScore == null)
                    {
                        insertOrUpdateSql = @"
                            INSERT INTO [dbo].[SKR_SCORE]
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

                        insertOrUpdateParamter.Add("@STORE_SEQ_NO", storeNo);
                        insertOrUpdateParamter.Add("@USER_SEQ_NO", userNo);
                        insertOrUpdateParamter.Add("@CRE_DTE", DateTime.Now);
                        insertOrUpdateParamter.Add("@CRE_USR", "SkrAdmin");
                        insertOrUpdateParamter.Add("@SCORE", 50 + score);
                    }
                    else
                    {
                        thisScore.ScorePoint = thisScore.ScorePoint + score;

                        insertOrUpdateSql = @"
                            UPDATE [dbo].[SKR_SCORE] SET [SCORE] = @SCORE 
                            WHERE STORE_SEQ_NO = @STORE_SEQ_NO AND USER_SEQ_NO = @USER_SEQ_NO
                        ";

                        insertOrUpdateParamter.Add("@STORE_SEQ_NO", thisScore.StoreNo);
                        insertOrUpdateParamter.Add("@USER_SEQ_NO", thisScore.UserNo);
                        insertOrUpdateParamter.Add("@SCORE", thisScore.ScorePoint);
                    }

                    this.DbFixture.Db.Connection.Execute(insertOrUpdateSql, insertOrUpdateParamter, tran);
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
