using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// 告警查询请求实体
    /// add2020-03-24 
    /// </summary>
    public class AlarmQueryInfo
    {
        private string m_CangKuBianMa;
        /// <summary> 
        /// 仓库编码
        /// </summary> 
        public string CangKuBianMa
        {
            get { return m_CangKuBianMa; }
            set { this.m_CangKuBianMa = value; }
        }

        private string m_SheBeiID;
        /// <summary> 
        /// 设备ID
        /// </summary> 
        public string SheBeiID
        {
            get { return m_SheBeiID; }
            set { this.m_SheBeiID = value; }
        } 

        private string m_GaoJingLeiXingBianMa;
        /// <summary> 
        /// 告警类型编码
        /// </summary> 
        public string GaoJingLeiXingBianMa
        {
            get { return m_GaoJingLeiXingBianMa; }
            set { this.m_GaoJingLeiXingBianMa = value; }
        }
        private string m_KaiShiShiJian;
        /// <summary> 
        /// 开始时间
        /// </summary> 
        public string KaiShiShiJian
        {
            get { return m_KaiShiShiJian; }
            set { this.m_KaiShiShiJian = value; }
        }
        private string m_JieShuShiJian;
        /// <summary> 
        /// 结束时间
        /// </summary> 
        public string JieShuShiJian
        {
            get { return m_JieShuShiJian; }
            set { this.m_JieShuShiJian = value; }
        }

    }
}
