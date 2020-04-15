using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// RFID仓库物联列表---仓库实体类
    /// </summary>
    public class RfidStorage_StorageInfo
    {
        private string m_CangKuID;
        /// <summary> 
        /// 仓库ID
        /// </summary> 
        public string CangKuID
        {
            get { return m_CangKuID; }
            set { this.m_CangKuID = value; }
        }

        private string m_CangKuMingCheng;
        /// <summary> 
        /// 仓库名称
        /// </summary> 
        public string CangKuMingCheng
        {
            get { return m_CangKuMingCheng; }
            set { this.m_CangKuMingCheng = value; }
        }

        private string m_DuXieQiID;
        /// <summary> 
        /// 读写器ID
        /// </summary> 
        public string DuXieQiID
        {
            get { return m_DuXieQiID; }
            set { this.m_DuXieQiID = value; }
        }
    }
}
