using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LJC.Com.LogService.Contract
{
    public class Client:LJC.FrameWork.SocketEasy.Client.SessionClient
    {
        public Client(string ip, int port)
            : base(ip, port, false)
        {

        }
    }
}
