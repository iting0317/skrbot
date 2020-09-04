using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skrbot.Generic.Dao;
using System.Data;
using Skrbot.Domain.Condition;
using Dapper;
using DapperExtensions;
using Skrbot.Common;

namespace Skrbot.Dao
{
    public class FavoriteDao : BaseDao
    {
        public List<string> LoadMealTypeStringList(List<string> MealTypeStringList)
        {
            List<string> result = new List<string>();
            List<string> value = new List<string>();
            int i;
            for (i = 0; i < MealTypeStringList.Count(); i++)
            {
                string sql = "SELECT MEAL_TYPE_NAME FROM [dbo].[SKR_TYPE_M] WITH(NOLOCK) WHERE MEAL_TYPE_NAME LIKE @MealTypeName";
                DynamicParameters paramter = new DynamicParameters();
                paramter.Add("@MealTypeName", '%' + MealTypeStringList[i] + '%');
                using (this.DbFixture)
                {
                    value = this.DbFixture.Db.Connection.Query<string>(sql, paramter).ToList();
                }
                if (value.Count() > 0)
                {
                    result.Add(value[0]);
                }
            }         
            return result;
        }

        public List<string> LoadMealTypeList(List<string> MealTypeStringList)
        {
            List<string> result = new List<string>();
            List<string> value = new List<string>();
            int i;
            for (i = 0; i < MealTypeStringList.Count(); i++)
            {
                string sql = "SELECT TYPE_SEQ_NO FROM [dbo].[SKR_TYPE_M] WITH(NOLOCK) WHERE MEAL_TYPE_NAME LIKE @MealTypeName";
                DynamicParameters paramter = new DynamicParameters();
                paramter.Add("@MealTypeName", '%' + MealTypeStringList[i] + '%');
                using (this.DbFixture)
                {
                    value = this.DbFixture.Db.Connection.Query<string>(sql, paramter).ToList();
                }
                if (value.Count() > 0)
                {
                    result.Add(value[0]);
                }
            }
            return result;
        }


        public bool SaveUserHateTypeList(FavotiteCondition condition)
        {
            using (this.DbFixture)
            {
                IDbTransaction tran = this.DbFixture.Db.Connection.BeginTransaction();
                try
                {
                    int i;
                    for (i = 0; i < condition.HateType.Count(); i++)
                    { 
                        string sql = @"
                            INSERT INTO [dbo].[SKR_TYPE_HATELIST_M]
                            ([TYPE_SEQ_NO]
                            ,[USER_SEQ_NO]
                            ,[CRE_DTE]
                            ,[CRE_USR]
                        )
                        VALUES
                            (@TYPE_SEQ_NO
                            ,@USER_SEQ_NO
                            ,@CRE_DTE
                            ,@CRE_USR
                        )";

                        DynamicParameters Paramter = new DynamicParameters();
                        Paramter.Add("@TYPE_SEQ_NO", condition.HateType[i]);
                        Paramter.Add("@USER_SEQ_NO", condition.UserSeqNum);
                        Paramter.Add("@CRE_DTE", DateTime.Now);
                        Paramter.Add("@CRE_USR", "SkrAdmin");

                        this.DbFixture.Db.Connection.Execute(sql, Paramter, tran);
                    }
                    tran.Commit();
                    return  true;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    Logger.Write(Domain.Enum.EnumLogCategory.Error, ex.ToString());
                    return false;
                }
            }
        }

        public bool SaveUserLikeTypeList(FavotiteCondition condition)
        {
            using (this.DbFixture)
            {
                IDbTransaction tran = this.DbFixture.Db.Connection.BeginTransaction();
                try
                {
                    int i;
                    for (i = 0; i < condition.LikeType.Count(); i++)
                    {
                        string sql = @"
                            INSERT INTO [dbo].[SKR_TYPE_LIKELIST_M]
                            ([TYPE_SEQ_NO]
                            ,[USER_SEQ_NO]
                            ,[CRE_DTE]
                            ,[CRE_USR]
                        )
                        VALUES
                            (@TYPE_SEQ_NO
                            ,@USER_SEQ_NO
                            ,@CRE_DTE
                            ,@CRE_USR
                        )";

                        DynamicParameters Paramter = new DynamicParameters();
                        Paramter.Add("@TYPE_SEQ_NO", condition.LikeType[i]);
                        Paramter.Add("@USER_SEQ_NO", condition.UserSeqNum);
                        Paramter.Add("@CRE_DTE", DateTime.Now);
                        Paramter.Add("@CRE_USR", "SkrAdmin");

                        this.DbFixture.Db.Connection.Execute(sql, Paramter, tran);
                    }
                    tran.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    Logger.Write(Domain.Enum.EnumLogCategory.Error, ex.ToString());
                    return false;
                }
            }
        }

        public bool SaveUserHateList(string storeNo, string userNo)
        {
            using (this.DbFixture)
            {
                IDbTransaction tran = this.DbFixture.Db.Connection.BeginTransaction();
                try
                {
                    string sql = @"
                            INSERT INTO [dbo].[SKR_HATELIST_M]
                            ([STORE_SEQ_NO]
                            ,[USER_SEQ_NO]
                            ,[CRE_DTE]
                            ,[CRE_USR]
                        )
                        VALUES
                            (@STORE_SEQ_NO
                            ,@USER_SEQ_NO
                            ,@CRE_DTE
                            ,@CRE_USR
                        )";

                    DynamicParameters Paramter = new DynamicParameters();
                    Paramter.Add("@STORE_SEQ_NO", storeNo);
                    Paramter.Add("@USER_SEQ_NO", userNo);
                    Paramter.Add("@CRE_DTE", DateTime.Now);
                    Paramter.Add("@CRE_USR", "SkrAdmin");

                    this.DbFixture.Db.Connection.Execute(sql, Paramter, tran);
                    tran.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    Logger.Write(Domain.Enum.EnumLogCategory.Error, ex.ToString());
                    return false;
                }
            }
        }

        public bool ResetUserHateTypeList(string userNo)
        {
            using (this.DbFixture)
            {
                IDbTransaction tran = this.DbFixture.Db.Connection.BeginTransaction();
                try
                {
                    string sql = @"
                           DELETE FROM  [dbo].[SKR_TYPE_HATELIST_M] WHERE  USER_SEQ_NO = @USER_SEQ_NO
                        ";

                    DynamicParameters Paramter = new DynamicParameters();
                    Paramter.Add("@USER_SEQ_NO", userNo);

                    this.DbFixture.Db.Connection.Execute(sql, Paramter, tran);
                    tran.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    Logger.Write(Domain.Enum.EnumLogCategory.Error, ex.ToString());
                    return false;
                }
            }
        }

        public bool ResetUserLikeTypeList(string userNo)
        {
            using (this.DbFixture)
            {
                IDbTransaction tran = this.DbFixture.Db.Connection.BeginTransaction();
                try
                {
                    string sql = @"
                           DELETE FROM  [dbo].[SKR_TYPE_LIKELIST_M] WHERE  USER_SEQ_NO = @USER_SEQ_NO
                        ";

                    DynamicParameters Paramter = new DynamicParameters();
                    Paramter.Add("@USER_SEQ_NO", userNo);

                    this.DbFixture.Db.Connection.Execute(sql, Paramter, tran);
                    tran.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    Logger.Write(Domain.Enum.EnumLogCategory.Error, ex.ToString());
                    return false;
                }
            }
        }

        public List<string> TestDao(FavotiteCondition condition)
        {
            string sql = "select STORE_NAME from SKR_STORE_M with(nolock) where STORE_PRICE = 3 ";

            DynamicParameters paramter = new DynamicParameters();
            List<string> result = new List<string>();
            using (this.DbFixture)
            {
                result = this.DbFixture.Db.Connection.Query<string>(sql, paramter).ToList();
            }
            return result;
        }
    }
}
