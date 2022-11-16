using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketGameProtocol;
using GameServer.Servers;

namespace GameServer.Controller
{
    /// <summary>
    /// 控制器基类
    /// </summary>
    class BaseController
    {
        protected RequestCode requestCode = RequestCode.RequestNone;

        public RequestCode GetRequestCode
        {
            get { return requestCode; }
        }
    }
}
