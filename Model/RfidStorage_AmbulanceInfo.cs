using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// RFID车辆物联列表---车辆实体类
    /// </summary>
    public class RfidStorage_AmbulanceInfo
    {
        private string m_CangKuID;
        /// <summary> 
        /// 仓库ID
        /// 因为车联车辆是在FIXED_Storage表中单独维护的,所以暂时就用仓库编码字段
        /// 如果要对应车的车辆ID,可能考虑FIXED_Storage表添加字段绑定车牌号对应的车辆id，管理Web仓库维护页面需要对应调整
        /// </summary> 
        public string CangKuID
        {
            get { return m_CangKuID; }
            set { this.m_CangKuID = value; }
        }

        private string m_ChePaiHao;
        /// <summary> 
        /// 车牌号
        /// </summary> 
        public string ChePaiHao
        {
            get { return m_ChePaiHao; }
            set { this.m_ChePaiHao = value; }
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
