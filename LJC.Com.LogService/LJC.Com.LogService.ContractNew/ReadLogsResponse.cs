using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJC.Com.LogService.ContractNew
{
    public class ReadLogsResponse
    {
        public LogInfo[] Logs
        {
            get;
            set;
        }

        public long Lastpos
        {
            get;
            set;
        }
    }
}
