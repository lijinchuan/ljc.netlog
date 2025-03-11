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
                var word = context.Request["word"];
                var begintxt = context.Request["begin"];
                var endtext = context.Request["end"];
                var range = context.Request["range"]?.ToUpper();
                DateTime begin = DateTime.Now;
                DateTime end = DateTime.Now;
                if(!DateTime.TryParse(begintxt,out begin))
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
                var readsize = 100;
                var skipcount = 0;
                
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

                        bool reset = pos == 0;

                        while ((templist = logreader.ReadObjectFromBack<LogInfo[]>(reset)) != null)
                        {
                            skipcount += templist.Length;
                            if(reset)
                            {
                                reset = false;
                            }

                            if (templist.Length > 0)
                            {
                                if (templist.First().LogTime > end)
                                {
                                    continue;
                                }
                                var lastlog = templist.Last();
                                templist = templist.Where(p => p.LogTime >= begin && p.LogTime <= end).ToArray();
                                if (!string.IsNullOrEmpty(word))
                                {
                                    if (range == "T")
                                    {
                                        templist = templist.Where(p => p.LogTitle?.IndexOf(word, StringComparison.OrdinalIgnoreCase) > -1).ToArray();
                                    }
                                    else if (range == "C")
                                    {
                                        templist = templist.Where(p => p.Info?.IndexOf(word, StringComparison.OrdinalIgnoreCase) > -1).ToArray();
                                    }
                                    else if (range == "S")
                                    {
                                        templist = templist.Where(p => p.LogFrom?.IndexOf(word, StringComparison.OrdinalIgnoreCase) > -1).ToArray();
                                    }
                                    else
                                    {
                                        templist = templist.Where(p => p.LogTitle?.IndexOf(word, StringComparison.OrdinalIgnoreCase) > -1
                                        || p.Info?.IndexOf(word, StringComparison.OrdinalIgnoreCase) > -1
                                        || p.StackTrace?.IndexOf(word, StringComparison.OrdinalIgnoreCase) > -1
                                        || p.LogFrom?.IndexOf(word, StringComparison.OrdinalIgnoreCase) > -1).ToArray();
                                    }
                                }

                                loglist.AddRange(templist);
                                if (loglist.Count >= readsize)
                                {
                                    lastpos = logreader.ReadedPostion();
                                    break;
                                }

                                if (lastlog.LogTime <begin)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
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
                    skipcount=skipcount,
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