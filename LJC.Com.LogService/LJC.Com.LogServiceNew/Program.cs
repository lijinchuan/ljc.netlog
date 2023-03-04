using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LJC.Com.LogServiceNew
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
#if DEBUG
            var svcHost = new LogSvcHost();
            svcHost.StartService();
            while (true)
            {
                Thread.Sleep(1000);
            }


#else

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new LogSvc()
            };
            ServiceBase.Run(ServicesToRun);
#endif


        }
    }
}
