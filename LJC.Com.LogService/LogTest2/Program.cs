using LJC.Com.LogService.Contract;
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
                StackTrace = "stacktrace"
            });

            Console.Read();
            NetLogHelper.SendLogEnshure(new LogInfo
            {
                Info = "测试info",
                Level = LogLevel.Debug,
                LogFrom = "test2",
                LogType = LogType.Web,
                StackTrace = "stacktrace"
            });
            Console.Read();
        }
    }
}
