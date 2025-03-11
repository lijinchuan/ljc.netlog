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

        public override object DoResponse(int funcId, byte[] Param, string clientid, Dictionary<string, string> header)
        {
            switch (funcId)
            {
                case Const.Fun_InsertLogs:
                    {
                        InsertLogsResponse resp = new InsertLogsResponse();
                        var req = FrameWork.EntityBuf.EntityBufCore.DeSerialize<InsertLogsRequest>(Param);

                        resp.Success = LogService.Biz.LogService.WriteLogs(req.LogInfos);

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

                        var result = LogService.Biz.LogService.ReadLogs(req.Loglevel, req.Pos, req.ReadSize,
                            req.Begin,req.End,req.Range,req.Word);

                        resp.Lastpos = result.Item1;
                        resp.Logs = result.Item2;

                        return resp;
                    }

            }

            return base.DoResponse(funcId, Param, clientid, header);
        }
    }
}
