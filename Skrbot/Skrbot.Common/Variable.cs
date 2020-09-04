using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skrbot.Common
{
    /// <summary>
    /// 公用變數
    /// </summary>
    public static class Variable
    {
        /// <summary>
        /// log放置的位置
        /// </summary>
        public static string LogPath
        {
            get { return ConfigManager.GetAppSetting("LogPath"); }
        }

        /// <summary>
        /// log4net設定檔的位置
        /// </summary>
        public static string Log4netConfigPath
        {
            get { return ConfigManager.GetAppSetting("Log4netConfigPath"); }
        }

        /// <summary>
        /// 間隔毫秒
        /// </summary>
        public static string TimeInterval
        {
            get { return ConfigManager.GetAppSetting("TimeInterval"); }
        }
    }
}
