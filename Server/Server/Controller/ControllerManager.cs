using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketGameProtocol;
using GameServer.Servers;
using System.Reflection;

namespace GameServer.Controller
{
    /// <summary>
    /// 控制器管理器
    /// </summary>
    class ControllerManager
    {
        private Dictionary<RequestCode, BaseController> _contDic = new Dictionary<RequestCode, BaseController>();

        private Server _server;

        public ControllerManager(Server server)
        {
            _server = server;

        }

        public void HandleRequest(MainPack pack, Client client, bool isUDP = false)
        {
            if (_contDic.TryGetValue(pack.Requestcode, out BaseController controller))
            {
                string metname = pack.Actioncode.ToString();
                MethodInfo method = controller.GetType().GetMethod(metname);
                if (method == null)
                {
                    Console.WriteLine("没有找到对应的处理方法");
                    return;
                }
                object[] obj;
                if (isUDP)//UDP
                {
                    obj = new object[] { client, pack };
                    method.Invoke(controller, obj);
                }
                else//TCP
                {
                    obj = new object[] { _server, client, pack };
                    object ret = method.Invoke(controller, obj);
                    if (ret != null)
                    {
                        //client.Send(ret as MainPack);
                        Console.WriteLine("发送数据：");
                    }
                }

            }
            else
            {
                Console.WriteLine("没有找到对应的controller处理");
            }
        }
    }
}
