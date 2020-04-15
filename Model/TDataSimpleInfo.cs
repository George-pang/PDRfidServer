using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// 简单消息实体---用于HttpListener监听
    /// </summary>
    public class TDataSimpleInfo
    {
        private string m_MessageType;
        private string m_Content;
         

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
    }
}
