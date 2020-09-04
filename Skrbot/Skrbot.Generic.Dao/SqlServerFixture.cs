using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions.Mapper;
using DapperExtensions.Sql;
using Skrbot.Common;


namespace Skrbot.Generic.Dao
{
    public class SqlServerFixture : IDisposable
    {
        private string connectionID;
        private DapperExtensions.IDatabase _Db = null; 
        /// <summary>
        /// ConnectionString 內定義的連線字串ID
        /// </summary>
        /// <param name="connectionID"></param>
        public SqlServerFixture(string connectionID)
        {
            this.connectionID = connectionID;
        }
        /// <summary>
        /// 取得DapperExtensions物件(並建立SQL Server Connection)
        /// </summary>
        public DapperExtensions.IDatabase Db
        {
            get
            {
                if (this._Db == null)
                {
                    //SqlConnection Connection = new SqlConnection(ConfigManager.GetConnectionString(this.connectionID));
                    SqlConnection Connection = new SqlConnection(ConfigManager.GetConnectionString(this.connectionID));
                    var config = new DapperExtensions.DapperExtensionsConfiguration(typeof(DapperExtensions.Mapper.AutoClassMapper<>), new List<Assembly>(), new SqlServerDialect());
                    var sqlGenerator = new SqlGeneratorImpl(config);
                    this._Db = new DapperExtensions.Database(Connection, sqlGenerator);
                    return this._Db;
                }
                else
                {
                    if (this._Db.Connection.State == System.Data.ConnectionState.Closed)
                    {
                        this._Db.Connection.Open();
                    }
                    return this._Db;
                }
            }
        }

        public void Dispose()
        {
            if (_Db == null)
            {
                //Gss.Sys.Common.Logger.Write(LogCategoryEnum.Error, "_Db NULL");
             
            }
            else
            {
                this._Db.Dispose();
            }

        }
    }
}
