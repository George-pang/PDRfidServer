using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// http监听请求--响应消息实体类
    /// </summary>
    public class TAppHttpResponseInfo
    {
        private string m_msgType;  //响应消息类型
        private bool m_isSuccess;  //响应是否成功
        private string m_Content;  //响应消息内容


        //响应消息类型
        public string msgType
        {
            get { return m_msgType; }
            set { m_msgType = value; }
        }

        //响应是否成功
        public bool isSuccess
        { 
            get { return m_isSuccess; }
            set { m_isSuccess = value; }
        }

        //响应消息内容
        public string Content
        {
            get { return m_Content; }
            set { m_Content = value; }
        }

    }
}
