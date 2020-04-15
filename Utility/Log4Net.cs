using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    /// <summary>
    /// 日志记录
    /// </summary>
    public class Log4Net
    {
        private static log4net.ILog Log;
        private Log4Net() { }
        private static object m_SyncObj = new object();//同步对象  
        static Log4Net()
        {
            Log = log4net.LogManager.GetLogger(typeof(Log4Net));
            string logFileName = AppConfig.LogConfigFile;
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(logFileName));
        }

        #region 记录各种类型 log

        /// <summary>
        /// 记录log信息
        /// </summary>
        /// <param name="Info">消息</param>
        public static void LogInfo(string Info)
        {
            lock (m_SyncObj)
            {
                Log.Info("[" + Info + "]");
            }
        }

        /// <summary>
        /// 记录错误日志到文件
        /// </summary>
        /// <param name="ErrorPlace">错误出处</param>
        /// <param name="ErrorMsg">错误内容</param>
        public static void LogError(string ErrorPlace, object ErrorMsg)
        {
            lock (m_SyncObj)
            {
                Log.Error("[" + ErrorPlace + "]");
                Log.Error(ErrorMsg);
            }
        }

        /// <summary>
        /// 记录调试日志到文件
        /// </summary>
        /// <param name="BugPlace">记录出处</param>
        /// <param name="BugMsg">记录内容</param>
        public static void LogBug(string BugPlace, object BugMsg)
        {
            lock (m_SyncObj)
            {
                Log.Debug("[" + BugPlace + "]");
                Log.Debug(BugMsg);
            }
        }

        /// <summary>
        /// 记录警告日志到文件
        /// </summary>
        /// <param name="BugPlace">记录出处</param>
        /// <param name="BugMsg">记录内容</param>
        public static void LogWarn(string WarnPlace, object WarnMsg)
        {
            lock (m_SyncObj)
            {
                Log.Warn("[" + WarnPlace + "]");
                Log.Warn(WarnMsg);
            }
        }

        #endregion
    }
}
