using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UbiGEM.Net.Utility.Logger
{
    [ComVisible(false)]
    internal sealed class LogWriter : IDisposable
    {
        public event LogWriteEventHandler OnWriteLog;

        private const int THREAD_SLEEP_INTERVAL_START = 500;
        private const int THREAD_SLEEP_INTERVAL_WRITE = 50;
        private const int LOG_DELETE_SLEEP_INTERVAL = 10;

        private readonly ReaderWriterLockSlim _writerLock;

        private readonly Queue<LogData> _que;
        private LogMode _logMode;
        private LogType _logType;
        private LogLevel _level;
        private string _rootPath;

        private bool _threadFlag;

        public LogWriter()
        {
            this._writerLock = new ReaderWriterLockSlim();
            this._que = new Queue<LogData>();
        }

        public void Dispose()
        {
            try
            {
                this._threadFlag = false;

                lock (this._que)
                {
                    this._que.Clear();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
            }
        }

        public void Initialize(string logPath, LogMode logMode, LogType logType, LogLevel level = LogLevel.Information)
        {
            this._rootPath = logPath;
            this._logMode = logMode;
            this._logType = logType;
            this._level = level;

            if (this._logMode != LogMode.None && this._threadFlag == false)
            {
                System.Threading.Thread th = new System.Threading.Thread(Run)
                {
                    Name = "LogWriter",
                    Priority = System.Threading.ThreadPriority.BelowNormal
                };

                this._threadFlag = true;

                th.Start();
            }
        }

        public void Add(LogLevel level, string logText)
        {
            lock (this._que)
            {
                if (this._logMode != LogMode.None && this._level <= level)
                    this._que.Enqueue(new LogData(level, logText));
            }
        }

        public void Add(Exception ex)
        {
            lock (this._que)
            {
                if (this._logMode != LogMode.None && this._level <= LogLevel.Error)
                    this._que.Enqueue(new LogData(ex));
            }
        }

        public string Delete(int expirationDays)
        {
            string result;
            DateTime startTime;

            try
            {
                startTime = DateTime.Now;

                DeleteExpiredFiles(this._rootPath, expirationDays);
                DeleteEmptyFolders(this._rootPath);

                TimeSpan timeSpan = DateTime.Now - startTime;

                result = string.Format("Log file delete 소요 시간 : {0:0.000000}", timeSpan.TotalSeconds);
            }
            catch (Exception ex)
            {
                result = string.Format("Log file delete failed\r\n Exception Messgae = {0}", ex.Message);
            }

            return result;
        }

        private void Run()
        {
            System.Threading.Thread.Sleep(THREAD_SLEEP_INTERVAL_START);
            LogData logData;

            while (this._threadFlag)
            {
                try
                {
                    while (this._que.Count > 0)
                    {
                        lock (this._que)
                        {
                            logData = this._que.Dequeue();
                        }

                        if (logData.Level >= this._level)
                        {
                            Write(logData);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(string.Format("### Log Write Exception(Run-GEM) : {0} [{1}]\r\n{2}", ex.GetType(), DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"), ex.Message));
                }
                finally
                {
                    System.Threading.Thread.Sleep(THREAD_SLEEP_INTERVAL_WRITE);
                }
            }
        }

        private string GetFileName(LogData logData)
        {
            if (this._logMode == LogMode.Hour)
            {
                return string.Format("{0}({1:00}).log", logData.Time.ToString("yyyy-MM-dd"), logData.Time.Hour);
            }
            else
            {
                return string.Format("{0}.log", logData.Time.ToString("yyyy-MM-dd"));
            }
        }

        private void Write(LogData logData)
        {
            string directory;
            string fileName;
            string logText;

            try
            {
                directory = string.Format(@"{0}\{1}", this._rootPath, logData.Time.ToString("yyyy-MM-dd"));

                if (Directory.Exists(directory) == false)
                {
                    Directory.CreateDirectory(directory);
                }

                fileName = GetFileName(logData);

                logText = logData.GetLogText();

                _writerLock.EnterWriteLock();

                try
                {
                    using (StreamWriter sw = new StreamWriter(string.Format(@"{0}\{1}", directory, fileName), true, Encoding.Default))
                    {
                        sw.Write(logText);
                    }
                }
                catch { }

                if (this.OnWriteLog != null && this._logType == LogType.GEM)
                {
                    this.OnWriteLog(logData.Level, logText);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(string.Format("### Log Write Exception(Write-GEM) : {0} [{1}]\r\n{2}", ex.GetType(), DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"), ex.Message));
            }
            finally
            {
                _writerLock.ExitWriteLock();
            }
        }

        private void DeleteExpiredFiles(string logPath, int expireDay)
        {
            string[] files;

            if (Directory.Exists(logPath) == true)
            {
                try
                {
                    foreach (string directory in Directory.GetDirectories(logPath))
                    {
                        DeleteExpiredFiles(directory, expireDay);
                    }

                    files = Directory.GetFiles(logPath);

                    for (int index = 0; index < files.Length; ++index)
                    {
                        if ((DateTime.Now - File.GetCreationTime(files[index])).TotalDays > expireDay && IsFileAccessible(files[index]))
                        {
                            File.Delete(files[index]);

                            System.Threading.Thread.Sleep(LOG_DELETE_SLEEP_INTERVAL);
                        }
                    }

                    files = null;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);
                }
            }
        }

        private static bool IsFileAccessible(string path)
        {
            FileStream fileStream = null;

            try
            {
                fileStream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);

                return false;
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }

            return true;
        }

        private void GetEmptyFolderList(string strPath, List<string> list)
        {
            if (Directory.Exists(strPath) == true)
            {
                try
                {
                    foreach (string directory in Directory.GetDirectories(strPath))
                    {
                        GetEmptyFolderList(directory, list);
                    }

                    if (Directory.GetFiles(strPath).Length > 0)
                        return;

                    list.Add(strPath);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);
                }
            }
        }

        private void DeleteEmptyFolders(string strPath)
        {
            if (Directory.Exists(strPath) == true)
            {
                try
                {
                    List<string> list = new List<string>();

                    GetEmptyFolderList(strPath, list);

                    for (int index = 0; index < list.Count; ++index)
                    {
                        if (Directory.GetDirectories(list[index]).Length <= 0)
                        {
                            Directory.Delete(list[index]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);
                }
            }
        }
    }
}