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

        public void RemoveRoom(Room room)
        {
            _roomList.Remove(room);
            room = null;
            //Memory.ClearMemory();
        }
    }
}
