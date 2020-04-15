using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utility
{
    public class AppConfig
    {
        public static readonly string LogConfigFile = GetStringConfigValue("LogConfigFile");//Log配置文件目录
        public static readonly string HttpListenerApp = GetStringConfigValue("HttpListenerApp"); //HttpListener监听地址

        #region 内部通讯

        private static string m_ListerIP = ConfigurationManager.AppSettings["ListerIP"].ToString();
        /// <summary>
        /// 监听IP地址
        /// </summary>
        public static string ListerIP
        {
            get
            {
                return m_ListerIP;
            }
        }

        private static int m_V7InnerCommPort = Convert.ToInt32(ConfigurationManager.AppSettings["V7InnerCommPort"]);
        /// <summary>
        /// V7内部通信端口
        /// </summary>
        public static int V7InnerCommPort
        {
            get
            {
                return m_V7InnerCommPort;
            }
        }

        private static int m_JsonPort = Convert.ToInt32(ConfigurationManager.AppSettings["JsonPort"]);
        /// <summary>
        /// json端口
        /// </summary>
        public static int JsonPort
        {
            get
            {
                return m_JsonPort;
            }
        }

        private static string m_BroadCastIP = ConfigurationManager.AppSettings["BroadCastIP"].ToString();
        /// <summary>
        /// 广播地址
        /// </summary>
        public static string BroadCastIP
        {
            get
            {
                return m_BroadCastIP;
            }
        }

        private static string m_CommPort = ConfigurationManager.AppSettings["CommPort"].ToString();
        /// <summary>
        /// 广播端口
        /// </summary>
        public static string CommPort
        {
            get
            {
                return m_CommPort;
            }
        }

        #endregion

        #region TCP Socket

        private static string m_ServerIP = ConfigurationManager.AppSettings["ServerIP"].ToString();
        /// <summary>
        /// 服务器IP地址
        /// </summary>
        public static string ServerIP
        {
            get
            {
                return m_ServerIP;
            }
        }

        private static string m_ServerPort = ConfigurationManager.AppSettings["ServerPort"].ToString();
        /// <summary>
        /// 服务器端口
        /// </summary>
        public static string ServerPort
        {
            get
            {
                return m_ServerPort;
            }
        }

        private static int m_TCPSocketInvalidTime = Convert.ToInt32(ConfigurationManager.AppSettings["TCPSocketInvalidTime"]);
        /// <summary>
        /// socket失效时间
        /// </summary>
        public static int TCPSocketInvalidTime
        {
            get
            {
                return m_TCPSocketInvalidTime;
            }
        }

        #endregion

        private static bool m_IsAllLocalIP = Convert.ToBoolean(ConfigurationManager.AppSettings["IsAllLocalIP"]);
        /// <summary>
        /// 是否监听所有IP
        /// </summary>
        public static bool IsAllLocalIP
        {
            get
            {
                return m_IsAllLocalIP;
            }
        }

        private static string m_HttpURL = Convert.ToString(ConfigurationManager.AppSettings["HttpURL"]);
        /// <summary>
        /// 远程服务器URL
        /// </summary>
        public static string HttpURL
        {
            get { return m_HttpURL; }
            set { m_HttpURL = value; }
        }


        #region 格式化各类型 Config
        /// <summary>
        /// 获取字符串型Config值
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        private static string GetStringConfigValue(string keyName)
        {
            try
            {
                return ConfigurationManager.AppSettings[keyName].ToString();
            }
            catch
            {
                //Log4Net.LogError("GlobalData/GetStringConfigValue()", keyName + "配置项有误");
                LogUtility.Error("AppConfig/GetStringConfigValue()", keyName + "配置项有误");
                return "";
            }
        }

        /// <summary>
        /// 获取bool型Config值
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        private static bool GetBoolConfigValue(string keyName)
        {
            try
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings[keyName]);
            }
            catch
            {
                //Log4Net.LogError("GlobalData/GetBoolConfigValue()", keyName + "配置项有误");
                LogUtility.Error("AppConfig/GetBoolConfigValue()", keyName + "配置项有误");
                return false;
            }
        }

        /// <summary>
        /// 获取数字型Config值
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        private static Int32 GetInt32ConfigValue(string keyName)
        {
            Regex regex = new Regex("^\\d*$");
            string tempStr;
            try
            {
                tempStr = ConfigurationManager.AppSettings[keyName].ToString();
                if (regex.IsMatch(tempStr))
                {
                    return Convert.ToInt32(tempStr);
                }

                return 0;
            }
            catch
            {
                //Log4Net.LogError("GlobalData/GetInt32ConfigValue()", keyName + "配置项有误");
                LogUtility.Error("AppConfig/GetInt32ConfigValue()", keyName + "配置项有误");
                return 0;
            }
        }
        #endregion
    }
}
