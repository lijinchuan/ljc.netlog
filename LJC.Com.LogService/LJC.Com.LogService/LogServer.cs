using LJC.Com.LogService.Contract;
using LJC.FrameWork.Comm;
using LJC.FrameWork.EntityBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LJC.Com.LogService
{
    public class LogServer:LJC.FrameWork.SocketEasy.Sever.SessionServer
    {
        private static string LogFileDir = System.AppDomain.CurrentDomain.BaseDirectory + "\\LogFile\\";
        private static LJC.FrameWork.Comm.ObjTextWriter LogWriter;

        public LogServer(string[] ips, int port)
            : base(ips,port)
        {
            if(!System.IO.Directory.Exists(LogFileDir))
            {
                System.IO.Directory.CreateDirectory(LogFileDir);
            }
            LogWriter = ObjTextWriter.CreateWriter(LogFileDir + "log.bin", ObjTextReaderWriterEncodeType.protobufex);
        }

        protected override void FormApp(FrameWork.SocketApplication.Message message, FrameWork.SocketApplication.Session session)
        {

            if (message.MessageHeader.MessageType == Contract.Const.LogMessageType)
            {
                try
                {
                    //Console.WriteLine("当前时间1：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss" + "." + DateTime.Now.Millisecond));

                    var logs = message.GetMessageBody<LogInfo[]>();

                    //Console.WriteLine("当前时间2：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss" + "." + DateTime.Now.Millisecond));

                    if (logs != null && logs.Length > 0)
                    {
                        LogWriter.AppendObject<LogInfo[]>(logs);
                        //LogWriter.Flush();
                    }

                    //Console.WriteLine("当前时间3：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss" + "." + DateTime.Now.Millisecond));

                    if (!string.IsNullOrWhiteSpace(message.MessageHeader.TransactionID))
                    {
                        WriteLogResponse response = new WriteLogResponse();
                        response.Result = 1;
                        response.Msg = "写入成功";


                        var msg = new FrameWork.SocketApplication.Message
                        {
                            MessageHeader = new FrameWork.SocketApplication.MessageHeader
                            {
                                TransactionID = message.MessageHeader.TransactionID,
                                MessageType = message.MessageHeader.MessageType
                            }
                        };

                        //Console.WriteLine("当前时间4：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss" + "." + DateTime.Now.Millisecond));

                        msg.SetMessageBody(response);

                        session.SendMessage(msg);
                    }
                    //Console.WriteLine("当前时间5：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss" + "." + DateTime.Now.Millisecond));
                }
                catch (Exception ex)
                {
                    LJC.FrameWork.LogManager.LogHelper.Instance.Error(ex.Message, ex);

                    if (!string.IsNullOrWhiteSpace(message.MessageHeader.TransactionID))
                    {
                        WriteLogResponse response = new WriteLogResponse();
                        response.Msg = ex.Message;


                        var msg = new FrameWork.SocketApplication.Message
                        {
                            MessageHeader = new FrameWork.SocketApplication.MessageHeader
                            {
                                TransactionID = message.MessageHeader.TransactionID,
                                MessageType = message.MessageHeader.MessageType
                            }
                        };

                        //Console.WriteLine("当前时间4：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss" + "." + DateTime.Now.Millisecond));

                        msg.SetMessageBody(response);

                        session.SendMessage(msg);
                    }
                }
                return;
            }

            base.FormApp(message, session);
        }

        protected override bool OnUserLogin(string user, string pwd, out string loginFailReson)
        {
            loginFailReson = string.Empty;
            return true;
        }
    }
}
