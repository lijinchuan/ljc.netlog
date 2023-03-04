using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJC.Com.LogService.ContractNew
{
    public class ReadLogsRequest
    {
        public LogLevel Loglevel
        {
            get;
            set;
        }

        public long Pos
        {
            get;
            set;
        }

        public int ReadSize
        {
            get;
            set;
        }
    }
}
