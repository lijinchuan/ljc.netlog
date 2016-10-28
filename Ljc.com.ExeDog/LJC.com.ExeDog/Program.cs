using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace LJC.com.ExeDog
{
    class Program
    {
        static void Main(string[] args)
        {
            //ProcessStart.Start2(@"E:\开发工具\数据库助手new\NETDBHelper -mysql\NETDBHelper\bin\Debug\NETDBMysqlHelper.exe", null);

            if (!File.Exists("点击我注册服务.bat") || !File.Exists("点击我注销服务.bat"))
            {
                var currentRunExePath = Process.GetCurrentProcess().MainModule.FileName;
                var cdpath = currentRunExePath.Substring(currentRunExePath.IndexOf('\\') + 1, currentRunExePath.LastIndexOf('\\') - currentRunExePath.IndexOf('\\') - 1);

                using (StreamWriter sw = new StreamWriter("点击我注册服务.bat", false, Encoding.GetEncoding("GBK")))
                {
                    sw.WriteLine(currentRunExePath.First() + ":");
                    sw.WriteLine("cd " + cdpath);
                    sw.WriteLine(@"%windir%\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe " + Process.GetCurrentProcess().MainModule.FileName);
                    sw.WriteLine("pause");
                }

                using (StreamWriter sw = new StreamWriter("点击我注销服务.bat", false, Encoding.GetEncoding("GBK")))
                {
                    sw.WriteLine(currentRunExePath.First() + ":");
                    sw.WriteLine("cd " + cdpath);
                    sw.WriteLine(@"%windir%\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe -u " + Process.GetCurrentProcess().MainModule.FileName);
                    sw.WriteLine("pause");
                }
            }

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new Service1()
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
