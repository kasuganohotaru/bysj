using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketGameProtocol;
using GameServer.Servers;

namespace GameServer.Controller
{
    class GameController:BaseController
    {
        public GameController()
        {
            requestCode = RequestCode.Game;
        }

        public MainPack ExitGame(Server server, Client client, MainPack pack)
        {
            client.GetRoom.ExitGame(client);
            return null;
        }

        public MainPack UpPos(Client client, MainPack pack)
        {
            client.GetRoom.BroadcastTo(client, pack);
            client.UpPos(pack);//更新位置信息
            return null;
        }

        public MainPack Fire(Server server, Client client, MainPack pack)
        {
            client.GetRoom.BroadcastTo(client, pack);
            return null;
        }

        public MainPack Damage(Server server, Client client, MainPack pack)
        {
            client.GetRoom.Damage(pack, client);
            return null;
        }
    }
}
