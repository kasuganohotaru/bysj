using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using GameServer.Tool;
using SocketGameProtocol;
using System.Data.SqlClient;
using GameServer.DAO;

namespace GameServer.Servers
{
    class Client
    {
        private Socket _clientSocket;
        //private Socket _udpClientSocket;
        private Server _server;
        private Message _msg;
        private SqlConnection _sqlConnt;
        private EndPoint _remoteEp;
        private UserData _userData;

        public UDPServer _us;

        public PlayerInFo GetPlayerInfo
        {
            get;
            set;
        }

        public class PlayerInFo
        {
            public float HP
            {
                set;
                get;
            }

            public PosPack Pos
            {
                get;
                set;
            }
        }

        public EndPoint IEP
        {
            get
            {
                return _remoteEp;
            }
            set
            {
                _remoteEp = value;
            }
        }

        public UserData GetUserData
        {
            get { return _userData; }
        }

        public SqlConnection GetSqlConnect
        {
            get
            {
                return _sqlConnt;
            }
        }

        public Room GetRoom
        {
            get; set;
        }

        public Client(Socket socket,Server server, UDPServer us)
        {
            _msg = new Message();
            _userData = new UserData();
            _sqlConnt = DbManager.Instance.OpenDB();
            if(_sqlConnt!=null)
            {
                Console.WriteLine("成功连接数据库");
            }
            else
            {
                Console.WriteLine("无法连接数据库");
            }
            _us = us;
            _clientSocket = socket;
            _server = server;

            StartReceive();
        }

        /// <summary>
        /// 异步接收消息
        /// </summary>
        private void StartReceive()
        {
            _clientSocket.BeginReceive(_msg.Buffer, _msg.StartIndex, _msg.Remsize, SocketFlags.None, ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult iar)
        {
            try
            {
                if (_clientSocket == null || _clientSocket.Connected == false) return;
                int len = _clientSocket.EndReceive(iar);
                Console.WriteLine("接收");
                if (len == 0)
                {
                    Console.WriteLine("接收数据为0");
                    Close();
                    return;
                }

                _msg.ReadBuffer(len, HandleRequest);
                StartReceive();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Close();
            }
        }

        public void UpPos(MainPack pack)
        {
            GetPlayerInfo.Pos = pack.Playerpack[0].Pospack;
        }

        public void Send(MainPack pack)
        {
            if (_clientSocket == null || _clientSocket.Connected == false) return;
            try
            {
                _clientSocket.Send(Message.PackData(pack));
            }
            catch
            {

            }
        }

        public void SendTo(MainPack pack)
        {
            if (IEP == null) return;
            _us.SendTo(pack, IEP);
        }

        private void Close()
        {
            Console.WriteLine("断开");
            _sqlConnt.Close();
            _clientSocket.Close();
            _server.RemoveClient(this);
        }

        void HandleRequest(MainPack pack)
        {
            _server.HandleRequest(pack, this);
        }
    }

}
