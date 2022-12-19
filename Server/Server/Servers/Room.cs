using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using SocketGameProtocol;

namespace GameServer.Servers
{
    class Room
    {
        private RoomPack _roomInfo;//房间信息
        private Server _server;
        private List<Client> _clientList = new List<Client>();//房间内所有的客户端

        /// <summary>
        /// 返回房间信息
        /// </summary>
        public RoomPack GetRoomInFo
        {
            get
            {
                _roomInfo.CurNum = _clientList.Count;
                return _roomInfo;
            }
        }

        public Room(Client client, RoomPack pack, Server server)
        {
            _roomInfo = pack;
            _clientList.Add(client);
            client.GetRoom = this;
            _server = server;
        }

        public RepeatedField<PlayerPack> GetPlayerInFo()
        {
            RepeatedField<PlayerPack> pack = new RepeatedField<PlayerPack>();
            foreach (Client c in _clientList)
            {
                PlayerPack player = new PlayerPack();
                player.PlayerName = c.GetUserData._userName;
                pack.Add(player);
            }
            return pack;
        }

        public void Broadcast(Client client, MainPack pack)
        {
            foreach (Client c in _clientList)
            {
                if (c.Equals(client))
                {
                    continue;
                }
                c.Send(pack);
            }
        }

        public void BroadcastTo(Client client, MainPack pack)
        {
            //Console.WriteLine("广播数据");
            foreach (Client c in _clientList)
            {
                if (c.Equals(client))
                {
                    continue;
                }
                c.SendTo(pack);
            }
        }

        public void Join(Client client)
        {
            _clientList.Add(client);
            if (_clientList.Count >= _roomInfo.MaxNum)
            {
                //满人了
                _roomInfo.Statc = 1;
            }
            client.GetRoom = this;
            MainPack pack = new MainPack();
            pack.Actioncode = ActionCode.PlayerList;
            foreach (PlayerPack player in GetPlayerInFo())
            {
                pack.Playerpack.Add(player);
            }
            Broadcast(client, pack);
        }

        public void Exit(Server server, Client client)
        {
            MainPack pack = new MainPack();
            if (_roomInfo.Statc == 2)//游戏已经开始
            {
                ExitGame(client);
            }
            else//游戏未开始
            {
                if (client == _clientList[0])
                {
                    //房主离开
                    client.GetRoom = null;
                    pack.Actioncode = ActionCode.Exit;
                    Broadcast(client, pack);
                    server.RemoveRoom(this);
                    return;
                }
                _clientList.Remove(client);
                _roomInfo.Statc = 0;
                client.GetRoom = null;
                pack.Actioncode = ActionCode.PlayerList;
                foreach (PlayerPack player in GetPlayerInFo())
                {
                    pack.Playerpack.Add(player);
                }
                Broadcast(client, pack);
            }
        }

        public ReturnCode StartGame(Client client)
        {
            if (client != _clientList[0])
            {
                return ReturnCode.Fail;
            }
            _roomInfo.Statc = 2;
            Thread starttime = new Thread(Time);
            starttime.Start();
            Thread startspawnitem = new Thread(SpawnItem);
            startspawnitem.Start();
            Console.WriteLine("开始游戏");
            return ReturnCode.Succeed;
        }

        public void ExitGame(Client client)
        {
            MainPack pack = new MainPack();
            if (client == _clientList[0])
            {
                //房主退出
                pack.Actioncode = ActionCode.ExitGame;
                pack.Str = "r";
                Broadcast(client, pack);
                _server.RemoveRoom(this);
                client.GetRoom = null;
            }
            else
            {
                //其他成员退出
                _clientList.Remove(client);
                client.GetRoom = null;
                pack.Actioncode = ActionCode.UpCharacterList;
                foreach (var VARIABLE in _clientList)
                {
                    PlayerPack playerPack = new PlayerPack();
                    playerPack.PlayerName = VARIABLE.GetUserData._userName;
                    playerPack.Hp = VARIABLE.GetPlayerInfo.HP;
                    pack.Playerpack.Add(playerPack);
                }
                pack.Str = client.GetUserData._userName;
                Broadcast(client, pack);
            }
        }

        private void Time()
        {
            MainPack pack = new MainPack();
            pack.Actioncode = ActionCode.Chat;
            pack.Str = "房主已启动游戏...";
            Broadcast(null, pack);
            Thread.Sleep(1000);
            for (int i = 5; i > 0; i--)
            {
                pack.Str = i.ToString();
                Broadcast(null, pack);
                Thread.Sleep(1000);
            }

            pack.Actioncode = ActionCode.Starting;


            foreach (var VARIABLE in _clientList)
            {
                PlayerPack player = new PlayerPack();
                VARIABLE.GetPlayerInfo.HP = 100;
                player.PlayerName= VARIABLE.GetUserData._userName;
                player.Hp = VARIABLE.GetPlayerInfo.HP;
                pack.Playerpack.Add(player);
            }


            Broadcast(null, pack);
        }

        public void Damage(MainPack pack, Client cc)
        {
            Bullet bulletPack = pack.Bullet;
            PosPack posPack = null;
            Client client = null;
            foreach (Client c in _clientList)
            {
                if (c.GetUserData._userName == bulletPack.HitPlayer)
                {
                    posPack = c.GetPlayerInfo.Pos;
                    client = c;
                    break;
                }
            }

            double distance = Math.Sqrt(Math.Pow((bulletPack.X - posPack.PosX), 2) + Math.Pow((bulletPack.Y - posPack.PosY), 2));

            if (distance < 0.7f)
            {
                //击中
                
                
                Broadcast(null, pack);
            }
        }

        /// <summary>
        /// 随机道具生成
        /// </summary>
        public void SpawnItem()
        {
            MainPack pack = new MainPack();
            while(true)
            {
                Thread.Sleep(30000);
                Random random = new Random();
                int id = random.Next(1,10);
                int point = random.Next(1, 30);
                pack.Itempack.ItemID = id;
                pack.Itempack.SpawnPoint = point;
                Broadcast(null, pack);
            }
        }
    }
}
