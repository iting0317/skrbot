using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skrbot.Common
{
    public static class ConfigManager
    {
        /// <summary>
        /// 取得Config檔內的AppSetting
        /// </summary>
        /// <param name="Key">Key Value</param>
        /// <returns></returns>
        public static string GetAppSetting(string Key)
        {
            string AppSetting = string.Empty;
            AppSetting = System.Configuration.ConfigurationManager.AppSettings[Key];

            return AppSetting;
        }

        /// <summary>
        /// 取得Config檔內的DB 連線字串
        /// </summary>
        /// <param name="DB">DB ID</param>
        /// <returns></returns>
        public static string GetConnectionString(string DB)
        {

            string ConnectionString;
            ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings[DB].ConnectionString.ToString();

            //組合成連線字串
            string conn_server = GetAppSetting("conn_server");
            string conn_db = GetAppSetting("conn_db");
            string conn_user = GetAppSetting("conn_user");
            string conn_security = GetAppSetting("conn_security");
            ConnectionString = string.Format(ConnectionString, conn_server, conn_db, conn_user, conn_security);
            
            //if (Variable.IsHashConnectionString == true)
            //{
            //    ConnectionString = DBConnectionTool.GetDESDecryptConnectionString(ConnectionString);
            //}

            return ConnectionString;
        }

    }
}
