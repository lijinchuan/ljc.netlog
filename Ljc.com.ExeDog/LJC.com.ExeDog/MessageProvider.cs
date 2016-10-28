using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LJC.com.ExeDog
{
    public class MessageProvider
    {
        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSSendMessage(
            IntPtr hServer,
            int SessionId,
            String pTitle,
            int TitleLength,
            String pMessage,
            int MessageLength,
            int Style,
            int Timeout,
            out int pResponse,
            bool bWait);

        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        public extern static int FormatMessage(int flag, ref IntPtr source, int msgid, int langid, ref string buf, int size, ref IntPtr args);

        [DllImport("kernel32.dll")]
        static extern uint WTSGetActiveConsoleSessionId();

        public static IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;

        public static void ShowMessageBox(string message, string title)
        {
            int resp = 0;
            WTSSendMessage(
                WTS_CURRENT_SERVER_HANDLE,
                (int)WTSGetActiveConsoleSessionId(),
                title, title.Length,
                message, message.Length,
                0, 0, out resp, false);
        }

        public static string GetSysErrMsg(int errCode)
        {
            IntPtr tempptr = IntPtr.Zero;
            string msg = null;
            FormatMessage(0x1300, ref tempptr, errCode, 0, ref msg, 255, ref tempptr);
            return msg;
        }
    }
}
