using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LJC.Com.LogService.Contract
{
    [ProtoContract]
    public class LogInfo
    {
        [ProtoMember(1)]
        public LogType LogType
        {
            get;
            set;
        }

        [ProtoMember(2)]
        public LogLevel Level
        {
            get;
            set;
        }

        [ProtoMember(3)]
        public string LogFrom
        {
            get;
            set;
        }

        [ProtoMember(4)]
        public string Info
        {
            get;
            set;
        }

        [ProtoMember(5)]
        public string StackTrace
        {
            get;
            set;
        }
    }
}
