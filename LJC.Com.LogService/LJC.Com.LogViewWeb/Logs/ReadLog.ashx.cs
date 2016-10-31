using LJC.Com.LogService.Contract;
using LJC.FrameWork.Comm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LJC.Com.LogViewWeb.Scripts.Pages.Logs
{
    /// <summary>
    /// ReadLog 的摘要说明
    /// </summary>
    public class ReadLog : IHttpHandler
    {
        static string _logdir = System.Configuration.ConfigurationManager.AppSettings["logforder"];

        public void ProcessRequest(HttpContext context)
        {
            var loglist = new List<LogInfo> { };
            try
            {
                context.Response.ContentType = "text/json";
                var loglevel = context.Request["loglevel"];
                var pos = -1;
                if (!int.TryParse(context.Request["pos"], out pos))
                {
                    pos = 0;
                }
                var readsize = 100;
                
                var lastpos = -1L;

                if (pos != -1)
                {
                    using (ObjTextReader logreader = ObjTextReader.CreateReader(_logdir + "\\" + loglevel + "log.bin"))
                    {
                        LogInfo[] templist = null;
                        if (pos != 0)
                        {
                            if (!logreader.SetPostion(pos))
                            {
                                throw new Exception("长度过长");
                            }
                        }

                        while ((templist = logreader.ReadObjectFromBack<LogInfo[]>()) != null)
                        {
                            if (templist.Length > 0)
                            {
                                loglist.AddRange(templist);
                                if (loglist.Count >= readsize)
                                {
                                    lastpos = logreader.ReadedPostion();
                                    break;
                                }
                            }
                        }

                        if (templist == null)
                        {
                            lastpos = -1;
                        }
                    }
                }

                var result = new
                {
                    result=1,
                    lastpos=lastpos,
                    data=loglist,
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