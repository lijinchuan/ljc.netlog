using LJC.Com.LogService.ContractNew;
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
                LogWriters[name] = ObjTextWriter.CreateWriter(LogFileDir + name + "log.bin", ObjTextReaderWriterEncodeType.entitybufex);
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

        public static bool WriteLogs(List<LogInfo> logs)
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

                if (logs != null && logs.Count > 0)
                {
                    logs.GroupBy(p => p.Level).ToList().ForEach(p =>
                    {
                        LogWriters[p.Key.ToString()].AppendObject(p.ToArray());
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

        public static Tuple<long,List<LogInfo>> ReadLogs(LogLevel logLevel,long pos,int pageSize,DateTime begin,DateTime end,string range,string word)
        {

            List<LogInfo> loglist = new List<LogInfo>();
            var readsize = Math.Min(100, pageSize);

            var lastpos = -1L;
            var skipcount = 0;

            if (pos != -1)
            {
                using (ObjTextReader logreader = ObjTextReader.CreateReader(LogFileDir + logLevel + "log.bin"))
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
                        skipcount += templist.Length;
                        if (reset)
                        {
                            reset = false;
                        }

                        if (templist.Length > 0)
                        {
                            if (templist.First().LogTime > end)
                            {
                                continue;
                            }
                            var lastlog = templist.Last();
                            templist = templist.Where(p => p.LogTime >= begin && p.LogTime <= end).ToArray();
                            if (!string.IsNullOrEmpty(word))
                            {
                                if (range == "T")
                                {
                                    templist = templist.Where(p => p.LogTitle?.IndexOf(word, StringComparison.OrdinalIgnoreCase) > -1).ToArray();
                                }
                                else if (range == "C")
                                {
                                    templist = templist.Where(p => p.Info?.IndexOf(word, StringComparison.OrdinalIgnoreCase) > -1).ToArray();
                                }
                                else if (range == "S")
                                {
                                    templist = templist.Where(p => p.LogFrom?.IndexOf(word, StringComparison.OrdinalIgnoreCase) > -1).ToArray();
                                }
                                else
                                {
                                    templist = templist.Where(p => p.LogTitle?.IndexOf(word, StringComparison.OrdinalIgnoreCase) > -1
                                    || p.Info?.IndexOf(word, StringComparison.OrdinalIgnoreCase) > -1
                                    || p.StackTrace?.IndexOf(word, StringComparison.OrdinalIgnoreCase) > -1
                                    || p.LogFrom?.IndexOf(word, StringComparison.OrdinalIgnoreCase) > -1).ToArray();
                                }
                            }

                            loglist.AddRange(templist);
                            if (loglist.Count >= readsize)
                            {
                                lastpos = logreader.ReadedPostion();
                                break;
                            }

                            if (lastlog.LogTime < begin)
                            {
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
