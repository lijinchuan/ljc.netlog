using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.ServiceProcess;
using System.Text;

namespace LJC.com.ExeDog
{
    partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 在此处添加代码以启动服务。
            var exes = ConfigurationManager.AppSettings["WatchExeList"].Split(',');

            var process = Process.GetProcesses().ToList();
            foreach (var e in exes)
            {
                if (!File.Exists(e))
                {
                    continue;
                }

                if (process.Exists(p => e.EndsWith(p.ProcessName + ".exe")))
                {
                    continue;
                }

                try
                {
                    ProcessStart.Start3(e, new string[] { "autorun-yes" });
                }
                catch (Exception ex)
                {

                }
            }
        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
        }
    }
}
