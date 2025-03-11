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
        public void ProcessRequest(HttpContext context)
        {
            List<LogService.ContractNew.LogInfo> loglist = null;
            try
            {
                context.Response.ContentType = "text/json";
                var loglevel = context.Request["loglevel"];
                var word = context.Request["word"];
                var begintxt = context.Request["begin"];
                var endtext = context.Request["end"];
                var range = context.Request["range"]?.ToUpper();
                DateTime begin = DateTime.Now;
                DateTime end = DateTime.Now;
                if (!DateTime.TryParse(begintxt, out begin))
                {
                    begin = DateTime.Now.AddDays(-7);
                }
                if (!DateTime.TryParse(endtext, out end))
                {
                    end = DateTime.Now;
                }
                var pos = -1L;
                if (!long.TryParse(context.Request["pos"], out pos))
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
                            ReadSize = 100,
                            Begin=begin,
                            End=end,
                            Range=range,
                            Word=word
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