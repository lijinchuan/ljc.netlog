using LJC.Com.LogService.ContractNew;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogTest2
{
    class Program
    {
        static void Main(string[] args)
        {
            NetLogHelper.SendLogEnshure(new LogInfo
            {
                Info = "测试info",
                Level = LogLevel.Debug,
                LogFrom = "test2",
                LogType = LogType.Web,
                StackTrace = "stacktrace",
                LogTime=DateTime.Now,
                LogTitle="test"
            });

            Console.Read();
            NetLogHelper.SendLogEnshure(new LogInfo
            {
                Info = "测试info",
                Level = LogLevel.Debug,
                LogFrom = "test2",
                LogType = LogType.Web,
                StackTrace = "stacktrace",
                LogTitle="testw",
                LogTime=DateTime.Now
            });
            Console.Read();
        }
    }
}
