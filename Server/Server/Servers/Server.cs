using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using SocketGameProtocol;
using GameServer.Controller;

namespace GameServer.Servers
{
    class Server
    {
        private Socket _serverSocket;
        private List<Client> _clients = new List<Client>();
        private List<Room> _roomList = new List<Room>();

        private UDPServer _us;

        private ControllerManager _controllerManager;

        public Server(int port)
        {
            _controllerManager = new ControllerManager(this);

            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            _serverSocket.Listen(0);
            StartAccept();
            Console.WriteLine("TCP服务已启动...");
            _us = new UDPServer(8889, this, _controllerManager);
        }

        /// <summary>
        /// 异步连接客户端
        /// </summary>
        private void StartAccept()
        {
            _serverSocket.BeginAccept(AcceptCallBack,null);
        }
        
        private void AcceptCallBack(IAsyncResult asyncResult)
        {
            //将连接到的客户端添加到用户列表中
            Socket clientSocket = _serverSocket.EndAccept(asyncResult);
            _clients.Add(new Client(clientSocket,this,_us));
            Console.WriteLine("客户端已连接，当前人数：{0}",_clients.Count);
            StartAccept();
        }

        public Client ClientFromUserName(string user)
        {
            foreach (Client c in _clients)
            {
                if (c.GetUserData._userName == user)
                {
                    return c;
                }
            }
            return null;
        }

        public bool SetIEP(EndPoint iPEnd, string user)
        {
            foreach (Client c in _clients)
            {
                if (c.GetUserData._userName == user)
                {
                    c.IEP = iPEnd;
                    //Console.WriteLine("设置IEP： "+c.IEP.ToString());
                    return true;
                }
            }
            return false;
        }

        public void HandleRequest(MainPack pack, Client client)
        {
            _controllerManager.HandleRequest(pack, client);
        }

        public void RemoveClient(Client client)
        {
            _clients.Remove(client);
            client = null;
            //Memory.ClearMemory();
        }

        public MainPack CreateRoom(Client client, MainPack pack)
        {
            try
            {
                Room room = new Room(client, pack.Roompack[0], this);
                _roomList.Add(room);
                foreach (PlayerPack p in room.GetPlayerInFo())
                {
                    pack.Playerpack.Add(p);
                }
                pack.Returncode = ReturnCode.Succeed;
                return pack;
            }
            catch
            {
                pack.Returncode = ReturnCode.Fail;
                return pack;
            }
        }

        public MainPack FindRoom()
        {
            MainPack pack = new MainPack();
            pack.Actioncode = ActionCode.FindRoom;
            try
            {
                if (_roomList.Count == 0)
                {
                    pack.Returncode = ReturnCode.NotRoom;
                    return pack;
                }
                foreach (Room room in _roomList)
                {
                    pack.Roompack.Add(room.GetRoomInFo);
                }
                pack.Returncode = ReturnCode.Succeed;
            }
            catch
            {
                pack.Returncode = ReturnCode.Fail;
            }
            return pack;
        }

        public MainPack JoinRoom(Client client, MainPack pack)
        {
            foreach (Room r in _roomList)
            {
                if (r.GetRoomInFo.RoomName.Equals(pack.Str))
                {
                    if (r.GetRoomInFo.Statc == 0)
                    {
                        //可以加入房间
                        r.Join(client);
                        pack.Roompack.Add(r.GetRoomInFo);
                        foreach (PlayerPack p in r.GetPlayerInFo())
                        {
                            pack.Playerpack.Add(p);
                        }
                        pack.Returncode = ReturnCode.Succeed;
                        return pack;
                    }
                    else
                    {
                        //房间不可加入
                        pack.Returncode = ReturnCode.Fail;
                        return pack;
                    }
                }
            }
            //没有此房间
            pack.Returncode = ReturnCode.NotRoom;
            return pack;
        }

        public MainPack ExitRoom(Client client, MainPack pack)
        {
            if (client.GetRoom == null)
            {
                pack.Returncode = ReturnCode.Fail;
                return pack;
            }

            client.GetRoom.Exit(this, client);
            pack.Returncode = ReturnCode.Succeed;
            return pack;
        }

        public void RemoveRoom(Room room)
        {
            _roomList.Remove(room);
            room = null;
            //Memory.ClearMemory();
        }
    }
}
