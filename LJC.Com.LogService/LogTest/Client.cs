using LJC.Com.LogService.Contract;
using LJC.FrameWork.SocketApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LogTest
{
    public class Client:LJC.FrameWork.SocketEasy.Client.SessionClient
    {

        public Client(string ip, int port)
            : base(ip, port, false)
        {

        }

        protected override void OnLoginSuccess()
        {
            base.OnLoginSuccess();

            while (true)
            {
                //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                for (int j = 0; j < 1; j++)
                {
                    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                    sw.Restart();

                    var msg = new LJC.FrameWork.SocketApplication.Message
                    {
                        MessageHeader = new LJC.FrameWork.SocketApplication.MessageHeader
                        {
                            MessageType = LJC.Com.LogService.Contract.Const.LogMessageType,
                            MessageTime = DateTime.Now,
                            TransactionID = LJC.FrameWork.SocketApplication.SocketApplicationComm.GetSeqNum()
                        }
                    };

                    //Console.WriteLine("当前时间0：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss" + "." + DateTime.Now.Millisecond));

                    msg.SetMessageBody(new LJC.Com.LogService.Contract.LogInfo[]{
                new LJC.Com.LogService.Contract.LogInfo{
                    LogType=LogType.Service,
                    Level=LogLevel.Info,
                    LogFrom="001",
                    Info="测试日志",
                    StackTrace="测试嘉信茂"

            }});

                    var resp = SendMessageAnsy<WriteLogResponse>(msg);

                    sw.Stop();
                    Console.WriteLine("用时："+sw.ElapsedMilliseconds+"ms");
                }
                Thread.Sleep(1000);
            }
        }
    }
}
