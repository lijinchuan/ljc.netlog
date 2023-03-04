using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace LJC.Com.LogServiceNew
{
    public partial class LogSvc : ServiceBase
    {
        LogSvcHost svcHost = null;
        public LogSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            svcHost = new LogSvcHost();
            svcHost.StartService();
        }

        protected override void OnStop()
        {
            if (svcHost != null)
            {
                try
                {
                    svcHost.UnRegisterService();
                }
                finally
                {
                    svcHost.Dispose();
                }
            }
        }
    }
}
