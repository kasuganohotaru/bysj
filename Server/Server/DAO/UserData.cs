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
        public bool Register(MainPack pack,SqlConnection sqlConnection)
        {
            string userName = pack.Loginpack.Username;
            string passWord = pack.Loginpack.Password;

            try
            {
                string sql = "INSERT INTO `sys`.`userdata` (`username`, `password`) VALUES ('" + userName + "', '" + passWord + "')";
                SqlCommand comd = new SqlCommand(sql, sqlConnection);

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
            string userName = pack.Loginpack.Username;
            string passWord = pack.Loginpack.Password;

            string sql = "SELECT * FROM userdata WHERE username='" + userName + "' AND password='" + passWord + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlConnection);
            SqlDataReader read = cmd.ExecuteReader();
            bool result = read.HasRows;
            read.Close();
            return result;
        }
    }
}
