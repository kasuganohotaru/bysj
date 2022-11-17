using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketGameProtocol;
using System.Data.SqlClient;
using GameServer.Servers;

namespace GameServer.DAO
{
    class UserData
    {
        public string _userName;
        public string _userAcct;

        public bool Register(MainPack pack,SqlConnection sqlConnection)
        {
            string userAcct = pack.Userinfo.UserAcct;
            string passWord = pack.Userinfo.PassWord;

            try
            {
                string sql = "INSERT INTO account (acct,pass,name,skin) VALUES(@acct,@pass,@name,@skin)";
                SqlCommand comd = new SqlCommand(sql, sqlConnection);
                comd.Parameters.AddWithValue("acct", userAcct);
                comd.Parameters.AddWithValue("pass", passWord);
                Random random = new Random();
                comd.Parameters.AddWithValue("name", "玩家"+ random);
                comd.Parameters.AddWithValue("skin", 1);

                //插入数据
                comd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public bool Login(MainPack pack, SqlConnection sqlConnection)
        {
            string userAcct = pack.Userinfo.UserAcct;
            string passWord = pack.Userinfo.PassWord;
            _userAcct = userAcct;

            string sql = "SELECT id FROM dbo.account WHERE acct = @Param1 AND pass = @Param2";
            SqlCommand cmd = new SqlCommand(sql, sqlConnection);
            cmd.Parameters.AddWithValue("Param1", userAcct);
            cmd.Parameters.AddWithValue("Param2", passWord);
            SqlDataReader read = cmd.ExecuteReader();
            bool result = read.HasRows;
            if(result)
            {

                _userName = pack.Userinfo.UserName = read["name"].ToString().Trim();
                pack.Userinfo.Skin = int.Parse(read["skin"].ToString().Trim());
            }
            read.Close();
            return result;
        }
    }
}
