using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using SocketGameProtocol;

namespace GameServer.Servers
{
    class DbManager : SingletonBase<DbManager>
    {
        public static string _constr = "";

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

        public bool Updata(MainPack pack, SqlConnection sqlConnection)
        {
            bool isSucc = false;
            try
            {
                string updateStr = "UPDATE account SET name = @name,skin=@skin WHERE acct=@acct";
                SqlCommand sqlCommand = new SqlCommand(updateStr, sqlConnection);
                sqlCommand.Parameters.AddWithValue("name", pack.Userinfo.UserName);
                sqlCommand.Parameters.AddWithValue("skin", pack.Userinfo.Skin);

                sqlCommand.Parameters.AddWithValue("acct", pack.Userinfo.UserAcct);

                int rows = sqlCommand.ExecuteNonQuery();
                if (rows != 0)
                    isSucc = true;
            }
            catch(Exception ex)
            {
                isSucc = false;
                Console.WriteLine("更新用户数据出错: " + ex);
            }
            return isSucc;
        }
    }

}
