using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LJC.Com.LogService.Contract
{
    public class WriteLogResponse
    {
        public int Result
        {
            get;
            set;
        }

        public string Msg
        {
            get;
            set;
        }
    }
}
