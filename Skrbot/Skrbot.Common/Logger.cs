using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using log4net.Appender;
using log4net.Config;
using System.Web;
using Skrbot.Domain;

namespace Skrbot.Common
{
    /// <summary>
    /// 計錄Log的共用程式
    /// </summary>
    public class Logger
    {
        static log4net.ILog log4netInstance;

        public Logger()
        {

        }
        /// <summary>
        /// 寫入log資訊
        /// </summary>
        /// <param name="logCatogroy">log的分類</param>
        /// <param name="context">要記錄的log內容</param>
        public static void Write(Domain.Enum.EnumLogCategory logCatogroy, string context)
        {         
            try
            {
                log4netInstance = log4net.LogManager.GetLogger("Looger");
                log4net.Config.XmlConfigurator.Configure(new FileInfo(Variable.Log4netConfigPath));
                //log4net.GlobalContext.Properties["LogName"] =Variable.LogFilePrefix+"-"+ System.DateTime.Now.ToShortDateString().Replace("/", "-") + ".txt"

                switch (logCatogroy)
                {
                    case Domain.Enum.EnumLogCategory.Information:
                        log4netInstance.Info(context);
                        break;
                    case Domain.Enum.EnumLogCategory.Error:
                        log4netInstance.Error(context);
                        break;
                    case Domain.Enum.EnumLogCategory.Warning:
                        log4netInstance.Warn(context);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

    }
}
