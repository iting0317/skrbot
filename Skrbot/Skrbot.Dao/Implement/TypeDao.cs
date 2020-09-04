using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using Skrbot.Common;
using Skrbot.Domain.Condition;

namespace Skrbot.Dao.Implement
{
    public class TypeDao : BaseDao, ITypeDao
    {
        public IList<Domain.Type> GetList(TypeCondition condition)
        {
            DynamicParameters parameters = new DynamicParameters();

            StringBuilder fromScript = new StringBuilder();
            fromScript.Append("select * from SKR_TYPE_M as t");

            StringBuilder whereScript = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(condition.Name))
            {
                whereScript.Append(" and t.MEAL_TYPE_NAME > @name");
                parameters.Add("@name", condition.Name);
            }

            string sql = whereScript.Length > 0
                ? $"{fromScript.ToString()} where 1=1 {whereScript.ToString()}"
                : fromScript.ToString();

            IList<Domain.Type> result = new List<Domain.Type>();

            using (DbFixture)
            {
                try
                {
                    IEnumerable<dynamic> dataRows = DbFixture.Db.Connection.Query(sql, parameters);

                    if (dataRows != null && dataRows.Count() > 0)
                    {
                        result = dataRows.ToList()
                            .Select(r =>
                            {
                                IDictionary<string, object> row = r as IDictionary<string, object>;

                                return new Domain.Type()
                                {
                                    Id = row["TYPE_SEQ_NO"] as string,
                                    Name = row["MEAL_TYPE_NAME"] as string,
                                    CreateDate = row["CRE_DTE"] as DateTime?,
                                    CreateUser = row["CRE_USR"] as string
                                };
                            })
                            .ToList();
                    }
                }
                catch (Exception exception)
                {
                    Logger.Write(Domain.Enum.EnumLogCategory.Error, exception.ToString());
                }
            }

            return result;
        }

        public IList<Domain.Type> GetFavoriteList(string userId)
        {
            DynamicParameters parameters = new DynamicParameters();

            StringBuilder fromScript = new StringBuilder();
            fromScript.Append("select t.* from SKR_TYPE_M as t");

            StringBuilder whereScript = new StringBuilder();

            whereScript.Append("and t.TYPE_SEQ_NO in (select tl.TYPE_SEQ_NO from SKR_TYPE_LIKELIST_M as tl where tl.USER_SEQ_NO = @userId)");
            parameters.Add("@userId", userId);

            string sql = $"{fromScript.ToString()} where 1=1 {whereScript.ToString()}";

            IList<Domain.Type> result = new List<Domain.Type>();

            using (DbFixture)
            {
                try
                {
                    IEnumerable<dynamic> dataRows = DbFixture.Db.Connection.Query(sql, parameters);

                    if (dataRows != null && dataRows.Count() > 0)
                    {
                        result = dataRows.ToList()
                            .Select(r =>
                            {
                                IDictionary<string, object> row = r as IDictionary<string, object>;

                                return new Domain.Type()
                                {
                                    Id = row["TYPE_SEQ_NO"] as string,
                                    Name = row["MEAL_TYPE_NAME"] as string,
                                    CreateDate = row["CRE_DTE"] as DateTime?,
                                    CreateUser = row["CRE_USR"] as string
                                };
                            })
                            .ToList();
                    }
                }
                catch (Exception exception)
                {
                    Logger.Write(Domain.Enum.EnumLogCategory.Error, exception.ToString());
                }
            }

            return result;
        }
    }
}
