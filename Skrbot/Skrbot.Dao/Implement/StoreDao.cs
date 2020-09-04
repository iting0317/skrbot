using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using Skrbot.Common;
using Skrbot.Domain;
using Skrbot.Domain.Condition;
using Skrbot.Domain.Enum;

namespace Skrbot.Dao.Implement
{
    public class StoreDao : BaseDao, IStoreDao
    {
        public IList<Store> GetList(User user, EnumSkrType type)
        {
            DynamicParameters parameters = new DynamicParameters();

            StringBuilder fromScript = new StringBuilder();
            fromScript.Append("select s.*, ss.SCORE as SCORE from SKR_STORE_M as s");
            fromScript.Append(" left join SKR_SCORE as ss on s.STORE_SEQ_NO = ss.STORE_SEQ_NO and ss.USER_SEQ_NO = @userId");
            parameters.Add("@userId", user.Id);

            StringBuilder whereScript = new StringBuilder();

            // Not in hate list
            whereScript.Append(" and s.STORE_SEQ_NO in (" +
                "select st.STORE_SEQ_NO from SKR_STORETYPE_M as st" +
                " where st.TYPE_SEQ_NO not in (select h.TYPE_SEQ_NO from SKR_TYPE_HATELIST_M as h where h.USER_SEQ_NO = @hUserId))");
            parameters.Add("@hUserId", user.Id);

            // Exclude stores had be eaten
            if (type == EnumSkrType.NotEatenYetRandom)
            {
                whereScript.Append(" and s.STORE_SEQ_NO not in (" +
                    "select r.STORE_SEQ_NO from SKR_RECORD_M as r" +
                    " where r.USER_SEQ_NO = @rUserId)");
                parameters.Add("@rUserId", user.Id);
            }

            string sql = $"{fromScript.ToString()} where 1=1 {whereScript.ToString()}";

            IList<Store> result = new List<Store>();

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

                                return new Store()
                                {
                                    Id = row["STORE_SEQ_NO"] as string,
                                    Name = row["STORE_NAME"] as string,
                                    Price = Convert.ToDouble(row["STORE_PRICE"]),
                                    Address = row["STORE_ADDRESS"] as string,
                                    Latitude = Convert.ToDouble(row["LOCATION_LAT"]),
                                    Longitude = Convert.ToDouble(row["LOCATION_LONG"]),
                                    Distance = Convert.ToDouble(row["DISTANCE"]),
                                    Seat = row["SEAT"] as string,
                                    Url = row["STORE_URL"] as string,
                                    CreateDate = row["CRE_DTE"] as DateTime?,
                                    CreateUser = row["CRE_USR"] as string,
                                    Score = Convert.ToDouble(row["SCORE"]),
                                    StoreUrl = row["STORE_URL"] as string,
                                    PictureUrl = row["PICTURE_URL"] as string,
                                    PictureType = row["PICTURE_TYPE"] as string
                                };
                            })
                            .ToList();
                    }
                }
                catch (Exception exception)
                {
                    Logger.Write(EnumLogCategory.Error, exception.ToString());
                }
            }

            return result;
        }

        public IList<Store> GetList(SimpleFilterCondition condition, User user)
        {
            DynamicParameters parameters = new DynamicParameters();

            StringBuilder fromScript = new StringBuilder();
            fromScript.Append("select s.*, ss.SCORE as SCORE from SKR_STORE_M as s");
            fromScript.Append(" left join SKR_SCORE as ss on s.STORE_SEQ_NO = ss.STORE_SEQ_NO and ss.USER_SEQ_NO = @userId");
            parameters.Add("@userId", user.Id);

            StringBuilder whereScript = new StringBuilder();

            // In selected types and not in hate list
            if (condition.Types.Count > 0)
            {
                whereScript.Append(" and s.STORE_SEQ_NO in (" +
                    "select st.STORE_SEQ_NO from SKR_STORETYPE_M as st" +
                    " where st.TYPE_SEQ_NO in @typeIds" +
                    " and st.TYPE_SEQ_NO not in (select h.TYPE_SEQ_NO from SKR_TYPE_HATELIST_M as h where h.USER_SEQ_NO = @hUserId))");
                parameters.Add("@typeIds", condition.Types.Select(t => t.Id));
                parameters.Add("@hUserId", user.Id);
            }

            switch (condition.PriceRange)
            {
                case EnumPriceRange.Cheapest:
                case EnumPriceRange.Low:
                    whereScript.Append(" and s.STORE_PRICE <= @priceLevel");
                    parameters.Add("@priceLevel", 7);
                    break;
                case EnumPriceRange.Midddle:
                case EnumPriceRange.High:
                    whereScript.Append(" and s.STORE_PRICE > @lowPriceLevel and s.STORE_PRICE <= @highPriceLevel");
                    parameters.Add("@lowPriceLevel", 3);
                    parameters.Add("@highPriceLevel", 15);
                    break;
                case EnumPriceRange.MostExpensive:
                    whereScript.Append(" and s.STORE_PRICE > @priceLevel");
                    parameters.Add("@priceLevel", 15);
                    break;
            }

            switch (condition.Distance)
            {
                case EnumDistance.Near:
                    whereScript.Append(" and s.DISTANCE <= @distance");
                    parameters.Add("@distance", 250);
                    break;
                case EnumDistance.Middle:
                    whereScript.Append(" and s.DISTANCE > @lowDistance and s.DISTANCE <= @farDistance");
                    parameters.Add("@lowDistance", 250);
                    parameters.Add("@farDistance", 500);
                    break;
                case EnumDistance.Far:
                    whereScript.Append(" and s.DISTANCE > @distance");
                    parameters.Add("@distance", 500);
                    break;
            }

            string sql = whereScript.Length > 0
                ? $"{fromScript.ToString()} where 1=1 {whereScript.ToString()}"
                : fromScript.ToString();

            IList<Store> result = new List<Store>();

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

                                return new Store()
                                {
                                    Id = row["STORE_SEQ_NO"] as string,
                                    Name = row["STORE_NAME"] as string,
                                    Price = Convert.ToDouble(row["STORE_PRICE"]),
                                    Address = row["STORE_ADDRESS"] as string,
                                    Latitude = Convert.ToDouble(row["LOCATION_LAT"]),
                                    Longitude = Convert.ToDouble(row["LOCATION_LONG"]),
                                    Distance = Convert.ToDouble(row["DISTANCE"]),
                                    Seat = row["SEAT"] as string,
                                    Url = row["STORE_URL"] as string,
                                    CreateDate = row["CRE_DTE"] as DateTime?,
                                    CreateUser = row["CRE_USR"] as string,
                                    Score = Convert.ToDouble(row["SCORE"]),
                                    StoreUrl = row["STORE_URL"] as string,
                                    PictureUrl = row["PICTURE_URL"] as string,
                                    PictureType = row["PICTURE_TYPE"] as string
                                };
                            })
                            .ToList();
                    }
                }
                catch (Exception exception)
                {
                    Logger.Write(EnumLogCategory.Error, exception.ToString());
                }
            }

            return result;
        }
    }
}
