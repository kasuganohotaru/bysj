using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using GameServer.Tool;

namespace GameServer.Servers
{
    class Client
    {
        private Socket ClientSocket;
        private Server server;
        private Message message;


        public Client(Socket socket,Server _server)
        {
            ClientSocket = socket;
            server = _server;
        }

        /// <summary>
        /// 异步接收消息
        /// </summary>
        private void StartReceive()
        {
            ClientSocket.BeginReceive(message.Buffer, message.StartIndex, message.Remsize, SocketFlags.None, ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult iar)
        {

        }
    }

}
