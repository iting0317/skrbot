using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using Skrbot.Common;
using Skrbot.Domain;
using Skrbot.Domain.Condition;

namespace Skrbot.Dao.Implement
{
    public class UserDao : BaseDao, IUserDao
    {
        public User GetById(string id)
        {
            DynamicParameters parameters = new DynamicParameters();

            StringBuilder builder = new StringBuilder();
            builder.Append("select * from SKR_USER_M as u where u.USER_SEQ_NO = @id");

            parameters.Add("@id", id);

            User result = null;

            using (DbFixture)
            {
                try
                {
                    IEnumerable<dynamic> dataRows = DbFixture.Db.Connection.Query(builder.ToString(), parameters);

                    if (dataRows != null && dataRows.Count() > 0)
                    {
                        IDictionary<string, object> row = dataRows.FirstOrDefault() as IDictionary<string, object>;

                        result = new User()
                        {
                            Id = row["USER_SEQ_NO"] as string,
                            Name = row["USER_LINE_NAME"] as string,
                            LineUserId = row["USER_LINE_ID"] as string,
                            CreateDate = row["CRE_DTE"] as DateTime?
                        };
                    }
                }
                catch (Exception exception)
                {
                    Logger.Write(Domain.Enum.EnumLogCategory.Error, exception.ToString());
                }
            }

            return result;
        }

        public User Get(UserCondition condition)
        {
            DynamicParameters parameters = new DynamicParameters();

            StringBuilder builder = new StringBuilder();
            builder.Append("select * from SKR_USER_M as u");

            if (!string.IsNullOrWhiteSpace(condition.LineUserId))
            {
                builder.Append(" where u.USER_LINE_ID = @lineUserId");
                parameters.Add("@lineUserId", condition.LineUserId);
            }

            User result = null;

            using (DbFixture)
            {
                try
                {
                    IEnumerable<dynamic> dataRows = DbFixture.Db.Connection.Query(builder.ToString(), parameters);

                    if (dataRows != null && dataRows.Count() > 0)
                    {
                        IDictionary<string, object> row = dataRows.FirstOrDefault() as IDictionary<string, object>;

                        result = new User()
                        {
                            Id = row["USER_SEQ_NO"] as string,
                            Name = row["USER_LINE_NAME"] as string,
                            LineUserId = row["USER_LINE_ID"] as string,
                            CreateDate = row["CRE_DTE"] as DateTime?
                        };
                    }
                }
                catch (Exception exception)
                {
                    Logger.Write(Domain.Enum.EnumLogCategory.Error, exception.ToString());
                }
            }

            return result;
        }

        public User Create(string lineUserId, string userName)
        {
            DynamicParameters parameters = new DynamicParameters();
            string id = Guid.NewGuid().ToString();

            StringBuilder builder = new StringBuilder();
            builder.Append("insert into SKR_USER_M (USER_SEQ_NO, USER_LINE_ID, USER_LINE_NAME, CRE_DTE) values (@id, @lineUserId, @userName, @createDate)");

            parameters.Add("@id", id);
            parameters.Add("@lineUserId", lineUserId);
            parameters.Add("@userName", userName);
            parameters.Add("@createDate", DateTime.UtcNow);

            User result = null;

            using (DbFixture)
            {
                IDbTransaction transaction = DbFixture.Db.Connection.BeginTransaction();

                try
                {
                    int insertResult = DbFixture.Db.Connection.Execute(builder.ToString(), parameters, transaction);

                    if (insertResult > 0)
                    {
                        transaction.Commit();

                        result = GetById(id);
                    }
                }
                catch (Exception exception)
                {
                    transaction.Rollback();

                    Logger.Write(Domain.Enum.EnumLogCategory.Error, exception.ToString());
                }
            }

            return result;
        }
    }
}
