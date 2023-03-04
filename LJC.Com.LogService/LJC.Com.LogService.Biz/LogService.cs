using LJC.Com.LogService.Contract;
using LJC.FrameWork.Comm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LJC.Com.LogService.Biz
{
    public class LogService
    {
        private static string LogFileDir = System.Configuration.ConfigurationManager.AppSettings["LogFileDir"] ?? System.AppDomain.CurrentDomain.BaseDirectory + "\\LogFile\\";
        //private static LJC.FrameWork.Comm.ObjTextWriter LogWriter;
        private static Dictionary<string, ObjTextWriter> LogWriters = new Dictionary<string, ObjTextWriter>();
        private static System.Threading.ReaderWriterLockSlim _lock = new System.Threading.ReaderWriterLockSlim();
        private static System.Timers.Timer _flushtimer = new System.Timers.Timer();

        static LogService()
        {
            if (!System.IO.Directory.Exists(LogFileDir))
            {
                System.IO.Directory.CreateDirectory(LogFileDir);
            }

            foreach (var name in Enum.GetNames(typeof(LogLevel)))
            {
                LogWriters[name] = ObjTextWriter.CreateWriter(LogFileDir + name + "log.bin", ObjTextReaderWriterEncodeType.protobufex);
            }

            _flushtimer.Interval = 5000;
            _flushtimer.Elapsed += _flushtimer_Elapsed;
            _flushtimer.Start();
        }

        static void _flushtimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _flushtimer.Stop();
            try
            {
                _lock.EnterWriteLock();
                foreach (var writer in LogWriters)
                {
                    writer.Value.Flush();
                }
            }
            catch
            {

            }
            finally
            {
                _lock.ExitWriteLock();
            }
            _flushtimer.Start();
        }

        public static bool WriteLogs(LogInfo[] logs)
        {
            if (logs == null || !logs.Any())
            {
                return false;
            }
            try
            {
                _lock.EnterReadLock();
                //Console.WriteLine("当前时间1：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss" + "." + DateTime.Now.Millisecond));

                //Console.WriteLine("当前时间2：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss" + "." + DateTime.Now.Millisecond));

                if (logs != null && logs.Length > 0)
                {
                    logs.GroupBy(p => p.Level).ToList().ForEach(p =>
                    {
                        LogWriters[p.Key.ToString()].AppendObject<LogInfo[]>(p.ToArray());
                    });

                    //LogWriter.Flush();
                    return true;
                }

                //Console.WriteLine("当前时间3：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss" + "." + DateTime.Now.Millisecond));
                

            }
            catch (Exception ex)
            {
                FrameWork.LogManager.LogHelper.Instance.Error(ex.Message, ex);

            }
            finally
            {
                _lock.ExitReadLock();
            }

            return false;
        }

        public static Tuple<long,List<LogInfo>> ReadLogs(LogLevel logLevel,long pos,int pageSize)
        {

            List<LogInfo> loglist = new List<LogInfo>();
            var readsize = Math.Min(100, pageSize);

            var lastpos = -1L;

            if (pos != -1)
            {
                using (ObjTextReader logreader = ObjTextReader.CreateReader(LogFileDir + "\\" + logLevel + "log.bin"))
                {
                    LogInfo[] templist = null;
                    if (pos != 0)
                    {
                        if (!logreader.SetPostion(pos))
                        {
                            throw new Exception("长度过长");
                        }
                    }

                    bool reset = pos == 0;

                    while ((templist = logreader.ReadObjectFromBack<LogInfo[]>(reset)) != null)
                    {
                        if (reset)
                        {
                            reset = false;
                        }

                        if (templist.Length > 0)
                        {
                            loglist.AddRange(templist);
                            if (loglist.Count >= readsize)
                            {
                                lastpos = logreader.ReadedPostion();
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (templist == null)
                    {
                        lastpos = -1;
                    }
                }
            }

            return new Tuple<long, List<LogInfo>>(lastpos, loglist);
        }
    }
}
