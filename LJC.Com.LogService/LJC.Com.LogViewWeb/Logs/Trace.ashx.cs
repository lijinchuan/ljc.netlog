using LJC.FrameWork.SOA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LJC.Com.LogViewWeb.Logs
{
    /// <summary>
    /// Trace 的摘要说明
    /// </summary>
    public class Trace : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("");

            try
            {
                ESBClient.DoSOARequest<bool>(Ljc.Com.IMChat.Contract.Consts.ServiceNo, Ljc.Com.IMChat.Contract.Consts.Funcid_OfflineMsg, new Ljc.Com.IMChat.Contract.UserOflineMsgRequest
                {
                    MsgTo = 100001,
                    MsgContent = "有人查看日志",
                    MsgFrom = 7,
                    MsgFromName = "网络日志 ",
                    MsgTime = DateTime.Now,
                    MsgTitle = "注意有人查看日志"
                });
            }
            catch (Exception ex)
            {

            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}