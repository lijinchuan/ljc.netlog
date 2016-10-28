using LJC.Com.LogService.Contract;
using LJC.FrameWork.SocketApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LogTest
{
    class Program
    {
        //static Client client = null;
        static List<Client> clientList = new List<Client>();

        static void Main(string[] args)
        {

            var time = DateTime.FromOADate(42643.520833333336);

            //byte[] buffer = null;

            //for (int i = 0; i < 2; i++)
            //{
            //    var msg1 = new LJC.FrameWork.SocketApplication.Message
            //    {
            //        MessageHeader = new LJC.FrameWork.SocketApplication.MessageHeader
            //        {
            //            MessageType = LJC.Com.LogService.Contract.Const.LogMessageType,
            //            MessageTime = DateTime.Now,
            //            TransactionID = LJC.FrameWork.SocketApplication.SocketApplicationComm.GetSeqNum()
            //        }
            //    };

            //    msg1.SetMessageBody(new LJC.Com.LogService.Contract.LogInfo[]{
            //    new LJC.Com.LogService.Contract.LogInfo{
            //        LogType=LogType.Service,
            //        Level=LogLevel.Info,
            //        LogFrom="001",
            //        Info="测试日志",
            //        StackTrace="测试嘉信茂"

            //}});

            //    buffer = msg1.MessageBuffer;
            //    if(i==0)
            //    {
            //        sw.Restart();
            //    }
            //}
            //sw.Stop();
            //Console.WriteLine("序列化用时:"+sw.Elapsed.TotalMilliseconds);

            
            //for (int i = 0; i < 2; i++)
            //{
            //    var msg1 = new Message();
            //    msg1.MessageBuffer = buffer;
            //    var logs = msg1.GetMessageBody<LJC.Com.LogService.Contract.LogInfo[]>();
            //    if(i==0)
            //    {
            //        sw.Restart();
            //    }
            //}
            //sw.Stop();
            //Console.WriteLine("反序列化用时:" + sw.Elapsed.TotalMilliseconds);
            //Console.Read();

            //LJC.FrameWork.EntityBuf.EntityBufCore.Serialize(msg1, false);

            //LJC.FrameWork.Comm.ObjTextReader rd = LJC.FrameWork.Comm.ObjTextReader.CreateReader(@"E:\Work\learn\LJC.Com.LogService\LogServerTest\bin\Debug\LogFile\log.bin");
            //LJC.Com.LogService.Contract.LogInfo[] list = null;
            //int i = 0;
            //while ((list = rd.ReadObject<LJC.Com.LogService.Contract.LogInfo[]>()) != null)
            //{
            //    Console.WriteLine((++i) + " " + list[0].Info);
            //    if (i == 173447)
            //    {

            //    }
            //}
            //Console.Read();

            //List<Client> clientList = new List<Client>();
            for (int ii = 0; ii < 1;ii++ )
            {
                //var client = new Client("172.16.86.13", LJC.Com.LogService.Contract.Const.ServerPort);
                //var client = new Client("172.16.63.143", LJC.Com.LogService.Contract.Const.ServerPort);
                var client = new Client("2.5.163.18", LJC.Com.LogService.Contract.Const.ServerPort);
                client.LoginSuccess += new Action(() =>
                {
                    Console.WriteLine("登录成功");
                });

                clientList.Add(client);
                client.Error += client_Error;
                client.StartClient();
                client.Login("", "");
            }
            Console.WriteLine("完毕");
            Console.Read();
        }

        static void client_Error(Exception obj)
        {
            Console.WriteLine(obj.Message);
        }

        static void client_LoginSuccess()
        {
            

            //Console.WriteLine(resp.Result);
        }
    }
}
