using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LJC.Com.LogService.Contract
{
    public class NetLogHelper
    {
        internal static ConcurrentQueue<LogInfo> LogQueue = new ConcurrentQueue<LogInfo>();
        private static string _netLogServer = System.Configuration.ConfigurationManager.AppSettings["NetLogServer"];
        private static int _netLogServerPort = int.Parse(System.Configuration.ConfigurationManager.AppSettings["NetLogServerPort"] ?? Const.ServerPort.ToString());
        static Client _client = null;
        static bool _islogin = false;

        static NetLogHelper()
        {
            _client = new Client(_netLogServer, _netLogServerPort);
            _client.LoginSuccess += client_LoginSuccess;
            _client.SessionResume += client_SessionResume;
            _client.SessionTimeOut += client_SessionTimeOut;

            if (_client.StartClient())
            {
                _client.Login("", "");
            }
            else
            {
                new Action(() =>
                {
                    while (!_client.StartClient())
                    {
                        Thread.Sleep(1000);
                    }
                    _client.Login("", "");
                }).BeginInvoke(null, null);
            }
        }

        static void client_SessionTimeOut()
        {
           
        }

        static void SendQueue()
        {
            LogInfo log=null;
            while(LogQueue.TryDequeue(out log))
            {
                SendLog(log);
            }
        }

        static void client_SessionResume()
        {
            SendQueue();
        }

        static void client_LoginSuccess()
        {
            _islogin = true;
            SendQueue();
        }

        public static void SendLog(LogInfo log)
        {
            if (log == null)
                return;

            try
            {
                if(_islogin)
                {
                    var msg = new FrameWork.SocketApplication.Message
                    {
                        MessageHeader = new FrameWork.SocketApplication.MessageHeader
                        {
                            MessageType=Const.LogMessageType
                        },
                    };

                    msg.SetMessageBody(new LogInfo[] { log });

                    _client.SendMessage(msg);
                }
                else
                {
                    throw new Exception("未准备好发送数据");
                }
            }
            catch(Exception ex)
            {
                if(LogQueue.Count>10000)
                {
                    //throw new Exception("发送日志失败:" + ex.Message + "，且日志队列已经满。");
                    
                    return;
                }
                LogQueue.Enqueue(log);
            }
        }

        public static bool SendLogEnshure(LogInfo log)
        {
            if (log == null)
                return false;


            if (_islogin)
            {
                var msg = new FrameWork.SocketApplication.Message
                {
                    MessageHeader = new FrameWork.SocketApplication.MessageHeader
                    {
                        TransactionID = LJC.FrameWork.SocketApplication.SocketApplicationComm.GetSeqNum(),
                        MessageType = Const.LogMessageType
                    },
                };

                msg.SetMessageBody(new LogInfo[] { log });
                //msg.SetMessageBody(log);

                var result = _client.SendMessageAnsy<WriteLogResponse>(msg);

                if(result.Result != 1)
                {
                    throw new Exception(result.Msg);
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
