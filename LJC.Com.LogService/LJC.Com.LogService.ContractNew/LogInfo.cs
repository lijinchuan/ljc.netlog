using System;

namespace LJC.Com.LogService.ContractNew
{
    public class LogInfo
    {
        public LogType LogType
        {
            get;
            set;
        }

        public LogLevel Level
        {
            get;
            set;
        }
        public string LogFrom
        {
            get;
            set;
        }

        public string LogTitle
        {
            get;
            set;
        }

        public string Info
        {
            get;
            set;
        }

        public string StackTrace
        {
            get;
            set;
        }

        public DateTime LogTime
        {
            get;
            set;
        }
    }
}
