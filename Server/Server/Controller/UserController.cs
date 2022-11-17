using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketGameProtocol;
using System.Net;
using GameServer.Servers;

namespace GameServer.Controller
{
    class UserController : BaseController
    {
        public UserController()
        {
            requestCode = RequestCode.User;
        }

        public MainPack Register(Server server, Client client, MainPack pack)
        {
            if (client.GetUserData.Register(pack, client.GetSqlConnect))
            {
                pack.Returncode = ReturnCode.Succeed;
            }
            else
            {
                pack.Returncode = ReturnCode.Fail;
            }
            return pack;
        }

        public MainPack Login(Server server, Client client, MainPack pack)
        {
            if (client.GetUserData.Login(pack, client.GetSqlConnect))
            {
                pack.Returncode = ReturnCode.Succeed;
                client.GetUserData._userName = pack.Userinfo.UserName;
            }
            else
            {
                pack.Returncode = ReturnCode.Fail;
            }
            return pack;
        }
    }
}
