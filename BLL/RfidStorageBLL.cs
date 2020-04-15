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
    /// RFID仓库---读写器BLL
    /// </summary>
    public class RfidStorageBLL
    {
        private RfidStorageDAL dal = new RfidStorageDAL();

        /// <summary>
        /// 获取车辆物联列表
        /// </summary>
        /// <param name="ref_errMsg"></param>
        /// <returns></returns>
        public List<RfidStorage_AmbulanceInfo> GetCarCoupletList(ref string ref_errMsg)
        {
            return dal.GetCarCoupletList(ref ref_errMsg);
        }

        /// <summary>
        /// 获取仓库物联列表
        /// </summary>
        /// <param name="ref_errMsg2"></param>
        /// <returns></returns>
        public List<RfidStorage_StorageInfo> GetObjectAssociationList(ref string ref_errMsg2)
        {
            return dal.GetObjectAssociationList(ref ref_errMsg2);
        }

        /// <summary>
        /// 获取仓库(对象)列表请求(包括车辆和分站仓库)
        /// </summary>
        /// <param name="ref_errMsg_gslr"></param>
        /// <returns></returns>
        public List<GetStorageListResponseInfo> GetStorageList(ref string ref_errMsg_gslr)
        {
            return dal.GetStorageList(ref ref_errMsg_gslr);
        }
    }
}
