using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skrbot.Generic.Dao
{
    public class Variable
    {
        /// <summary>
        /// 設定在Config內的sfm db 連線字串id
        /// </summary>
        public static string GenericConnectionID
        {
            get { return "Default"; }
        }

        public static int DefaultPaggingPageSize
        {
            get { return 10; }
        }
    }
}
