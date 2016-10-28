using LJC.FrameWork.Comm;
using LJC.FrameWork.LogManager;
using LJC.FrameWork.SocketApplication;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace LJC.Com.LogService
{
    public partial class Service1 : ServiceBase
    {
        private LogServer server;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                var port = !string.IsNullOrWhiteSpace(ConfigHelper.AppConfig("LogServicePort")) ? int.Parse(ConfigHelper.AppConfig("LogServicePort")) : Contract.Const.ServerPort;
                server = new LogServer(null,port);
                server.Error += server_Error;
                server.StartServer();
                LogHelper.Instance.Info("服务启动:" + port);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error("服务启动失败", ex);
            }
        }

        void server_Error(Exception obj)
        {
            LogHelper.Instance.Error("日志出错", obj);
        }

        protected override void OnStop()
        {
            
        }
    }
}
