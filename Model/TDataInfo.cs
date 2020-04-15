using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    /// <summary>
    /// 服务端Socket TCP接收消息实体
    /// 2017-06-20  xmx
    /// 2020-03-11 plq 移植改造
    /// </summary>
    public class TDataInfo
    {
        private string m_MessageType;
        private string m_Content;
        private string m_Code;
        private string m_Name;
        private byte[] m_bytes;

        //消息类型
        public string MessageType
        { 
            get { return m_MessageType; }
            set { m_MessageType = value; }
        }

        //消息内容
        public string Content
        { 
            get { return m_Content; }
            set { m_Content = value; }
        }

        //编码--用来区分客户端(可理解为客户端的编码)
        public string Code
        {
            get { return m_Code; }
            set { m_Code = value; }
        }

        //名称--用来区分客户端(可理解为客户端的名称)
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        //byte[]类型消息
        public byte[] Bytes
        {
            get { return m_bytes; }
            set { m_bytes = value; }
        }

    }
}
