using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace GameServer.Servers
{
    class DbManager:SingletonBase<DbManager>
    {
        public string _constr = "";

        public override void Init()
        {
            string temp = ConfigurationManager.ConnectionStrings["PixelWarDB"].ConnectionString;
            _constr = temp;
            return;
        }

        public SqlConnection OpenDB()//打开数据库，返回连接
        {
            Init();
            SqlConnection connection = new SqlConnection(_constr);
            try
            {
                connection.Open();
            }
            catch (SqlException e)
            {
                Console.WriteLine("打开数据库出错:" + e);
            }
            return connection;
        }
    }

}
