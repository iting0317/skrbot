using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Skrbot.Generic.Dao;
using System.Data;
namespace Skrbot.Dao
{
    public class BaseDao
    {
        protected SqlServerFixture DbFixture;

        public BaseDao()
        {
            this.ChangeDB(Skrbot.Generic.Dao.Variable.GenericConnectionID);
        }

        public void ChangeDB(string connectionID)
        {
            this.DbFixture = new SqlServerFixture(connectionID);   
        }
    }
}
