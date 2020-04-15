using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Utility
{
    public class LogUtility
    {
        /// <summary>
        /// 程序目录名
        /// (一般根据需要修改此变量值即可)
        /// </summary>
        private const string FolderName = "PDRfidServerLog";
        public static DateTime CallTime = DateTime.Now;//2013-01-28 wy 电话最后一次正常操作时刻
        /// <summary>
        /// 日志文件名
        /// (全名或后缀名,按日分类时为后缀名)
        /// </summary>
        private const string logFileName = "Log.Log";

        /// <summary>
        /// 私有构造函数,不允许直接实例化
        /// </summary>
        private LogUtility()
        {
            //
        }
        private static object m_SysLog = new Object();//互斥对象 2013-01-21
        /// <summary>
        /// 记录错误日志到文本文件到我的文档目录
        /// 按月分,每月产生一个日志文件
        /// </summary>
        /// <param name="text">日志内容</param>
        static private void WriteLogToApplicationFolderByMonth(string text)
        {
            //string folderPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\My Tests\\Log1";


            //格式化文件夹字符串
            string folderPath = string.Format("{0}\\{1}\\Year_{2}", System.Windows.Forms.Application.StartupPath, FolderName, System.DateTime.Now.ToString("yyyy"));

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            //格式化文件路径字符串
            string folderPath1 = string.Format("{0}\\Month_{1}", folderPath, System.DateTime.Today.ToString("MM"));
            if (!Directory.Exists(folderPath1))
            {
                Directory.CreateDirectory(folderPath1);
            }

            //2012-10-16 wy 修改日志为按天记录。
            string filePath = string.Format("{0}\\Day_{1}_{2}", folderPath1, System.DateTime.Today.ToString("dd"), logFileName);

            LogToFile(filePath, text);
        }

        /// <summary>
        /// 记录文本到文本文件(根据微软MSDN2005帮助文档System.IO.File.AppendText()提供的示例修改)
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="text">记录内容</param>
        static private void LogToFile(string filePath, string text)
        {
            //-------------------
            StreamWriter sw = null;
            try
            {
                if (!File.Exists(filePath))
                {
                    sw = File.CreateText(filePath);
                }
                else
                {
                    sw = File.AppendText(filePath);
                }

                //设置写入文件的文本
                //string msg = string.Format("{0}---------Log Time:{1}--------{0}{2}", System.Environment.NewLine, System.DateTime.Now.ToString(), text);

                string msg = string.Format("\r\n---------Log Time:{0}--------\r\n{1}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), text);

                sw.WriteLine(msg);
                //sw.WriteLine(text);

            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                    sw = null;
                }
            }
        }

        public static void Error(string place, string msg)
        {
            lock (m_SysLog)//记录日志
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("【错误消息】").Append(" (").Append(place).Append(") ").Append(msg);
                WriteLogToApplicationFolderByMonth(sb.ToString());
            }
        }

        public static void Debug(string place, string msg)
        {
            lock (m_SysLog)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("【调试消息】").Append(" (").Append(place).Append(") ").Append(msg);
                WriteLogToApplicationFolderByMonth(sb.ToString());
            }
        }
   
    }
}
