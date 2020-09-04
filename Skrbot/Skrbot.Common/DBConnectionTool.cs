using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace Gss.Sys.Common
{
    public class DBConnectionTool
    {
        public static string GetDESDecryptConnectionString(string enCryptConnectionString)
        {
            string conn_server=Gss.Sys.Common.ConfigManager.GetAppSetting("conn_server");
            string conn_db=Gss.Sys.Common.ConfigManager.GetAppSetting("conn_db");
            string conn_user=Gss.Sys.Common.ConfigManager.GetAppSetting("conn_user");
            string conn_security = Gss.Sys.Common.Encrypt.AESDecrypt(Gss.Sys.Common.ConfigManager.GetAppSetting("conn_security"));

            enCryptConnectionString = string.Format(enCryptConnectionString, conn_server, conn_db, conn_user, conn_security);

            return enCryptConnectionString;
            //string[] connStrCollection = enCryptConnectionString.Split(';');
            //for (int i = 0; i < connStrCollection.Length; i++)
            //{
            //    switch (connStrCollection[i].Split('=')[0])
            //    {
            //        case "Password":
            //            connStrCollection[i] = "Password=" +
            //                Gss.Sys.Common.Encrypt.AESDecrypt(connStrCollection[i].Replace("Password=", ""));
            //            break;
            //        default:
            //            break;
            //    }
            //}
            //return string.Join(";", connStrCollection);
        }        
    }
}
