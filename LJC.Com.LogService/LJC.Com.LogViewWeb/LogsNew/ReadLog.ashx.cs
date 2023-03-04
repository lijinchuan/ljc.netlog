using LJC.Com.LogService.Contract;
using LJC.Com.LogService.ContractNew;
using LJC.FrameWork.Comm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LJC.Com.LogViewWeb.Scripts.Pages.LogsNew
{
    /// <summary>
    /// ReadLog 的摘要说明
    /// </summary>
    public class ReadLog : IHttpHandler
    {
        static string _logdir = System.Configuration.ConfigurationManager.AppSettings["logforder"];

        public void ProcessRequest(HttpContext context)
        {
            LogService.ContractNew.LogInfo[] loglist = null;
            try
            {
                context.Response.ContentType = "text/json";
                var loglevel = context.Request["loglevel"];
                var pos = -1;
                if (!int.TryParse(context.Request["pos"], out pos))
                {
                    pos = 0;
                }
                
                var lastpos = -1L;

                if (pos != -1)
                {
                    var resp = FrameWork.SOA.ESBClient.DoSOARequest2<ReadLogsResponse>(LogService.ContractNew.Const.Sno,
                        LogService.ContractNew.Const.Fun_ReadLogs,
                        new ReadLogsRequest
                        {
                            Loglevel = (LogService.ContractNew.LogLevel)Enum.Parse(typeof(LogService.ContractNew.LogLevel), loglevel),
                            Pos = pos,
                            ReadSize = 100
                        });

                    loglist = resp.Logs;
                    lastpos = resp.Lastpos;
                }

                var result = new
                {
                    result=1,
                    lastpos=lastpos,
                    data=loglist.Select(p=>new{
                      content=p.Info+"<br/>"+p.StackTrace,
                      logfrom=p.LogFrom,
                      time=p.LogTime.ToString("yyyy-MM-dd HH:mm:ss"),
                      title=p.LogTitle
                    }),
                    msg="成功"
                };

                context.Response.Write(result.ToJson());
            }
            catch (Exception ex)
            {
                var result = new
                {
                    result = 0,
                    lastpos = -1,
                    data = loglist,
                    msg = ex.Message
                };

                context.Response.Write(result.ToJson());
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