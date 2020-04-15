using DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    /// <summary>
    /// RFID---设备 业务处理BLL
    /// </summary>
    public class RfidMaterialBLL
    {
        private RfidMaterialDAL dal = new RfidMaterialDAL();


        /// <summary>
        /// 获取对应读写器ID下绑定的设备列表
        /// </summary>
        /// <param name="ref_errMsg_gbe"></param>
        /// <returns></returns>
        public List<FIXED_MatetialInfo> GetBindEquipmentList(string CangKuID, ref string ref_errMsg_gbe)
        {
            return dal.GetBindEquipmentList(CangKuID, ref ref_errMsg_gbe);
        }

        /// <summary>
        /// 获取RFID卡号为空的设备列表
        /// </summary>
        /// <param name="ref_errMsg_gse"></param>
        /// <returns></returns>
        public List<FIXED_MatetialInfo> GetSelectEquipmentList(ref string ref_errMsg_gse)
        {
            return dal.GetSelectEquipmentList(ref ref_errMsg_gse);
        }

        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <param name="ref_errMsg_gelr"></param>
        /// <returns></returns>
        public List<FIXED_MatetialInfo> GetEquipmentList(ref string ref_errMsg_gelr)
        {
            return dal.GetEquipmentList(ref ref_errMsg_gelr);
        }
    }
}
