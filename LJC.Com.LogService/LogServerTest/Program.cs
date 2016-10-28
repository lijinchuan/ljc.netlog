using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogServerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            LJC.Com.LogService.LogServer server = new LJC.Com.LogService.LogServer(null,LJC.Com.LogService.Contract.Const.ServerPort);
            server.Error += server_Error;
            server.StartServer();

            Console.Read();
        }

        static void server_Error(Exception obj)
        {
            Console.WriteLine(obj.Message);
        }
    }
}
