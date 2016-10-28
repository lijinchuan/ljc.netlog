using LJC.FrameWork.LogManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace LJC.com.ExeDog
{
    public static class ProcessStart
    {
        #region
        [StructLayout(LayoutKind.Sequential)]
        internal struct PROCESS_INFORMATION
        {
            internal IntPtr hProcess;
            internal IntPtr hThread;
            internal int dwProcessId;
            internal int dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct STARTUPINFO
        {
            internal int cb;
            [MarshalAs(UnmanagedType.LPTStr)]
            internal string lpReserved;
            [MarshalAs(UnmanagedType.LPTStr)]
            internal string lpDesktop;
            [MarshalAs(UnmanagedType.LPTStr)]
            internal string lpTitle;
            internal int dwX;
            internal int dwY;
            internal int dwXSize;
            internal int dwYSize;
            internal int dwXCountChars;
            internal int dwYCountChars;
            internal int dwFillAttribute;
            internal int dwFlags;
            internal short wShowWindow;
            internal short cbReserved2;
            internal IntPtr lpReserved2;
            internal IntPtr hStdInput;
            internal IntPtr hStdOutput;
            internal IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SECURITY_ATTRIBUTES
        {
            internal int nLength;
            internal int lpSecurityDescriptor;
            internal bool bInheritHandle;

        }

        #endregion

        #region win32function
        [DllImport("kernel32.dll")]
        static extern uint WTSGetActiveConsoleSessionId();

        [DllImport("advapi32", SetLastError = true), SuppressUnmanagedCodeSecurityAttribute]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, ref IntPtr TokenHandle);

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool DuplicateTokenEx(IntPtr tokenHandle, 
            int dwDesiredAccess,
            ref SECURITY_ATTRIBUTES lpTokenAttributes, 
            int SECURITY_IMPERSONATION_LEVEL,
            int TOKEN_TYPE, ref IntPtr dupeTokenHandle);

        [DllImport("userenv.dll", SetLastError = true)]
        static extern bool CreateEnvironmentBlock(
            out IntPtr lpEnvironment,
            IntPtr hToken,
            bool bInherit);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern Boolean SetTokenInformation(IntPtr TokenHandle, TOKEN_INFORMATION_CLASS TokenInformationClass,ref UInt32 TokenInformation, UInt32 TokenInformationLength);

        [DllImport("advapi32.dll", EntryPoint = "CreateProcessAsUser", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private extern static bool CreateProcessAsUser(IntPtr hToken, String lpApplicationName, String lpCommandLine, ref SECURITY_ATTRIBUTES lpProcessAttributes,
            ref SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandle, int dwCreationFlags, IntPtr lpEnvironment,
            String lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress", SetLastError = false)]
        public static extern IntPtr GetProcAddress(int hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        [DllImport("kernel32.dll", EntryPoint = "FreeLibrary", SetLastError = false)]
        public static extern bool FreeLibrary(int hModule);

        [DllImport("kernel32.dll", EntryPoint = "CloseHandle", SetLastError = false,CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private extern static bool CloseHandle(IntPtr handle);

        #endregion

        #region 
        // SECURITY_IMPERSONATION_LEVEL
        const int SecurityAnonymous = 0;
        const int SecurityIdentification = 1;
        const int SecurityImpersonation = 2;
        const int SecurityDelegation = 3;

        // TOKEN_TYPE
        const int TokenPrimary = 1;
        const int TokenImpersonation = 2;

        //dwCreationFlags parameter controls the new process's priority class
        const int NORMAL_PRIORITY_CLASS = 0x00000020;
        const int IDLE_PRIORITY_CLASS = 0x00000040;
        const int HIGH_PRIORITY_CLASS = 0x00000080;
        const int REALTIME_PRIORITY_CLASS = 0x00000100;
        const int BELOW_NORMAL_PRIORITY_CLASS = 0x00004000;
        const int ABOVE_NORMAL_PRIORITY_CLASS = 0x00008000;

        const uint TOKEN_DUPLICATE = 0x0002;
        const uint MAXIMUM_ALLOWED = 0x2000000;
        const int CREATE_NEW_CONSOLE = 0x00000010;
        #endregion

        public static bool Start(string file,string[] args)
        {
            uint winlogonPid = 0;
            uint dwSessionId = WTSGetActiveConsoleSessionId();

            IntPtr hProcess = IntPtr.Zero, hPToken = IntPtr.Zero, hTargetToken = IntPtr.Zero, pEnvironment = IntPtr.Zero;

            Process[] processes = Process.GetProcessesByName("explorer");
            foreach (Process p in processes)
            {
                if ((uint)p.SessionId == dwSessionId)
                {
                    winlogonPid = (uint)p.Id;
                    break;
                }
            }

            hProcess = OpenProcess(MAXIMUM_ALLOWED, false, winlogonPid);

            // obtain a handle to the access token of the winlogon process
            if (!OpenProcessToken(hProcess, TOKEN_DUPLICATE, ref hPToken))
            {
                CloseHandle(hProcess);
                return false;
            }

            PROCESS_INFORMATION processInfo = new PROCESS_INFORMATION();
            //Setting startupinfo
            STARTUPINFO startInfo = new STARTUPINFO();
            startInfo.cb = Marshal.SizeOf(startInfo);
            startInfo.lpDesktop = null;
            startInfo.wShowWindow = 5;

            SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();
            sa.bInheritHandle = false;
            sa.nLength = Marshal.SizeOf(sa);
            sa.lpSecurityDescriptor = 0;

            bool status = DuplicateTokenEx(hPToken, (int)MAXIMUM_ALLOWED, ref sa, SecurityImpersonation, TokenPrimary, ref  hTargetToken);

            status = status && CreateEnvironmentBlock(out pEnvironment, hTargetToken, false);
            int dwCreationFlags = NORMAL_PRIORITY_CLASS | CREATE_NEW_CONSOLE;

            string cmdlime = null;
            if (args != null && args.Length > 0)
            {
                cmdlime = " " + string.Join(" ", args);
            }

            status = status && CreateProcessAsUser(
                            hTargetToken,
                            file, // file to execute
                            cmdlime, // command line
                            ref sa, // pointer to process
                            ref sa, // pointer to thread SECURITY_ATTRIBUTES
                            false, // handles are not inheritable
                            dwCreationFlags, // creation flags
                            IntPtr.Zero, // pointer to new environment block
                            Path.GetDirectoryName(file), // name of current directory
                            ref startInfo, // pointer to STARTUPINFO structure
                            out processInfo // receives information about new process
            );

            if (hProcess != IntPtr.Zero)
                CloseHandle(hProcess);
            if (hPToken != IntPtr.Zero)
                CloseHandle(hPToken);
            if (hTargetToken != IntPtr.Zero)
                CloseHandle(hTargetToken);

            return status;
        }


        public static bool Start2(string file, string[] args)
        {
            uint winlogonPid = (uint)Process.GetCurrentProcess().Id;
            //uint dwSessionId = (uint)Process.GetCurrentProcess().SessionId;

            IntPtr hProcess = IntPtr.Zero, hPToken = IntPtr.Zero, hTargetToken = IntPtr.Zero, pEnvironment = IntPtr.Zero;

            hProcess = OpenProcess(MAXIMUM_ALLOWED, false, winlogonPid);

            // obtain a handle to the access token of the winlogon process
            if (!OpenProcessToken(hProcess, TOKEN_DUPLICATE, ref hPToken))
            {
                LogHelper.Instance.Error("OpenProcessToken:" + MessageProvider.GetSysErrMsg(Marshal.GetLastWin32Error()));
                CloseHandle(hProcess);
                return false;
            }

            PROCESS_INFORMATION processInfo = new PROCESS_INFORMATION();
            //Setting startupinfo
            STARTUPINFO startInfo = new STARTUPINFO();
            startInfo.cb = Marshal.SizeOf(startInfo);
            startInfo.lpDesktop = null;
            startInfo.wShowWindow = 5;

            SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();
            sa.bInheritHandle = false;
            sa.nLength = Marshal.SizeOf(sa);
            sa.lpSecurityDescriptor = 0;

            bool status = DuplicateTokenEx(hPToken, (int)MAXIMUM_ALLOWED, ref sa, SecurityImpersonation, TokenPrimary, ref  hTargetToken);

            if (!status)
            {
                LogHelper.Instance.Error("DuplicateTokenEx:" + MessageProvider.GetSysErrMsg(Marshal.GetLastWin32Error()));
            }

            //找出有桌面的
            uint tokenInfo = 1;
            Process[] processes = Process.GetProcessesByName("explorer");
            foreach (Process p in processes)
            {
                tokenInfo = (uint)p.SessionId;
                break;
            }
            
            status = status && SetTokenInformation(hTargetToken, TOKEN_INFORMATION_CLASS.TokenSessionId, ref tokenInfo, 4);

            if (!status)
            {
                LogHelper.Instance.Error("SetTokenInformation:" + MessageProvider.GetSysErrMsg(Marshal.GetLastWin32Error()));
            }

            status = status && CreateEnvironmentBlock(out pEnvironment, hTargetToken, false);
            int dwCreationFlags = NORMAL_PRIORITY_CLASS | CREATE_NEW_CONSOLE;

            if (!status)
            {
                LogHelper.Instance.Error("CreateEnvironmentBlock:" + MessageProvider.GetSysErrMsg(Marshal.GetLastWin32Error()));
            }

            string cmdlime = null;
            if (args != null && args.Length > 0)
            {
                cmdlime = " " + string.Join(" ", args);
            }

            status = status && CreateProcessAsUser(
                            hTargetToken,
                            file, // file to execute
                            cmdlime, // command line
                            ref sa, // pointer to process
                            ref sa, // pointer to thread SECURITY_ATTRIBUTES
                            false, // handles are not inheritable
                            dwCreationFlags, // creation flags
                            IntPtr.Zero, // pointer to new environment block
                            Path.GetDirectoryName(file), // name of current directory
                            ref startInfo, // pointer to STARTUPINFO structure
                            out processInfo // receives information about new process
            );

            if(!status)
            {
                LogHelper.Instance.Error("CreateProcessAsUser:" + MessageProvider.GetSysErrMsg(Marshal.GetLastWin32Error()));
            }

            if (hProcess != IntPtr.Zero)
                CloseHandle(hProcess);
            if (hPToken != IntPtr.Zero)
                CloseHandle(hPToken);
            if (hTargetToken != IntPtr.Zero)
                CloseHandle(hTargetToken);

            return status;
        }

        public static bool Start3(string file, string[] args)
        {
            uint winlogonPid = 0;
            uint dwSessionId = WTSGetActiveConsoleSessionId();

            IntPtr hProcess = IntPtr.Zero, hPToken = IntPtr.Zero, hTargetToken = IntPtr.Zero, pEnvironment = IntPtr.Zero;

            Process[] processes = Process.GetProcessesByName("explorer");
            foreach (Process p in processes)
            {
                if(p.SessionId!=dwSessionId)
                {
                    dwSessionId = (uint)p.SessionId;
                }
                winlogonPid = (uint)p.Id;
                break;
            }

            LogHelper.Instance.Info("winlogonPid:" + winlogonPid);
            LogHelper.Instance.Info("dwSessionId:" + dwSessionId);

            hProcess = OpenProcess(MAXIMUM_ALLOWED, false, winlogonPid);

            LogHelper.Instance.Info("hProcess:" + hProcess);

            // obtain a handle to the access token of the winlogon process
            if (!OpenProcessToken(hProcess, TOKEN_DUPLICATE, ref hPToken))
            {
                LogHelper.Instance.Error("OpenProcessToken:" + MessageProvider.GetSysErrMsg(Marshal.GetLastWin32Error()));
                CloseHandle(hProcess);
                return false;
            }

            LogHelper.Instance.Info("hPToken:" + hPToken);

            PROCESS_INFORMATION processInfo = new PROCESS_INFORMATION();
            //Setting startupinfo
            STARTUPINFO startInfo = new STARTUPINFO();
            startInfo.cb = Marshal.SizeOf(startInfo);
            startInfo.lpDesktop = null;
            startInfo.wShowWindow = 5;

            SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();
            sa.bInheritHandle = false;
            sa.nLength = Marshal.SizeOf(sa);
            sa.lpSecurityDescriptor = 0;

            bool status = DuplicateTokenEx(hPToken, (int)MAXIMUM_ALLOWED, ref sa, SecurityImpersonation, TokenPrimary, ref  hTargetToken);

            LogHelper.Instance.Info("hTargetToken:" + hTargetToken);

            if (!status)
            {
                LogHelper.Instance.Error("DuplicateTokenEx:" + MessageProvider.GetSysErrMsg(Marshal.GetLastWin32Error()));
            }

            status = status && SetTokenInformation(hTargetToken, TOKEN_INFORMATION_CLASS.TokenSessionId, ref dwSessionId, 4);

            if (!status)
            {
                LogHelper.Instance.Error("SetTokenInformation:" + MessageProvider.GetSysErrMsg(Marshal.GetLastWin32Error()));
            }

            status = status && CreateEnvironmentBlock(out pEnvironment, hTargetToken, false);

            if (!status)
            {
                LogHelper.Instance.Error("CreateEnvironmentBlock:" + MessageProvider.GetSysErrMsg(Marshal.GetLastWin32Error()));
            }

            int dwCreationFlags = NORMAL_PRIORITY_CLASS | CREATE_NEW_CONSOLE;

            string cmdlime = null;
            if (args != null && args.Length > 0)
            {
                cmdlime = " " + string.Join(" ", args);
            }

            status = status && CreateProcessAsUser(
                            hTargetToken,
                            file, // file to execute
                            cmdlime, // command line
                            ref sa, // pointer to process
                            ref sa, // pointer to thread SECURITY_ATTRIBUTES
                            false, // handles are not inheritable
                            dwCreationFlags, // creation flags
                            IntPtr.Zero, // pointer to new environment block
                            Path.GetDirectoryName(file), // name of current directory
                            ref startInfo, // pointer to STARTUPINFO structure
                            out processInfo // receives information about new process
            );

            if (!status)
            {
                LogHelper.Instance.Error("CreateProcessAsUser:" + MessageProvider.GetSysErrMsg(Marshal.GetLastWin32Error()));
            }

            if (hProcess != IntPtr.Zero)
                CloseHandle(hProcess);
            if (hPToken != IntPtr.Zero)
                CloseHandle(hPToken);
            if (hTargetToken != IntPtr.Zero)
                CloseHandle(hTargetToken);

            return status;
        }
    }
}
