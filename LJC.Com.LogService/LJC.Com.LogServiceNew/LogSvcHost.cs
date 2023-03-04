using LJC.Com.LogService.ContractNew;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJC.Com.LogServiceNew
{
    public class LogSvcHost : FrameWork.SOA.ESBService
    {
        public LogSvcHost() : base(Const.Sno, true, true, Const.ServiceName, System.Net.Dns.GetHostName())
        {
        }

        public override object DoResponse(int funcId, byte[] Param, string clientid)
        {
            switch (funcId)
            {
                case Const.Fun_InsertLogs:
                    {
                        InsertLogsResponse resp = new InsertLogsResponse();
                        var req = FrameWork.EntityBuf.EntityBufCore.DeSerialize<InsertLogsRequest>(Param);

                        resp.Success = LogService.Biz.LogService.WriteLogs(req.LogInfos.Select(p => new LogService.Contract.LogInfo
                        {
                            Info = p.Info,
                            Level = (LogService.Contract.LogLevel)p.Level,
                            LogFrom = p.LogFrom,
                            LogTime = p.LogTime,
                            LogTitle = p.LogTitle,
                            LogType = (LogService.Contract.LogType)p.LogType,
                            StackTrace = p.StackTrace
                        }).ToArray());

                        if (resp.Success)
                        {
                            resp.Message = "成功";
                        }
                        else
                        {
                            resp.Message = "失败";
                        }

                        return resp;
                    }
                case Const.Fun_ReadLogs:
                    {
                        ReadLogsResponse resp = new ReadLogsResponse();
                        var req = FrameWork.EntityBuf.EntityBufCore.DeSerialize<ReadLogsRequest>(Param);

                        var result = LogService.Biz.LogService.ReadLogs((LogService.Contract.LogLevel)req.Loglevel, req.Pos, req.ReadSize);

                        resp.Lastpos = result.Item1;
                        resp.Logs = result.Item2.Select(p=>new LogInfo
                        {
                            Info=p.Info,
                            Level= (LogLevel)p.Level,
                            LogFrom=p.LogFrom,
                            LogTime=p.LogTime,
                            LogTitle=p.LogTitle,
                            LogType= (LogType)p.LogType,
                            StackTrace=p.StackTrace
                        }).ToArray();

                        return resp;
                    }

            }

            return base.DoResponse(funcId, Param, clientid);
        }
    }
}
