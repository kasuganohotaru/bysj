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
        private List<Client> clients = new List<Client>();

        private ControllerManager _controllerManager;

        public Server(int port)
        {
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            _serverSocket.Listen(0);
            StartAccept();
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
            Socket ClientSocket = _serverSocket.EndAccept(asyncResult);
            clients.Add(new Client(ClientSocket,this));
            StartAccept();
        }

        public void HandleRequest(MainPack pack, Client client)
        {
            _controllerManager.HandleRequest(pack, client);
        }

        public void RemoveClient(Client client)
        {
            clients.Remove(client);
            client = null;
            //Memory.ClearMemory();
        }
    }
}
