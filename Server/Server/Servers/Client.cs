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
        private DbManager _DB;

        private Socket _clientSocket;
        private Server _server;
        private Message _msg;
        private SqlConnection _sqlConnt;
        private EndPoint _remoteEp;
        private UserData _userData;
        private SqlConnection _sqlConnection;

        public UDPServer us;

        public UserInFo GetUserInFo
        {
            get;
            set;
        }

        public class UserInFo
        {
            public string UserName
            {
                get; set;
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
                return _sqlConnection;
            }
        }

        public Client(Socket socket,Server server)
        {
            _userData = new UserData();
            _msg = new Message();
            _sqlConnection = DbManager.Instance.OpenDB();
            GetUserInFo = new UserInFo();
            _sqlConnection.Open();


            _clientSocket = socket;
            _server = server;
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

        private void Close()
        {
            Console.WriteLine("断开");

            _clientSocket.Close();
            _server.RemoveClient(this);
        }

        void HandleRequest(MainPack pack)
        {
            _server.HandleRequest(pack, this);
        }
    }

}
