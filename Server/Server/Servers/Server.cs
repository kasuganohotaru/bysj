using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace GameServer.Servers
{
    class Server
    {
        private Socket ServerSocket;
        private List<Client> clients = new List<Client>();

        public Server(int port)
        {
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ServerSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            ServerSocket.Listen(0);
            StartAccept();
        }

        /// <summary>
        /// 异步连接客户端
        /// </summary>
        private void StartAccept()
        {
            ServerSocket.BeginAccept(AcceptCallBack,null);
        }
        
        private void AcceptCallBack(IAsyncResult asyncResult)
        {
            //将连接到的客户端添加到用户列表中
            Socket ClientSocket = ServerSocket.EndAccept(asyncResult);
            clients.Add(new Client(ClientSocket,this));
            StartAccept();
        }
    }
}
