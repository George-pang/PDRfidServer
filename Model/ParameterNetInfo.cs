using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ParameterNetInfo
    {
        private string m_BroadcastIP;
        /// <summary>
        /// 广播IP
        /// </summary>
        public string BroadcastIP
        {
            get { return m_BroadcastIP; }
            set { m_BroadcastIP = value; }
        }

        private int m_CommonPort;
        /// <summary>
        /// IP地址
        /// </summary>
        public int CommonPort
        {
            get { return m_CommonPort; }
            set { m_CommonPort = value; }
        }

        private int m_Port;
        /// <summary>
        /// 端口号码
        /// </summary>
        public int Port
        {
            get { return m_Port; }
            set { m_Port = value; }
        }

        private List<string> m_GpsServerIPList;
        /// <summary>
        /// GPS的IP地址列表
        /// </summary>
        public List<string> GpsServerIPList
        {
            get { return m_GpsServerIPList; }
            set { m_GpsServerIPList = value; }
        }
        private int m_GpsPort;
        /// <summary>
        /// GPS端口号
        /// </summary>
        public int GpsPort
        {
            get { return m_GpsPort; }
            set { m_GpsPort = value; }
        }

        private string m_CtiServerIP;
        /// <summary>
        /// Cti服务器IP
        /// </summary>
        public string CtiServerIP
        {
            get { return m_CtiServerIP; }
            set { m_CtiServerIP = value; }
        }

        private int m_CtiPort;
        /// <summary>
        /// Cti服务器IP
        /// </summary>
        public int CtiPort
        {
            get { return m_CtiPort; }
            set { m_CtiPort = value; }
        }
    }
}
