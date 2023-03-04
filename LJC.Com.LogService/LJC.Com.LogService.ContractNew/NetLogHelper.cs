using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJC.Com.LogService.ContractNew
{
    public class NetLogHelper
    {
        internal static ConcurrentQueue<LogInfo> LogQueue = new ConcurrentQueue<LogInfo>();

        static void SendQueue()
        {
            LogInfo log = null;
            while (LogQueue.TryDequeue(out log))
            {
                SendLog(log);
            }
        }

        public static void SendLog(LogInfo log)
        {
            if (log == null)
                return;

            try
            {
                var resp = FrameWork.SOA.ESBClient.DoSOARequest2<InsertLogsResponse>(Const.Sno, Const.Fun_InsertLogs, new InsertLogsRequest
                {
                    LogInfos = new List<LogInfo>
                    {
                        log
                    }
                });

                if (!resp.Success)
                {
                    throw new Exception("日志写入失败");
                }
            }
            catch (Exception ex)
            {
                if (LogQueue.Count > 10000)
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

            var resp = FrameWork.SOA.ESBClient.DoSOARequest2<InsertLogsResponse>(Const.Sno, Const.Fun_InsertLogs, new InsertLogsRequest
            {
                LogInfos = new List<LogInfo>
                    {
                        log
                    }
            });

            if (!resp.Success)
            {
                return false;
            }

            return true;
        }
    }
}
